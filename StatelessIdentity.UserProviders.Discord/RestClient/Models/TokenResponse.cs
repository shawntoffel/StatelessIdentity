using System.Text.Json.Serialization;

namespace StatelessIdentity.UserProviders.Discord.RestClient.Models
{
    internal class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
