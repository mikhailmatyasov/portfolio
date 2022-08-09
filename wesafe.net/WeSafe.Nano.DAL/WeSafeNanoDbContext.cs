using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeSafe.DAL;
using WeSafe.DAL.Entities;

namespace WeSafe.Nano.DAL
{
    public class WeSafeNanoDbContext : WeSafeDbContext
    {
        public WeSafeNanoDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public override void Seed(IServiceScope serviceScope)
        {
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<string>>>();
            CreateRoles(roleManager);

            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

            CreateAdministrator(userManager);

            //var defaultUsername = "user";
            //var user = CreateUser(userManager, defaultUsername);

            CreateDevice();
            CreateGlobalSettings();

            FillRtspPath(serviceScope);
        }

        private void CreateDevice()
        {
            if(Devices.Any())
                return;

            Devices.Add(new Device()
            {
                ActivationDate = DateTimeOffset.UtcNow,
                Name = "Device Name",
                MACAddress = GetMACAddress(),
                MaxActiveCameras = Int32.MaxValue,
                Token = "AB1298FF"
            });

            SaveChanges();
        }

        private User CreateUser(UserManager<User> userManager, string username, string password = default)
        {
            var user = userManager.FindByNameAsync(username).Result;
            if (user != null)
                return user;

            user = new User
            {
                UserName = username,
                DisplayName = "Master",
                IsActive = true,
                IsDeleted = false,
            };

            userManager.CreateAsync(user, string.IsNullOrWhiteSpace(password) ? "123456" : password).Wait();
            userManager.AddToRoleAsync(user, WeSafe.Shared.Roles.UserRoles.Users).Wait();

            return user;
        }

        private string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics.Where(a =>
                a.NetworkInterfaceType == NetworkInterfaceType.Ethernet && a.OperationalStatus == OperationalStatus.Up))
            {
                return FormatMacAddress(adapter.GetPhysicalAddress().ToString());
            }

            return FormatMacAddress("00000000");
        }

        private string FormatMacAddress(string macAddress)
        {
            var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
            var replace = "$1:$2:$3:$4:$5:$6";
            return Regex.Replace(macAddress, regex, replace);
        }
    }
}
