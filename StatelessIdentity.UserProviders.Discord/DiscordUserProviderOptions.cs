namespace StatelessIdentity.UserProviders.Discord
{
    public class DiscordUserProviderOptions
    {
        public const string ConfigurationSection = "DiscordUserProvider";

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }

        public DiscordUserProviderOptions()
        {
            Scope = "identify";
        }
    }
}
