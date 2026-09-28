#!/usr/bin/env python3
"""
Hand-written mutation checks.

Every mutation in a spec file is applied to a copy of the repository, the tests
it targets are run, and the mutation must make at least one of them fail.
Afterwards every test method of the targeted test classes must have failed for
at least one mutation, i.e. no test is kept that was never seen failing.

Spec files are JSON lists of objects:
  {
    "file":    "BookingCalendarApi.Services/UserService.cs",  # repository relative
    "find":    "if (existingUser != null)",                   # must occur exactly once (or use "occurrence")
    "replace": "if (existingUser == null)",
    "tests":   "UserServiceTests",                            # dotnet test filter: FullyQualifiedName~<tests>
    "occurrence": 2                                           # optional, 1-based, when "find" is not unique
  }
An optional "project" key overrides the test project (defaults to the unit tests).
A mutation with a "skip" key (the reason, e.g. an equivalent mutant) is documented but not run.

Usage: tests/mutation-checks/run.py [--workers N] [--check] spec.json [spec.json ...]
"""
import argparse
import json
import os
import re
import shutil
import subprocess
import sys
import tempfile
import threading
from concurrent.futures import ThreadPoolExecutor
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DEFAULT_PROJECT = "tests/BookingCalendarApi.UnitTests"
FAILED_RE = re.compile(r"^\s+Failed (\S+?)(\(.*\))? \[", re.M)
COPY_IGNORE = shutil.ignore_patterns(".git", "bin", "obj", "StrykerOutput", "TestResults")


def method_of(test_name):
    return test_name.split("(")[0]


def apply(text, mutation):
    find, occurrence = mutation["find"], mutation.get("occurrence")
    count = text.count(find)
    if occurrence is None:
        if count != 1:
            raise ValueError(f"'find' occurs {count} times (expected exactly once)")
        occurrence = 1
    elif occurrence > count:
        raise ValueError(f"'find' occurs {count} times, occurrence {occurrence} requested")
    index = -1
    for _ in range(occurrence):
        index = text.index(find, index + 1)
    return text[:index] + mutation["replace"] + text[index + len(find):]


def run_tests(workdir, project, test_filter):
    result = subprocess.run(
        ["dotnet", "test", project, "--nologo", "--filter", f"FullyQualifiedName~{test_filter}",
         "--logger", "console;verbosity=normal"],
        cwd=workdir, capture_output=True, text=True)
    return result.returncode, result.stdout + result.stderr


def list_tests(project, test_filters):
    result = subprocess.run(["dotnet", "test", project, "--nologo", "--list-tests"],
                            cwd=ROOT, capture_output=True, text=True, check=True)
    names = [line.strip() for line in result.stdout.splitlines() if line.startswith("    ")]
    return {method_of(n) for n in names if any(f"{f}." in n or n.endswith(f) for f in test_filters)}


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("specs", nargs="+")
    parser.add_argument("--workers", type=int, default=1)
    parser.add_argument("--check", action="store_true", help="only validate that every mutation applies")
    args = parser.parse_args()

    mutations = []
    for spec in args.specs:
        for i, mutation in enumerate(json.loads(Path(spec).read_text())):
            mutation.setdefault("project", DEFAULT_PROJECT)
            mutation["id"] = f"{Path(spec).stem}#{i + 1}"
            if "skip" in mutation:
                print(f"[SKIPPED] {mutation['id']} {mutation['file']}: {mutation['find'].strip()!r} ({mutation['skip']})")
                continue
            mutations.append(mutation)

    # sanity check every mutation before spending time on builds
    problems = []
    for m in mutations:
        try:
            apply((ROOT / m["file"]).read_text(encoding="utf-8-sig"), m)
        except (ValueError, FileNotFoundError) as error:
            problems.append(f"{m['id']} {m['file']}: {error}")
    if problems:
        print("Invalid mutations:\n  " + "\n  ".join(problems))
        return 2
    if args.check:
        print(f"{len(mutations)} mutations apply cleanly")
        return 0

    workdirs = []
    for n in range(args.workers):
        workdir = Path(tempfile.mkdtemp(prefix=f"mutation-worker-{n}-"))
        shutil.rmtree(workdir)
        shutil.copytree(ROOT, workdir, ignore=COPY_IGNORE)
        workdirs.append(workdir)
    free = list(workdirs)
    lock = threading.Lock()

    def check(mutation):
        with lock:
            workdir = free.pop()
        try:
            target = workdir / mutation["file"]
            original = target.read_bytes()
            bom = original.startswith(b"\xef\xbb\xbf")
            target.write_bytes((b"\xef\xbb\xbf" if bom else b"") + apply(original.decode("utf-8-sig"), mutation).encode())
            try:
                code, output = run_tests(workdir, mutation["project"], mutation["tests"])
            finally:
                target.write_bytes(original)
        finally:
            with lock:
                free.append(workdir)
        failed = sorted({method_of(name) for name, _ in FAILED_RE.findall(output)})
        if re.search(r"error CS\d+", output):
            status = "COMPILE_ERROR"
        elif failed:
            status = "KILLED"
        elif code != 0:
            status = "ERROR"
        else:
            status = "SURVIVED"
        return mutation, status, failed, output

    results = []
    with ThreadPoolExecutor(max_workers=args.workers) as pool:
        for mutation, status, failed, output in pool.map(check, mutations):
            short = [f.split(".")[-1] for f in failed]
            print(f"[{status}] {mutation['id']} {mutation['file']}: {mutation['find'].strip()!r} -> {mutation['replace'].strip()!r}")
            if short:
                print(f"           failing: {', '.join(short)}")
            if status in ("ERROR", "COMPILE_ERROR"):
                print("\n".join(line for line in output.splitlines() if "error" in line.lower())[:2000])
            results.append((mutation, status, failed))
            sys.stdout.flush()

    for workdir in workdirs:
        shutil.rmtree(workdir, ignore_errors=True)

    exit_code = 0
    survived = [m for m, status, _ in results if status != "KILLED"]
    if survived:
        exit_code = 1
        print(f"\n{len(survived)} mutation(s) not killed: {', '.join(m['id'] for m in survived)}")

    by_project = {}
    for mutation, _, failed in results:
        entry = by_project.setdefault(mutation["project"], (set(), set()))
        entry[0].add(mutation["tests"])
        entry[1].update(failed)
    for project, (filters, failed) in by_project.items():
        never_failed = sorted(list_tests(project, filters) - failed)
        if never_failed:
            exit_code = 1
            print(f"\nTests in {project} never failed under any mutation:\n  " + "\n  ".join(never_failed))

    print(f"\n{sum(1 for _, s, _ in results if s == 'KILLED')}/{len(results)} mutations killed")
    return exit_code


if __name__ == "__main__":
    sys.exit(main())
