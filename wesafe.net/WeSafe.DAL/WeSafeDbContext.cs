using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using WeSafe.DAL.Entities;
using Camera = WeSafe.DAL.Entities.Camera;

namespace WeSafe.DAL
{
    public class WeSafeDbContext : IdentityDbContext<User, IdentityRole<string>, string>
    {
        public DbSet<Camera> Cameras { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Device> Devices { get; set; }

        public DbSet<DeviceLog> DeviceLogs { get; set; }

        public DbSet<CameraLog> CameraLogs { get; set; }

        public DbSet<CameraManufacturer> CameraManufactors { get; set; }

        public DbSet<CameraMark> CameraMarks { get; set; }

        public DbSet<RtspPath> RtspPaths { get; set; }

        public DbSet<PermittedAdminIp> PermittedAdminIps { get; set; }

        public DbSet<ClientSubscriber> ClientSubscribers { get; set; }

        public DbSet<ClientSubscriberSettings> ClientSubscriberSettings { get; set; }

        public DbSet<TelegramUser> TelegramUsers { get; set; }

        public DbSet<MobileUser> MobileUsers { get; set; }

        public DbSet<CameraLogEntry> CameraLogEntries { get; set; }

        public DbSet<MobileDevice> MobileDevices { get; set; }

        public DbSet<Entities.UnhandledException> UnhandledExceptions { get; set; }

        public DbSet<Email> Emails { get; set; }

        public DbSet<ClientSubscriberAssignment> ClientSubscriberAssignments { get; set; }

        public DbSet<TrafficEvent> TrafficEvents { get; set; }

        public DbSet<PlateEvent> PlateEvents { get; set; }

        public DbSet<Frame> Frames { get; set; }

        public DbSet<LicensePlateRestriction> LicensePlateRestrictions { get; set; }

        public DbSet<PlateEventState> PlateEventStates { get; set; }

        public DbSet<DeviceIndicators> DeviceIndicators { get; set; }

        public DbSet<GlobalSettings> GlobalSettings { get; set; }

        public DbSet<RecognitionObject> RecognitionObjects { get; set; }

        public DbSet<DetectedCamera> DetectedCameras { get; set; }

        public WeSafeDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public virtual void Seed(IServiceScope serviceScope)
        {
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<string>>>();
            CreateRoles(roleManager);

            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

            CreateAdministrator(userManager);
            CreateDemoUser(userManager);
            CreateGlobalSettings();

            FillRtspPath(serviceScope);
        }

        protected void CreateDemoUser(UserManager<User> userManager, string password = default)
        {
            var user = userManager.FindByNameAsync("demo").Result;

            if (user == null)
            {
                user = new User
                {
                    UserName = "demo",
                    DisplayName = "Demo user",
                    IsActive = true,
                    IsDeleted = false,
                    Demo = true
                };

                userManager.CreateAsync(user, string.IsNullOrWhiteSpace(password) ? "123456" : password).Wait();
                userManager.AddToRoleAsync(user, Shared.Roles.UserRoles.Users).Wait();
            }
        }

        protected void CreateRoles(RoleManager<IdentityRole<string>> roleManager)
        {
            var role = roleManager.FindByNameAsync(Shared.Roles.UserRoles.Administrators).Result;

            if (role == null)
            {
                role = new IdentityRole
                {
                    Name = Shared.Roles.UserRoles.Administrators,
                };

                roleManager.CreateAsync(role).Wait();
            }

            role = roleManager.FindByNameAsync(Shared.Roles.UserRoles.Users).Result;

            if (role == null)
            {
                role = new IdentityRole
                {
                    Name = Shared.Roles.UserRoles.Users,
                };

                roleManager.CreateAsync(role).Wait();
            }
        }

        protected void CreateAdministrator(UserManager<User> userManager, string password = default)
        {
            var user = userManager.FindByNameAsync("admin").Result;

            if (user == null)
            {
                user = new User
                {
                    UserName = "admin",
                    DisplayName = "Administrator",
                    IsActive = true,
                    IsDeleted = false
                };

                userManager.CreateAsync(user, string.IsNullOrWhiteSpace(password) ? "123456" : password).Wait();
                userManager.AddToRoleAsync(user, Shared.Roles.UserRoles.Administrators).Wait();
            }
        }

        protected void CreateGlobalSettings()
        {
            var settings = GlobalSettings.FirstOrDefault();

            if ( settings == null )
            {
                settings = new GlobalSettings
                {
                    KeepingDeviceLogsDays = 7,
                    KeepingCameraLogsDays = 30
                };

                GlobalSettings.Add(settings);
                SaveChanges();
            }
        }

        protected void FillRtspPath(IServiceScope serviceScope)
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            IEnumerable<string> marks = GetFileText<List<string>>(assemblyFolder, "Content/cameraMarks.json");

            if (!CameraManufactors.Any())
            {
                foreach (string mark in marks)
                {
                    CameraManufacturer cameraManufacturer = new CameraManufacturer
                    {
                        Manufacturer = mark
                    };

                    IEnumerable<CameraMark> cameraMarks = GetFileText<IEnumerable<CameraMark>>(assemblyFolder, "Content/" + mark + ".json");
                    cameraManufacturer.CameraMarks = cameraMarks.ToList();
                    CameraManufactors.Add(cameraManufacturer);
                    SaveChanges();
                }
            }

        }

        private T GetFileText<T>(string assemblyFolder, string fileName)
        {
            string filePath = Path.Combine(assemblyFolder, fileName);
            FileExistanceValidator(filePath);
            string fileText = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<T>(fileText);
        }
        private void FileExistanceValidator(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"{filePath} not found.");
        }
    }
}