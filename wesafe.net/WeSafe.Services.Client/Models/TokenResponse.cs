using System;

namespace WeSafe.Services.Client.Models
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Role { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool Demo { get; set; }
    }
}