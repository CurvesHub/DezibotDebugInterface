﻿// <auto-generated />
using System;
using DezibotDebugInterface.Api.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DezibotDebugInterface.Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class DezibotDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("ClassDezibot", b =>
                {
                    b.Property<int>("ClassesId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DezibotId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ClassesId", "DezibotId");

                    b.HasIndex("DezibotId");

                    b.ToTable("ClassDezibot");
                });

            modelBuilder.Entity("ClassProperty", b =>
                {
                    b.Property<int>("ClassId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PropertiesId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ClassId", "PropertiesId");

                    b.HasIndex("PropertiesId");

                    b.ToTable("ClassProperty");
                });

            modelBuilder.Entity("DezibotDebugInterface.Api.DataAccess.Models.Class", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Class");
                });

            modelBuilder.Entity("DezibotDebugInterface.Api.DataAccess.Models.Dezibot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("LastConnectionUtc")
                        .HasColumnType("TEXT");

                    b.Property<int?>("SessionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Ip");

                    b.HasIndex("SessionId");

                    b.ToTable("Dezibots");
                });

            modelBuilder.Entity("DezibotDebugInterface.Api.DataAccess.Models.DezibotHubClient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConnectionId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("DezibotDebugInterface.Api.DataAccess.Models.Property", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Property");
                });

            modelBuilder.Entity("DezibotDebugInterface.Api.DataAccess.Models.Session", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("CreatedUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("DezibotDebugInterface.Api.DataAccess.Models.SessionClientConnection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ClientId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ReceiveUpdates")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SessionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("SessionId");

                    b.ToTable("SessionClientConnection");
                });

            modelBuilder.Entity("DezibotDebugInterface.Api.DataAccess.Models.TimeValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("TimestampUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TimeValue");
                });

            modelBuilder.Entity("PropertyTimeValue", b =>
                {
                    b.Property<int>("PropertyId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ValuesId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PropertyId", "ValuesId");

                    b.HasIndex("ValuesId");

                    b.ToTable("PropertyTimeValue");
                });

            modelBuilder.Entity("ClassDezibot", b =>
                {
                    b.HasOne("DezibotDebugInterface.Api.DataAccess.Models.Class", null)
                        .WithMany()
                        .HasForeignKey("ClassesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DezibotDebugInterface.Api.DataAccess.Models.Dezibot", null)
                        .WithMany()
                        .HasForeignKey("DezibotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ClassProperty", b =>
                {
                    b.HasOne("DezibotDebugInterface.Api.DataAccess.Models.Class", null)
                        .WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DezibotDebugInterface.Api.DataAccess.Models.Property", null)
                        .WithMany()
                        .HasForeignKey("PropertiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DezibotDebugInterface.Api.DataAccess.Models.Dezibot", b =>
                {
                    b.HasOne("DezibotDebugInterface.Api.DataAccess.Models.Session", null)
                        .WithMany("Dezibots")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsMany("DezibotDebugInterface.Api.DataAccess.Models.LogEntry", "Logs", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER");

                            b1.Property<string>("ClassName")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("Data")
                                .HasColumnType("TEXT");

                            b1.Property<int>("DezibotId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("LogLevel")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("Message")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<DateTimeOffset>("TimestampUtc")
                                .HasColumnType("TEXT");

                            b1.HasKey("Id");

                            b1.HasIndex("DezibotId");

                            b1.ToTable("LogEntry");

                            b1.WithOwner()
                                .HasForeignKey("DezibotId");
                        });

                    b.Navigation("Logs");
                });

            modelBuilder.Entity("DezibotDebugInterface.Api.DataAccess.Models.SessionClientConnection", b =>
                {
                    b.HasOne("DezibotDebugInterface.Api.DataAccess.Models.DezibotHubClient", "Client")
                        .WithMany("Sessions")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DezibotDebugInterface.Api.DataAccess.Models.Session", "Session")
                        .WithMany("SessionClientConnections")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Session");
                });

            modelBuilder.Entity("PropertyTimeValue", b =>
                {
                    b.HasOne("DezibotDebugInterface.Api.DataAccess.Models.Property", null)
                        .WithMany()
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DezibotDebugInterface.Api.DataAccess.Models.TimeValue", null)
                        .WithMany()
                        .HasForeignKey("ValuesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DezibotDebugInterface.Api.DataAccess.Models.DezibotHubClient", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("DezibotDebugInterface.Api.DataAccess.Models.Session", b =>
                {
                    b.Navigation("Dezibots");

                    b.Navigation("SessionClientConnections");
                });
#pragma warning restore 612, 618
        }
    }
}
