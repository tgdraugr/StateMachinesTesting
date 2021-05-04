﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StateMachines;

namespace StateMachines.MySql.Oracle.Migrations
{
    [DbContext(typeof(GreetingsDbContext))]
    partial class GreetingsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("StateMachines.GreetingsState", b =>
                {
                    b.Property<byte[]>("CorrelationId")
                        .HasColumnType("varbinary(16)");

                    b.Property<DateTime?>("Created")
                        .HasColumnType("datetime");

                    b.Property<string>("CurrentState")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<int>("Occurrences")
                        .HasColumnType("int");

                    b.Property<string>("Sender")
                        .HasColumnType("text");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime");

                    b.HasKey("CorrelationId");

                    b.ToTable("Greetings");
                });
#pragma warning restore 612, 618
        }
    }
}
