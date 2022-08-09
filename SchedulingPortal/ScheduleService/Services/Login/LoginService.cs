using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.Login;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScheduleService.Services.Login
{
    public class LoginService : ILoginService
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<LoginService> _logger;

        public LoginService(HttpClient httpClient, ILogger<LoginService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<LoginResultDto> LoginAsync(string login, string password)
        {
            var isValid = false;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            const string url = "public/singlelogin.aspx?fromwhere=app";

            var requestParams = new Dictionary<string, string>
            {
                {
                    "username", login
                },
                {
                    "password", password
                },
                {
                    "action", "Submit"
                },
            };

            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.LoginMeasurement,
                "public/singlelogin.aspx");
            HttpResponseMessage response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(requestParams));
            performanceMeter.Stop();

            string responseBody = await response.Content.ReadAsStringAsync();

            if (IsLoginValid(responseBody, out string transferUrl))
            {
                isValid = true;
            }
            else
            {
                _logger.LogWarning($"Login request failed. Response html:{responseBody}");
            }

            return new LoginResultDto
            {
                Success = isValid,
                MyAccountUrl = isValid ? $"{_httpClient.BaseAddress}{transferUrl}" : string.Empty,
            };
        }

        public string GetForgotPasswordUrl()
        {
            return $"{_httpClient.BaseAddress}Foundation/Webforms/Public/ForgotPassword.aspx?fromwhere=mobileapp";
        }

        private static bool IsLoginValid(string responseBody, out string transferUrl)
        {
            transferUrl = string.Empty;

            if (!string.IsNullOrWhiteSpace(responseBody)
                && responseBody.Contains("/public/logintransfer.asp?login=true"))
            {
                transferUrl = GetTransferUrl(responseBody);
                return true;
            }

            return false;
        }

        private static string GetTransferUrl(string responseBody)
        {
            int start = responseBody.IndexOf("public/logintransfer.asp?login=true", StringComparison.Ordinal);
            int length = responseBody[start..].IndexOf("\">", StringComparison.Ordinal);

            return WebUtility.HtmlDecode(responseBody.Substring(start, length));
        }
    }
}
