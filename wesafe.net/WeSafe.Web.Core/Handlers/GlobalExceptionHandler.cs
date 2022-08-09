using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeSafe.Services;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Consts;

namespace WeSafe.Web.Core.Handlers
{
    public static class GlobalExceptionHandlerExtension
    {
        #region Fields

        private const string _contentType = "application/json";

        #endregion

        #region Public Methods

        /// <summary>
        /// Handle exceptions.
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="isDevelopmentEnvironment"> Environment is development</param>
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app, bool isDevelopmentEnvironment)
        {
            app.UseExceptionHandler(options => RunOptions(options, isDevelopmentEnvironment));
        }

        #endregion

        #region Private Methods

        private static void RunOptions(IApplicationBuilder options, bool isDevelopmentEnvironment)
        {
            options.Run(async context => await HandleExceptionAsync(options, context, isDevelopmentEnvironment));
        }

        private static async Task HandleExceptionAsync(IApplicationBuilder options, HttpContext context, bool isDevelopmentEnvironment)
        {
            string userId = GetUserId(context);
            Exception exception = context.Features.Get<IExceptionHandlerFeature>().Error;

            LogExceptions(options, exception, userId);
            AddExceptionToQueue(context, exception, userId);

            await IncludeErrorInResponse(context, exception, userId, isDevelopmentEnvironment);
        }

        private static void AddExceptionToQueue(HttpContext context, Exception exception, string userId)
        {
            var exceptionModel = GetExceptionModel(context, exception, userId);
            var logsHandler = context.RequestServices.GetService<UnhandledExceptionsHandlerHostedService>();
            logsHandler?.AddExceptionsToTheQueue(exceptionModel);
        }

        private static UnhandledExceptionModel GetExceptionModel(HttpContext context, Exception exception, string userId)
        {
            var exceptionMapper = context.RequestServices.GetService<ExceptionMapper>();

            return exceptionMapper.ToExceptionModel(exception, userId);
        }

        private static void LogExceptions(IApplicationBuilder app, Exception exception, string userId)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<BaseStartup>>();
            logger.LogError(exception, "Global exception handler. User id: " + userId);
        }

        private static string GetUserId(HttpContext context)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                userId = Consts.defaultUserName;

            return userId;
        }

        private static async Task IncludeErrorInResponse(HttpContext context, Exception exError, string userId, bool isDevelopmentEnvironment)
        {
            ErrorModel error = new ErrorModel
            {
                Code = (int)HttpStatusCode.InternalServerError,
                UserId = userId,
                ErrorMessage = exError.Message
            };

            if (isDevelopmentEnvironment)
                error.StackTrace = exError.StackTrace;

            context.Response.ContentType = _contentType;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
        }

        #endregion
    }
}
