﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WeSafe.DAL;

namespace WeSafe.DAL.Migrations
{
    [DbContext(typeof(WeSafeDbContext))]
    [Migration("20191221170303_Add_Base_Entities")]
    partial class Add_Base_Entities
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<string>", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.Camera", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CameraName")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<int>("DeviceId");

                    b.Property<int>("Direction");

                    b.Property<int>("DirectionLeft");

                    b.Property<int>("DirectionRight");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsForRecognition");

                    b.Property<string>("LastImagePath");

                    b.Property<string>("Login")
                        .HasMaxLength(100);

                    b.Property<string>("Password")
                        .HasMaxLength(50);

                    b.Property<string>("Port")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<string>("Roi");

                    b.Property<string>("Schedule");

                    b.Property<int?>("SeparateIndex");

                    b.Property<string>("SpecificRtcpConnectionString");

                    b.Property<byte>("TimeStartRecord");

                    b.Property<byte>("TimeStopRecord");

                    b.HasKey("Id");

                    b.HasIndex("CameraName")
                        .IsUnique();

                    b.HasIndex("DeviceId");

                    b.ToTable("Cameras");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.CameraLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Alert");

                    b.Property<int>("CameraId");

                    b.Property<string>("Message");

                    b.Property<DateTimeOffset>("Time");

                    b.HasKey("Id");

                    b.HasIndex("CameraId");

                    b.HasIndex("Time");

                    b.ToTable("CameraLogs");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContractNumber");

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("Info");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name")
                        .HasMaxLength(250);

                    b.Property<string>("Phone")
                        .HasMaxLength(50);

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("Token");

                    b.HasIndex("CreatedAt", "IsActive");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset?>("ActivationDate");

                    b.Property<DateTimeOffset?>("AssemblingDate");

                    b.Property<int?>("ClientId");

                    b.Property<string>("ClientNetworkIp");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("HWVersion");

                    b.Property<string>("Info");

                    b.Property<string>("MACAddress");

                    b.Property<string>("NVIDIASn");

                    b.Property<string>("SWVersion");

                    b.Property<string>("SerialNumber");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("Token");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.MessengerUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ClientId");

                    b.Property<string>("Permissions");

                    b.Property<string>("Phone");

                    b.Property<string>("Schedule");

                    b.Property<string>("Settings");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("MessengerUsers");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<int?>("ClientId");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("DisplayName")
                        .HasMaxLength(250);

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<string>")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<string>")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WeSafe.DAL.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.Camera", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.Device", "Device")
                        .WithMany("Cameras")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.CameraLog", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.Camera", "Cameras")
                        .WithMany("CameraLogs")
                        .HasForeignKey("CameraId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.Device", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId");

                    b.HasOne("WeSafe.DAL.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("CreatedBy");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.MessengerUser", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.Client", "Client")
                        .WithMany("MessengerUsers")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.User", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId");
                });
#pragma warning restore 612, 618
        }
    }
}
