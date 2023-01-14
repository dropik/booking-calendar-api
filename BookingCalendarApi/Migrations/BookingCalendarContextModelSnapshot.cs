﻿// <auto-generated />
using System;
using BookingCalendarApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookingCalendarApi.Migrations
{
    [DbContext(typeof(BookingCalendarContext))]
    partial class BookingCalendarContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("BookingCalendarApi.Models.ColorAssignment", b =>
                {
                    b.Property<string>("BookingId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("BookingId");

                    b.ToTable("ColorAssignments");
                });

            modelBuilder.Entity("BookingCalendarApi.Models.Floor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Floors");
                });

            modelBuilder.Entity("BookingCalendarApi.Models.Nation", b =>
                {
                    b.Property<string>("Iso")
                        .HasColumnType("varchar(255)");

                    b.Property<ulong>("Code")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Iso");

                    b.ToTable("Nations");

                    b.HasData(
                        new
                        {
                            Iso = "UK",
                            Code = 100000219ul,
                            Description = "REGNO UNITO"
                        });
                });

            modelBuilder.Entity("BookingCalendarApi.Models.Room", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("FloorId")
                        .HasColumnType("bigint");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("FloorId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("BookingCalendarApi.Models.RoomAssignment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<long?>("RoomId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.ToTable("RoomAssignments");
                });

            modelBuilder.Entity("BookingCalendarApi.Models.SessionEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("BookingCalendarApi.Models.Room", b =>
                {
                    b.HasOne("BookingCalendarApi.Models.Floor", null)
                        .WithMany("Rooms")
                        .HasForeignKey("FloorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookingCalendarApi.Models.RoomAssignment", b =>
                {
                    b.HasOne("BookingCalendarApi.Models.Room", null)
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("BookingCalendarApi.Models.Floor", b =>
                {
                    b.Navigation("Rooms");
                });
#pragma warning restore 612, 618
        }
    }
}
