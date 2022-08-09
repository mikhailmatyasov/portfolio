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
    [Migration("20200531094949_Add_Unhandled_Exceptions")]
    partial class Add_Unhandled_Exceptions
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

                    b.Property<int?>("CameraTypeId");

                    b.Property<int>("DeviceId");

                    b.Property<int>("Direction");

                    b.Property<int>("DirectionLeft");

                    b.Property<int>("DirectionRight");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsForRecognition");

                    b.Property<DateTimeOffset?>("LastActivityTime");

                    b.Property<string>("LastImagePath");

                    b.Property<string>("Login")
                        .HasMaxLength(100);

                    b.Property<string>("Password")
                        .HasMaxLength(200);

                    b.Property<string>("Port")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<string>("RecognitionSettings");

                    b.Property<string>("Roi");

                    b.Property<string>("Schedule");

                    b.Property<int?>("SeparateIndex");

                    b.Property<string>("SpecificRtcpConnectionString");

                    b.Property<string>("Status")
                        .HasMaxLength(50);

                    b.Property<byte>("TimeStartRecord");

                    b.Property<byte>("TimeStopRecord");

                    b.HasKey("Id");

                    b.HasIndex("CameraName");

                    b.HasIndex("CameraTypeId");

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

                    b.Property<string>("Parameters");

                    b.Property<DateTimeOffset>("Time");

                    b.HasKey("Id");

                    b.HasIndex("CameraId");

                    b.HasIndex("Time");

                    b.ToTable("CameraLogs");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.CameraLogEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CameraLogId");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("TypeKey");

                    b.Property<DateTimeOffset?>("UrlExpiration");

                    b.HasKey("Id");

                    b.HasIndex("CameraLogId");

                    b.ToTable("CameraLogEntries");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.CameraManufactor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Manufactor");

                    b.HasKey("Id");

                    b.ToTable("CameraManufactors");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.CameraMark", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CameraManufactorId");

                    b.Property<string>("Model");

                    b.HasKey("Id");

                    b.HasIndex("CameraManufactorId");

                    b.ToTable("CameraMarks");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.CameraType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CameraVendorId");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<string>("RtspTemplate")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CameraVendorId");

                    b.ToTable("CameraTypes");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.CameraVendor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("CameraVendors");
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

            modelBuilder.Entity("WeSafe.DAL.Entities.ClientSubscriber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ClientId");

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasDefaultValue("Unnamed");

                    b.Property<string>("Permissions");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.HasIndex("ClientId", "Phone")
                        .IsUnique();

                    b.ToTable("ClientSubscribers");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.ClientSubscriberSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CameraId");

                    b.Property<int>("ClientSubscriberId");

                    b.Property<DateTimeOffset?>("Mute");

                    b.HasKey("Id");

                    b.HasIndex("CameraId");

                    b.HasIndex("ClientSubscriberId");

                    b.ToTable("ClientSubscriberSettings");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset?>("ActivationDate");

                    b.Property<DateTimeOffset?>("AssemblingDate");

                    b.Property<string>("AuthToken");

                    b.Property<int?>("ClientId");

                    b.Property<string>("ClientNetworkIp");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CurrentSshPassword")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("44JxTOaKbCyO0v5E/0vzFJn0xfdwnFJzdVWkrVOPHMs=");

                    b.Property<string>("HWVersion");

                    b.Property<string>("Info");

                    b.Property<bool>("IsArmed");

                    b.Property<DateTime>("LastUpdateDatePassword")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<string>("MACAddress");

                    b.Property<string>("NVIDIASn");

                    b.Property<string>("Name")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasDefaultValue("Unnamed");

                    b.Property<string>("PreviousSshPassword");

                    b.Property<string>("SWVersion");

                    b.Property<string>("SerialNumber");

                    b.Property<string>("Status");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("Token");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.DeviceLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CameraId");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamptz");

                    b.Property<int>("DeviceId");

                    b.Property<string>("ErrorMessage")
                        .IsRequired();

                    b.Property<int>("LogLevel");

                    b.HasKey("Id");

                    b.HasIndex("CameraId");

                    b.HasIndex("DeviceId");

                    b.ToTable("DeviceLogs");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.Email", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MailAddress")
                        .IsRequired();

                    b.Property<bool>("NotifyServerException");

                    b.HasKey("Id");

                    b.ToTable("Emails");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.MobileDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirebaseToken");

                    b.Property<int>("MobileUserId");

                    b.HasKey("Id");

                    b.HasIndex("MobileUserId");

                    b.ToTable("MobileDevices");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.MobileUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<DateTimeOffset?>("Mute");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("Status");

                    b.HasKey("Id");

                    b.HasIndex("Phone")
                        .IsUnique();

                    b.ToTable("MobileUsers");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.PermittedAdminIp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IpAddress");

                    b.HasKey("Id");

                    b.ToTable("PermittedAdminIps");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.RtspPath", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CameraMarkId");

                    b.Property<string>("Path");

                    b.HasKey("Id");

                    b.HasIndex("CameraMarkId");

                    b.ToTable("RtspPaths");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.TelegramUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<DateTimeOffset?>("Mute");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("Settings");

                    b.Property<string>("Status");

                    b.Property<long>("TelegramId");

                    b.HasKey("Id");

                    b.HasIndex("Phone")
                        .IsUnique();

                    b.HasIndex("TelegramId")
                        .IsUnique();

                    b.ToTable("TelegramUsers");
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.UnhandledException", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("ErrorMessage")
                        .IsRequired();

                    b.Property<string>("StackTrace")
                        .IsRequired();

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UnhandledExceptions");
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
                    b.HasOne("WeSafe.DAL.Entities.CameraType", "CameraType")
                        .WithMany()
                        .HasForeignKey("CameraTypeId");

                    b.HasOne("WeSafe.DAL.Entities.Device", "Device")
                        .WithMany("Cameras")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.CameraLog", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.Camera", "Camera")
                        .WithMany("CameraLogs")
                        .HasForeignKey("CameraId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.CameraLogEntry", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.CameraLog", "CameraLog")
                        .WithMany("Entries")
                        .HasForeignKey("CameraLogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.CameraMark", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.CameraManufactor", "CameraManufactor")
                        .WithMany("CameraMarks")
                        .HasForeignKey("CameraManufactorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.CameraType", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.CameraVendor", "CameraVendor")
                        .WithMany("CameraTypes")
                        .HasForeignKey("CameraVendorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.ClientSubscriber", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.Client", "Client")
                        .WithMany("Subscribers")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.ClientSubscriberSettings", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.Camera", "Camera")
                        .WithMany()
                        .HasForeignKey("CameraId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WeSafe.DAL.Entities.ClientSubscriber", "ClientSubscriber")
                        .WithMany("Settings")
                        .HasForeignKey("ClientSubscriberId")
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

            modelBuilder.Entity("WeSafe.DAL.Entities.DeviceLog", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.Camera", "Camera")
                        .WithMany("DeviceLogs")
                        .HasForeignKey("CameraId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WeSafe.DAL.Entities.Device", "Device")
                        .WithMany("DeviceLogs")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.MobileDevice", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.MobileUser", "MobileUser")
                        .WithMany("Devices")
                        .HasForeignKey("MobileUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.RtspPath", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.CameraMark", "CameraMark")
                        .WithMany("RtspPaths")
                        .HasForeignKey("CameraMarkId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WeSafe.DAL.Entities.UnhandledException", b =>
                {
                    b.HasOne("WeSafe.DAL.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
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
