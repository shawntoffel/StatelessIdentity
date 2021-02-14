using System;
using System.Text.Json;

namespace StatelessIdentity.Domain
{
    public class TokenOptions
    {
        public TimeSpan ExpirationTimeSpan { get; set; }

        public JsonSerializerOptions JsonSerializerOptions { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public TokenOptions()
        {
            ExpirationTimeSpan = TimeSpan.FromDays(30);
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                IgnoreNullValues = true
            };
            Issuer = "*";
            Audience = "*";
        }
    }
}
