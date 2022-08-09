using System;

namespace WeSafe.Authentication.WebApi.Models
{
    public class TokenRequestModel
    {
        public TokenRequestModel(string identifier, string name, DateTime expires)
        {
            if (String.IsNullOrWhiteSpace(identifier))
                throw new ArgumentNullException(nameof(identifier));
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Identifier = identifier;
            Name = name;
            Expires = expires;
        }

        public string Identifier { get; }

        public string Name { get; }

        public string DisplayName { get; set; }

        public string Role { get; set; }

        public DateTime Expires { get; set; }

        public bool Demo { get; set; }

        public int? ClientId { get; set; }
    }
}
