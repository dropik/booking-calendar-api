# Booking Calendar API
RESTful web service for booking calendar app.

## What for?
It serves for connecting together all of the endpoints involved in this app:
the web app itself on the client side, Iperbooking for retrieving booking and guests data,
MySQL database with room and color assignments,
Police web service for publishing arrived people data, ISTAT web service for similar data publishing purpose.

## How to run?
The API can run on both .NET Core and .NET Framework. If you chose to run on .NET Core set **BookingCalendarApi** as 
startup project and make sure that Package Manager Console is set to work with **BookingCalendarApi.Repository.NETCore** 
project for database operations. If instead you want to run on .NET Framework, the startup project must be 
**BookingCalendarApi.NETFramework** as well as the project for Package Manager Console.

### Configure

For running the API locally do the following steps:
1. Set user secrets for database and other:
```
dotnet user-secrets set "DB:ConnectionString" "Server=<mysql-server-address>; Database=<database-name>; Uid=<username>; Pwd=<password>"
dotnet user-secrets set "JWT:Key" "<your JWT key>"
dotnet user-secrets set "JWT:Issuer" "<your JWT issuer>"
dotnet user-secrets set "MasterPassword" "<password for master account>"
```
2. Update database using PM console:
```
Update-Database
```
Or via .NET CLI:
```
dotnet database update
```
3. Run it:
```
dotnet run
```

### Further steps

 - You can work either with **Master** hotel or you can create your own directly in the database. If you want to work with 
your own structure, you can add a user for it by calling **POST api/v1/users** endpoint.

 - For actually utilizing the critical features of the API, the credentials for Iperbooking, Portale Alloggiati and ISTAT must be
provided. To configure those, from settings page in the app go to *Chiavi API* and configure all the required keys.

## Testing

The tests cover the .NET Core stack (the .NET Framework projects share the same services and models). You need the
.NET 6 SDK; the mutation testing tool additionally needs the .NET 8 runtime.

### Unit tests

```
dotnet test tests/BookingCalendarApi.UnitTests
```

Services run against an in-memory `IRepository` that behaves like EF Core (changes are visible only after
`SaveChangesAsync`, ids are generated on save), external services are replaced with NSubstitute fakes and
Iperbooking with a stub `HttpMessageHandler`.

### Integration tests

```
dotnet test tests/BookingCalendarApi.IntegrationTests
```

The API is hosted in-process (`WebApplicationFactory`, environment `Testing`) on top of a real MySQL 8 database
created with the EF Core migrations. MySQL is started with Testcontainers, so Docker must be running; to use an
existing server instead set `TEST_MYSQL_CONNECTION_STRING` (e.g. `Server=localhost;Uid=root;Pwd=root`), every run
creates and drops its own database. The Alloggiati and C59 SOAP clients are substitutes and Iperbooking is served by
an in-process fake, so no external service is contacted.

### Mutation testing

Tests are checked with mutation testing, i.e. by verifying that they fail when the code they cover is changed.

[Stryker.NET](https://stryker-mutator.io/docs/stryker-net/introduction/) mutates the code automatically; the build
fails when the mutation score drops below 80%:

```
dotnet tool restore
cd stryker
dotnet stryker --config-file unit-services.json
dotnet stryker --config-file unit-models.json
dotnet stryker --config-file integration-host.json          # slow, uses the integration tests
dotnet stryker --config-file integration-repository.json    # slow, uses the integration tests
```

The HTML reports end up in `stryker/StrykerOutput`. Mutants that cannot be killed because they do not change the
behaviour are disabled in the code with a `// Stryker disable once <mutator> : <reason>` comment.

On top of that, `tests/mutation-checks` contains hand written mutations of the production code together with the
tests that must catch them. The script applies each mutation to a copy of the repository and checks that it makes a
test fail, and that every test fails for at least one mutation, so no test is kept that never proved it can fail:

```
python3 tests/mutation-checks/run.py --workers 4 tests/mutation-checks/unit/*.json tests/mutation-checks/integration/*.json
```

When a change makes a mutation no longer apply, the script says so: update the mutation to the new code.

All of this runs in GitHub Actions (`.github/workflows/tests.yml`): unit and integration tests and unit mutation
testing on every push and pull request, the slower integration mutation testing also on pushes to `master` and
`develop`, and the hand written mutation checks weekly and on demand.

## Changelog

### v1.6.2
- Configured stage selection in AWS environment.

### v1.6.1
- Added support for hosting on AWS Lambda.

### v1.6.0
- Added support for .NET Framework 4.
- Introduced JWT authentication and role based authorization (only admin supported for now).
- Introduced structure which contains all its API secrets.
- Separating data at repository level which belongs to the structrue of the current user.
- Placed user related and structure data into single /users/current endpoint.
- Removed the concept of session.
- Collapsed assignments into single resource.
- Fixed collision detection.

#### *Versions skipped up to 1.6 to sync with frontend*

### v1.2.8
- Added wether deposit is made with a bank transfer to booking response.

### v1.2.7
- Added deposit information to booking response.

### v1.2.6
- ISTAT data must be fetched first and then reuploaded, so that eventual changes could be consumed.
- Better validation and grouping of ISTAT data before send.

### v1.2.5
- Adjusted some specific nations that are missing in ISTAT documentaion, and other nation description typos.
- Added Pakistan nation.

### v1.2.4
- Reversed ISTAT nation descriptions as of version 1.2.1.

### v1.2.3
- Restored original Germany and USA country names, as it was giving errors in ISTAT.

### v1.2.2
- Adjusted nation descriptions in seeding for better ISTAT compatibility.

### v1.2.1
- Added exception filter. More advanced status codes handling.
- Handling connection errors and given correct status code for that.
- Added entities configurations. Seeding data directly from configuration.

### v1.2.0
- Added base board information to tiles.
- Fixed minor bugs.

### v1.1.2
- Fixed correct tracked records order by record type.

### v1.1.1
- Fixed missing date specified field in ISTAT publication.

### v1.1.0
- Added ISTAT endpoint to send new ISTAT data and to get last published date.
- Fixed wrong police data row serialization.
- Added nation descriptions to nations table.
- Better police place of birth search by description.
- Fixed simultaneous context bound stuff on police data generation.
- Fixed taking bookings from following date when publishing to police.
