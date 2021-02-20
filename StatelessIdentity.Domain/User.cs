using StatelessIdentity.Domain.Extensions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StatelessIdentity.Domain
{
    public class User
    {
        [JsonPropertyName("digest")]
        public string Digest { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("data")]
        public IDictionary<string, string> Data { get; set; }
        
        public User() { }

        public User(string provider, string externalId)
        {
            Digest = HashingExtensions.ComputeSHA512Base64Hash(provider + externalId);
        }

        public string GetData(string key)
        {
            if (Data == null)
                return null;

            return Data.TryGetValue(key, out string value) ? value : null;
        }
    }
}
