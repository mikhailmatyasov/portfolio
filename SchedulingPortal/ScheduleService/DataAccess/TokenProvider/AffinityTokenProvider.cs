using System;
using System.Security.Cryptography;
using System.Text;

namespace ScheduleService.DataAccess.TokenProvider
{
    public class AffinityTokenProvider : IAffinityTokenProvider
    {
        private readonly string _moduleId;
        private readonly string _secretKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="AffinityTokenProvider"/> class.
        /// </summary>
        /// <param name="moduleId">The ID assigned to the client module.</param>
        /// <param name="secretKey">The pre-shared (secret) key.</param>
        public AffinityTokenProvider(string moduleId, string secretKey)
        {
            _moduleId = moduleId;
            _secretKey = secretKey;
        }

        /// <summary>
        /// Generates an Affinity API access token.
        /// </summary>
        public string GenerateAffinityApiToken()
        {
            string result = string.Empty;
            DateTime dt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(_moduleId) && !string.IsNullOrWhiteSpace(_secretKey))
            {
                Random rnd = new Random();
                string token = _moduleId + "|" + TimeToUnixTimestamp(dt) + "|" +
                               (rnd.NextDouble() * 100000000000L).ToString("0000000000.000000000");
                result = token + "|" + GenerateHmacSha256(token, _secretKey);
            }

            return result;
        }

        private static double TimeToUnixTimestamp(DateTime dt)
        {
            return (dt.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000;
        }

        private static string GenerateHmacSha256(string data, string key)
        {
            string result = string.Empty;

            HMACSHA256 hs = new HMACSHA256();
            Encoding e = Encoding.UTF8;

            byte[] bKey = e.GetBytes(key);
            byte[] bData = e.GetBytes(data);
            hs.Key = bKey;

            byte[] hmac = hs.ComputeHash(bData);

            if (hmac.Length > 0)
            {
                StringBuilder hexString = new StringBuilder(3 * hmac.Length);

                foreach (var t in hmac)
                {
                    hexString.AppendFormat("{0:x2}", t);
                }

                result = hexString.ToString();
            }

            return result;
        }
    }
}
