using System.Text.Json.Serialization;

namespace StatelessIdentity.UserProviders.Discord.RestClient.Models
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
