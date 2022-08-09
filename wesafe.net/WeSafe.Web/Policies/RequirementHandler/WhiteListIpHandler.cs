using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.Shared.Roles;
using WeSafe.Web.Core.Models;
using WeSafe.Web.Policies.Requirements;
using System.Net;
using Microsoft.Extensions.Logging;

namespace WeSafe.Web.Policies.RequirementHandler
{
    public class WhiteListIpHandler : AuthorizationHandler<WhiteListIpRequirement>
    {
        private readonly WeSafeDbContext _dbContext;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<WhiteListIpHandler> _logger;
        public WhiteListIpHandler(WeSafeDbContext dbContext, IHttpContextAccessor accessor, UserManager<User> userManager, ILogger <WhiteListIpHandler> logger)
        {
            _dbContext = dbContext;
            _accessor = accessor;
            _userManager = userManager;
            _logger = logger;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, WhiteListIpRequirement requirement)
        {
            string includeIpCheckingOption = Environment.GetEnvironmentVariable("IncludeIpChecking");
            bool isHandleRequest = includeIpCheckingOption == "Enabled";

           
            var remoteIp = _accessor.HttpContext.Connection.RemoteIpAddress?.MapToIPv4();
            _logger.LogDebug ($"[LOG] WhiteListIpHandler: remote IP: {remoteIp}\n");
            isHandleRequest &= IsExternalAddress (remoteIp);
            _logger.LogDebug ($"[LOG] WhiteListIpHandler: 41\n");
            if (!isHandleRequest)
            {
                _logger.LogDebug("[LOG] WhiteListIpHandler: 44\n");
                context.Succeed(requirement);
                return;
            }

            string currentClientIp = remoteIp.ToString();
            _logger.LogDebug($"[LOG] WhiteListIpHandler: currentClientIp - {currentClientIp}\n");
            List<string> whiteListIps = await _dbContext.PermittedAdminIps.Select(x => x.IpAddress.ToString()).ToListAsync();
            LoginModel loginModel;

            foreach (var ip in whiteListIps) {
                _logger.LogDebug ($"[LOG] WhiteListIpHandler: whiteListIp - {ip}\n");
            }

            _accessor.HttpContext.Request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(_accessor.HttpContext.Request.ContentLength)];
            await _accessor.HttpContext.Request.Body.ReadAsync(buffer, 0, buffer.Length);

            _accessor.HttpContext.Request.Body.Position = 0;


            using (var reader = new StreamReader(new MemoryStream(buffer)))
            {
                loginModel = JsonConvert.DeserializeObject<LoginModel>(reader.ReadToEnd());
            }

            var user = await _userManager.FindByNameAsync(loginModel?.UserName);

            if (user == null) {
                _logger.LogDebug($"[LOG] WhiteListIpHandler: 74\n");
                return;
            }

            var role = (await _userManager.GetRolesAsync(user)).Single();

            if (role != UserRoles.Administrators) {
                _logger.LogDebug($"[LOG] WhiteListIpHandler: 81\n");
                context.Succeed(requirement);
            }

            if (!whiteListIps.Any()) {
                _logger.LogDebug($"[LOG] WhiteListIpHandler: 86\n");
                return;
            }

            if (whiteListIps.Any(x => x == currentClientIp)) {
                _logger.LogDebug($"[LOG] WhiteListIpHandler: 91\n");
                context.Succeed(requirement);
            }
        }

        private bool IsExternalAddress (IPAddress address)
        {
            if (address == null) {
                return false;
            }

            if (IPAddress.IsLoopback (address)) { 
                return false;
            }

            if (address.ToString().StartsWith ("192.168.")) {
                return false;
            }

            return true;
        }
    }
}
