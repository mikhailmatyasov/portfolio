using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class ExceptionMapper
    {
        private readonly UserManager<User> _userManager;

        public ExceptionMapper(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public UnhandledExceptionModel ToExceptionModel(Exception exception, string userId)
        {
            return new UnhandledExceptionModel()
            {
                DateTime = DateTime.UtcNow,
                ErrorMessage = exception.Message,
                StackTrace = exception.StackTrace,
                UserName = _userManager.Users.FirstOrDefault(u => u.Id == userId)?.UserName
            };
        }

        public UnhandledException ToUnhandledException(UnhandledExceptionModel unhandledException)
        {
            return new UnhandledException()
            {
                DateTime = unhandledException.DateTime,
                ErrorMessage = unhandledException.ErrorMessage,
                StackTrace = unhandledException.StackTrace,
                User = _userManager.Users.FirstOrDefault(u => u.Id == unhandledException.UserName)
            };
        }
    }
}
