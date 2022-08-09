using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using WeSafe.Web.Common.Exceptions.Abstract;
using WeSafe.Web.Common.Exceptions.Models;

namespace WeSafe.Web.Common.Exceptions.Handlers
{
    public static class ExceptionHandler
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

            await IncludeErrorInResponse(context, exception, userId, isDevelopmentEnvironment);
        }

        private static void LogExceptions(IApplicationBuilder app, Exception exception, string userId)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<ErrorModel>>();
            logger.LogError(exception, "Global exception handler. User id: " + userId);
        }

        private static string GetUserId(HttpContext context)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                userId = "Anonymous";

            return userId;
        }

        private static async Task IncludeErrorInResponse(HttpContext context, Exception exError, string userId, bool isDevelopmentEnvironment)
        {
            ErrorModel error = GetErrorModel(exError, userId);

            if (isDevelopmentEnvironment)
                error.StackTrace = exError.StackTrace;

            context.Response.ContentType = _contentType;
            context.Response.StatusCode = error.Code;

            await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
        }

        private static ErrorModel GetErrorModel(Exception exError, string userId)
        {
            ErrorModel error = new ErrorModel()
            {
                UserId = userId
            };

            if (exError is IHttpException httpException)
            {
                error.Code = httpException.StatusCode;
                error.ErrorMessage = httpException.GetMessage();
            }
            else
            {
                error.Code = (int)HttpStatusCode.InternalServerError;
                error.ErrorMessage = exError.Message;
            }

            return error;
        }

        #endregion
    }
}
