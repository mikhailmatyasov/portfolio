using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ScheduleService.DataAccess.TokenProvider
{
    public class AccuWeatherTokenProvider : IWeatherTokenProvider
    {
        private readonly string _jwtIss;

        private readonly string _jwtSecret;

        public AccuWeatherTokenProvider(string jwtIss, string jwtSecret)
        {
            _jwtIss = jwtIss;
            _jwtSecret = jwtSecret;
        }

        public string GetProvider()
        {
            return "accuweather";
        }

        public string GetToken()
        {
            var header = new
            {
                alg = "HS256",
                typ = "JWT",
            };

            byte[] utf8Header = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(header));
            string encodedHeader = ToBase64Url(utf8Header);
            var payload = new
            {
                iss = _jwtIss,
            };
            var utf8Payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
            var encodedPayload = ToBase64Url(utf8Payload);
            var token = encodedHeader + "." + encodedPayload;

            HMACSHA256 provider = new HMACSHA256(Encoding.UTF8.GetBytes(_jwtSecret));
            byte[] hash = provider.ComputeHash(Encoding.UTF8.GetBytes(token));
            string signature = ToBase64Url(hash);
            var signedToken = token + "." + signature;
            return signedToken;
        }

        private string ToBase64Url(byte[] source)
        {
            string base64 = Convert.ToBase64String(source);

            // Remove padding
            base64 = base64.Replace("=", string.Empty);

            // Replace special chars
            base64 = base64.Replace('+', '-');
            base64 = base64.Replace('/', '_');
            return base64;
        }
    }
}
