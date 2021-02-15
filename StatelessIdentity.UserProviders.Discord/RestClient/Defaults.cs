
using System;

namespace StatelessIdentity.UserProviders.Discord.RestClient
{
    public static class Defaults
    {
        public static TimeSpan HttpClientTimeout = TimeSpan.FromSeconds(10);
        public static string TokenUrl = "https://discord.com/api/oauth2/token";
        public static string GetUserUrl = "https://discord.com/api/users/@me";
    }
}
