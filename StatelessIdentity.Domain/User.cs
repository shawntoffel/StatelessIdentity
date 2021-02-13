using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using StatelessIdentity.Domain.Extensions;

namespace StatelessIdentity.Domain
{
    public class User
    {
        [JsonPropertyName("pid")]
        public Guid ProviderId { get; set; }

        [JsonPropertyName("eid")]
        public string ExternalId { get; set; }

        [JsonPropertyName("n")]
        public string Name { get; set; }

        [JsonPropertyName("d")]
        public IDictionary<string, string> Data { get; set; }

        public User()
        {
            Data = new Dictionary<string, string>();
        }

        public string GetBase64Hash()
        {
            return HashingExtensions.ComputeBase64Hash(ProviderId.ToString() + ExternalId);
        }
    }
}
