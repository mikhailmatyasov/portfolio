using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WeSafe.DAL;
using WeSafe.DAL.Entities;

namespace WeSafe.Authentication.Integration.Tests
{
    public class AuthenticationWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault
                    (d => d.ServiceType == typeof(DbContextOptions<WeSafeDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<WeSafeDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<WeSafeDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<AuthenticationWebApplicationFactory<TStartup>>>();

                    db.Database.EnsureCreated();

                    try
                    {
                        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<string>>>();
                        CreateRoles(roleManager);

                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                        CreateAdministrator(userManager);
                        CreateUser(userManager, "user", null);
                        CreateDevice(db, "Device", "00:30:48:5a:58:65", "123456abc");

                        var client = CreateClient(db, "client", "+71234567890");

                        CreateClientSubscriber(db, client);

                        RegisterClient(db, userManager);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }

        private void RegisterClient(WeSafeDbContext db, UserManager<User> userManager)
        {
            var client = CreateClient(db, "client1", "+71234567895");

            CreateClientSubscriber(db, client);
            CreateUser(userManager, "user1", null, client.Id);
            CreateDevice(db, "Device1", "1a:30:48:5a:58:65", "723456abc", client.Id);
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

        protected void CreateUser(UserManager<User> userManager, string name, string password, int? clientId = null)
        {
            var user = userManager.FindByNameAsync(name).Result;

            if (user == null)
            {
                user = new User
                {
                    UserName = name,
                    DisplayName = "TestUser",
                    IsActive = true,
                    IsDeleted = false,
                    ClientId = clientId
                };

                userManager.CreateAsync(user, string.IsNullOrWhiteSpace(password) ? "123456" : password).Wait();
                userManager.AddToRoleAsync(user, Shared.Roles.UserRoles.Users).Wait();
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

        protected void CreateDevice(WeSafeDbContext dbContext, string name, string mac, string token, int? clientId = null)
        {
            var device = new Device
            {
                MACAddress = mac,
                Name = name,
                Token = token,
                ClientId = clientId,
                AssemblingDate = DateTimeOffset.UtcNow,
                ActivationDate = clientId != null ? DateTimeOffset.UtcNow : (DateTimeOffset?)null
            };

            dbContext.Devices.Add(device);
            dbContext.SaveChanges();
        }

        protected Client CreateClient(WeSafeDbContext dbContext, string name, string phone)
        {
            var client = new Client
            {
                Name = name,
                Phone = phone,
                CreatedAt = DateTimeOffset.UtcNow,
                IsActive = true,
                Token = "123456"
            };

            dbContext.Clients.Add(client);
            dbContext.SaveChanges();

            return client;
        }

        protected ClientSubscriber CreateClientSubscriber(WeSafeDbContext dbContext, Client client)
        {
            var subscriber = new ClientSubscriber
            {
                Client = client,
                Name = "subscriber",
                Phone = client.Phone,
                IsActive = true,
            };

            dbContext.ClientSubscribers.Add(subscriber);
            dbContext.SaveChanges();

            return subscriber;
        }
    }
}
