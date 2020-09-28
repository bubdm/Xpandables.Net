﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xpandables.Net.Api.Storage;

namespace Xpandables.Net.Api.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20200922095313_InitDb")]
    partial class InitDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0-rc.1.20451.13");

            modelBuilder.Entity("Xpandables.Net.Api.Models.Domains.EventLog", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventLogId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("OccuredOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("EventLogId");

                    b.HasIndex("UserId");

                    b.ToTable("EventLog");
                });

            modelBuilder.Entity("Xpandables.Net.Api.Models.Domains.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("Xpandables.Net.Api.Models.Domains.EventLog", b =>
                {
                    b.HasOne("Xpandables.Net.Api.Models.Domains.EventLog", null)
                        .WithMany("EventLogs")
                        .HasForeignKey("EventLogId");

                    b.HasOne("Xpandables.Net.Api.Models.Domains.User", null)
                        .WithMany("EventLogs")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Xpandables.Net.Api.Models.Domains.User", b =>
                {
                    b.OwnsOne("Xpandables.Net.Api.Models.EmailAddress", "Email", b1 =>
                        {
                            b1.Property<string>("UserId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("UserId");

                            b1.ToTable("User");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("Xpandables.Net.Api.Models.PhoneNumber", "Phone", b1 =>
                        {
                            b1.Property<string>("UserId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("UserId");

                            b1.ToTable("User");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("Xpandables.Net.Strings.ValueEncrypted", "Password", b1 =>
                        {
                            b1.Property<string>("UserId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("Key")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Salt")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("UserId");

                            b1.ToTable("User");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("Xpandables.Net.Types.Picture", "Picture", b1 =>
                        {
                            b1.Property<string>("UserId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<byte[]>("Content")
                                .IsRequired()
                                .HasColumnType("varbinary(max)");

                            b1.Property<string>("Extension")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Height")
                                .HasColumnType("int");

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Width")
                                .HasColumnType("int");

                            b1.HasKey("UserId");

                            b1.ToTable("User");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("Email")
                        .IsRequired();

                    b.Navigation("Password")
                        .IsRequired();

                    b.Navigation("Phone")
                        .IsRequired();

                    b.Navigation("Picture")
                        .IsRequired();
                });

            modelBuilder.Entity("Xpandables.Net.Api.Models.Domains.EventLog", b =>
                {
                    b.Navigation("EventLogs");
                });

            modelBuilder.Entity("Xpandables.Net.Api.Models.Domains.User", b =>
                {
                    b.Navigation("EventLogs");
                });
#pragma warning restore 612, 618
        }
    }
}