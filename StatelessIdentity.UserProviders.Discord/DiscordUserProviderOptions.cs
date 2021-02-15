using System;

namespace StatelessIdentity.UserProviders.Discord
{
    public class DiscordUserProviderOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }

        public  DiscordUserProviderOptions()
        {
            Scope = "identify";
        }
    }
}
