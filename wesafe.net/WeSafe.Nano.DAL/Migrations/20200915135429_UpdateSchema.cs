using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeSafe.Nano.DAL.Migrations
{
    public partial class UpdateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CameraManufactors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Manufacturer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraManufactors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CameraVendors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraVendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    ContractNumber = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    Info = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    SendToDevChat = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MailAddress = table.Column<string>(nullable: true),
                    NotifyServerException = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Phone = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Mute = table.Column<DateTimeOffset>(nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermittedAdminIps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IpAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermittedAdminIps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Phone = table.Column<string>(nullable: true),
                    TelegramId = table.Column<long>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Settings = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Mute = table.Column<DateTimeOffset>(nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CameraMarks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CameraManufactorId = table.Column<int>(nullable: false),
                    CameraManufacturerId = table.Column<int>(nullable: true),
                    Model = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraMarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CameraMarks_CameraManufactors_CameraManufacturerId",
                        column: x => x.CameraManufacturerId,
                        principalTable: "CameraManufactors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CameraTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    RtspTemplate = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    CameraVendorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CameraTypes_CameraVendors_CameraVendorId",
                        column: x => x.CameraVendorId,
                        principalTable: "CameraVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Demo = table.Column<bool>(nullable: false),
                    ClientId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientSubscribers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Permissions = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSubscribers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSubscribers_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MobileDevices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MobileUserId = table.Column<int>(nullable: false),
                    FirebaseToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MobileDevices_MobileUsers_MobileUserId",
                        column: x => x.MobileUserId,
                        principalTable: "MobileUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RtspPaths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(nullable: true),
                    CameraMarkId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RtspPaths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RtspPaths_CameraMarks_CameraMarkId",
                        column: x => x.CameraMarkId,
                        principalTable: "CameraMarks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SerialNumber = table.Column<string>(nullable: true),
                    HWVersion = table.Column<string>(nullable: true),
                    SWVersion = table.Column<string>(nullable: true),
                    NVIDIASn = table.Column<string>(nullable: true),
                    MACAddress = table.Column<string>(nullable: true),
                    ClientId = table.Column<int>(nullable: true),
                    ClientNetworkIp = table.Column<string>(nullable: true),
                    ActivationDate = table.Column<DateTimeOffset>(nullable: true),
                    AssemblingDate = table.Column<DateTimeOffset>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Info = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    NetworkStatus = table.Column<string>(nullable: true),
                    IsArmed = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CurrentSshPassword = table.Column<string>(nullable: true),
                    PreviousSshPassword = table.Column<string>(nullable: true),
                    LastUpdateDatePassword = table.Column<DateTime>(nullable: false),
                    AuthToken = table.Column<string>(nullable: true),
                    MaxActiveCameras = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devices_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnhandledExceptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    StackTrace = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnhandledExceptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnhandledExceptions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cameras",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CameraName = table.Column<string>(nullable: true),
                    Ip = table.Column<string>(nullable: true),
                    Port = table.Column<string>(nullable: true),
                    Login = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Roi = table.Column<string>(nullable: true),
                    Schedule = table.Column<string>(nullable: true),
                    SpecificRtcpConnectionString = table.Column<string>(nullable: true),
                    DeviceId = table.Column<int>(nullable: false),
                    LastImagePath = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    NetworkStatus = table.Column<string>(nullable: true),
                    CameraTypeId = table.Column<int>(nullable: true),
                    RecognitionSettings = table.Column<string>(nullable: true),
                    LastActivityTime = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cameras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cameras_CameraTypes_CameraTypeId",
                        column: x => x.CameraTypeId,
                        principalTable: "CameraTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cameras_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceIndicators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<int>(nullable: false),
                    CpuUtilization = table.Column<double>(nullable: true),
                    GpuUtilization = table.Column<double>(nullable: true),
                    MemoryUtilization = table.Column<double>(nullable: true),
                    Temperature = table.Column<double>(nullable: true),
                    CamerasFps = table.Column<string>(nullable: true),
                    Traffic = table.Column<double>(nullable: true),
                    Time = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceIndicators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceIndicators_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LicensePlateRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LicensePlate = table.Column<string>(nullable: true),
                    DeviceId = table.Column<int>(nullable: false),
                    LicensePlateType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicensePlateRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicensePlateRestrictions_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CameraLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CameraId = table.Column<int>(nullable: false),
                    Alert = table.Column<bool>(nullable: false),
                    Parameters = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Time = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CameraLogs_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientSubscriberAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientSubscriberId = table.Column<int>(nullable: false),
                    DeviceId = table.Column<int>(nullable: false),
                    CameraId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSubscriberAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSubscriberAssignments_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientSubscriberAssignments_ClientSubscribers_ClientSubscriberId",
                        column: x => x.ClientSubscriberId,
                        principalTable: "ClientSubscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientSubscriberAssignments_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientSubscriberSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientSubscriberId = table.Column<int>(nullable: false),
                    CameraId = table.Column<int>(nullable: false),
                    Mute = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSubscriberSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSubscriberSettings_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientSubscriberSettings_ClientSubscribers_ClientSubscriberId",
                        column: x => x.ClientSubscriberId,
                        principalTable: "ClientSubscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<int>(nullable: false),
                    CameraId = table.Column<int>(nullable: true),
                    LogLevel = table.Column<int>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceLogs_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeviceLogs_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlateEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<int>(nullable: false),
                    CameraId = table.Column<int>(nullable: false),
                    PlateNumber = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlateEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlateEvents_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlateEvents_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrafficEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceMAC = table.Column<string>(nullable: true),
                    CameraId = table.Column<int>(nullable: false),
                    UtcDateTime = table.Column<DateTime>(nullable: false),
                    Direction = table.Column<int>(nullable: false),
                    ObjectId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrafficEvents_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CameraLogEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CameraLogId = table.Column<int>(nullable: false),
                    TypeKey = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    UrlExpiration = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraLogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CameraLogEntries_CameraLogs_CameraLogId",
                        column: x => x.CameraLogId,
                        principalTable: "CameraLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Frames",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlateEventId = table.Column<int>(nullable: false),
                    ImageType = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Frames_PlateEvents_PlateEventId",
                        column: x => x.PlateEventId,
                        principalTable: "PlateEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlateEventStates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlateEventId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlateEventStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlateEventStates_PlateEvents_PlateEventId",
                        column: x => x.PlateEventId,
                        principalTable: "PlateEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ClientId",
                table: "AspNetUsers",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CameraLogEntries_CameraLogId",
                table: "CameraLogEntries",
                column: "CameraLogId");

            migrationBuilder.CreateIndex(
                name: "IX_CameraLogs_CameraId",
                table: "CameraLogs",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_CameraMarks_CameraManufacturerId",
                table: "CameraMarks",
                column: "CameraManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_CameraTypeId",
                table: "Cameras",
                column: "CameraTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_DeviceId",
                table: "Cameras",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_CameraTypes_CameraVendorId",
                table: "CameraTypes",
                column: "CameraVendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriberAssignments_CameraId",
                table: "ClientSubscriberAssignments",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriberAssignments_ClientSubscriberId",
                table: "ClientSubscriberAssignments",
                column: "ClientSubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriberAssignments_DeviceId",
                table: "ClientSubscriberAssignments",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscribers_ClientId",
                table: "ClientSubscribers",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriberSettings_CameraId",
                table: "ClientSubscriberSettings",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriberSettings_ClientSubscriberId",
                table: "ClientSubscriberSettings",
                column: "ClientSubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceIndicators_DeviceId",
                table: "DeviceIndicators",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLogs_CameraId",
                table: "DeviceLogs",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLogs_DeviceId",
                table: "DeviceLogs",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ClientId",
                table: "Devices",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_CreatedBy",
                table: "Devices",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Frames_PlateEventId",
                table: "Frames",
                column: "PlateEventId");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlateRestrictions_DeviceId",
                table: "LicensePlateRestrictions",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_MobileDevices_MobileUserId",
                table: "MobileDevices",
                column: "MobileUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlateEvents_CameraId",
                table: "PlateEvents",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_PlateEvents_DeviceId",
                table: "PlateEvents",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_PlateEventStates_PlateEventId",
                table: "PlateEventStates",
                column: "PlateEventId");

            migrationBuilder.CreateIndex(
                name: "IX_RtspPaths_CameraMarkId",
                table: "RtspPaths",
                column: "CameraMarkId");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficEvents_CameraId",
                table: "TrafficEvents",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_UnhandledExceptions_UserId",
                table: "UnhandledExceptions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CameraLogEntries");

            migrationBuilder.DropTable(
                name: "ClientSubscriberAssignments");

            migrationBuilder.DropTable(
                name: "ClientSubscriberSettings");

            migrationBuilder.DropTable(
                name: "DeviceIndicators");

            migrationBuilder.DropTable(
                name: "DeviceLogs");

            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropTable(
                name: "Frames");

            migrationBuilder.DropTable(
                name: "LicensePlateRestrictions");

            migrationBuilder.DropTable(
                name: "MobileDevices");

            migrationBuilder.DropTable(
                name: "PermittedAdminIps");

            migrationBuilder.DropTable(
                name: "PlateEventStates");

            migrationBuilder.DropTable(
                name: "RtspPaths");

            migrationBuilder.DropTable(
                name: "TelegramUsers");

            migrationBuilder.DropTable(
                name: "TrafficEvents");

            migrationBuilder.DropTable(
                name: "UnhandledExceptions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CameraLogs");

            migrationBuilder.DropTable(
                name: "ClientSubscribers");

            migrationBuilder.DropTable(
                name: "MobileUsers");

            migrationBuilder.DropTable(
                name: "PlateEvents");

            migrationBuilder.DropTable(
                name: "CameraMarks");

            migrationBuilder.DropTable(
                name: "Cameras");

            migrationBuilder.DropTable(
                name: "CameraManufactors");

            migrationBuilder.DropTable(
                name: "CameraTypes");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "CameraVendors");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
