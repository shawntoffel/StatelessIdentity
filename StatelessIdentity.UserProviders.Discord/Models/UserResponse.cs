using System.Text.Json.Serialization;

namespace StatelessIdentity.UserProviders.Discord.Models
{
    internal class UserResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        public string GetAvatarUrl()
        {
            return $"https://cdn.discordapp.com/avatars/{Id}/{Avatar}.png";
        }
    }
}
