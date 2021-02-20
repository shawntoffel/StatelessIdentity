using StatelessIdentity.Domain.Extensions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StatelessIdentity.Domain
{
    public class User
    {
        [JsonPropertyName("h")]
        public string Hash { get; set; }

        [JsonPropertyName("n")]
        public string Name { get; set; }

        [JsonPropertyName("d")]
        public IDictionary<string, string> Data { get; set; }
        
        internal User()
        {
            Data = new Dictionary<string, string>();
        }

        public User(string provider, string externalId) : this()
        {
            Hash = HashingExtensions.ComputeSHA512Base64Hash(provider + externalId);
        }

        public string GetData(string key)
        {
            if (Data == null)
                return null;

            return Data.TryGetValue(key, out string value) ? value : null;
        }
    }
}
