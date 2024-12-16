﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using flight_assistant_backend.Api.Data;

#nullable disable

namespace flight_assistant_backend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("flight_assistant_backend.Data.Models.Country", b =>
                {
                    b.Property<string>("Code3")
                        .HasColumnType("text");

                    b.Property<string>("Code2")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Visited")
                        .HasColumnType("boolean");

                    b.HasKey("Code3");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("flight_assistant_backend.Data.Models.Flight", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ArrivalAirport")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ArrivalTime")
                        .HasColumnType("timestamp");

                    b.Property<string>("DepartureAirport")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("DepartureTime")
                        .HasColumnType("timestamp");

                    b.Property<bool>("HasTargetPrice")
                        .HasColumnType("boolean");

                    b.Property<int>("LayoverDuration")
                        .HasColumnType("integer");

                    b.Property<int>("NumberLayovers")
                        .HasColumnType("integer");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<string>("SearchUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TotalDuration")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Flights");
                });

            modelBuilder.Entity("flight_assistant_backend.Data.Models.FlightQuery", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ArrivalAirport")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DepartureAirport")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("DepartureTime")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("ReturnTime")
                        .HasColumnType("timestamp");

                    b.Property<float>("TargetPrice")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("FlightQueries");
                });

            modelBuilder.Entity("flight_assistant_backend.Data.Models.TravelDestionation", b =>
                {
                    b.Property<string>("Code3")
                        .HasColumnType("text");

                    b.Property<DateTime>("TravelDate")
                        .HasColumnType("timestamp");

                    b.HasKey("Code3", "TravelDate");

                    b.ToTable("TravelDestinations");
                });

            modelBuilder.Entity("flight_assistant_backend.Data.Models.TravelDestionation", b =>
                {
                    b.HasOne("flight_assistant_backend.Data.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("Code3")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });
#pragma warning restore 612, 618
        }
    }
}