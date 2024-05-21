﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace SmartMirror.Migrations
{
    [DbContext(typeof(SmartMirrorContext))]
    partial class SmartMirrorContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.17");

            modelBuilder.Entity("SmartMirror.Models.ConditionalRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("MotionEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<float>("TemperatureThreshold")
                        .HasColumnType("REAL");

                    b.Property<double>("UpdateInterval")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("ConditionalRule");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            MotionEnabled = true,
                            TemperatureThreshold = 25f,
                            UpdateInterval = 1.0
                        });
                });

            modelBuilder.Entity("SmartMirror.Models.SensorData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("LightLevel")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Temperature")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SensorData");
                });
#pragma warning restore 612, 618
        }
    }
}
