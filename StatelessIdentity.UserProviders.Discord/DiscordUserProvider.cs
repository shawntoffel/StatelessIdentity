using StatelessIdentity.Domain;
using StatelessIdentity.UserProviders.Discord.RestClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace StatelessIdentity.UserProviders.Discord
{
    public class DiscordUserProvider : IUserProvider
    {
        public string Name => nameof(DiscordUserProvider);

        private readonly IDiscordRestClient _restClient;

        public DiscordUserProvider(DiscordUserProviderOptions options)
        {
            _restClient = new DiscordRestClient(options);
        }

        public DiscordUserProvider(DiscordUserProviderOptions options, HttpClient httpClient)
        {
            _restClient = new DiscordRestClient(options, httpClient);
        }

        public DiscordUserProvider(DiscordUserProviderOptions options, IDiscordRestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<User> GetUserAsync(AuthorizationContext authorizationContext)
        {
            if (authorizationContext == null)
                throw new ArgumentNullException(nameof(authorizationContext));

            var code = authorizationContext.GetData("code");

            var token = await _restClient.ExchangeCode(code);
            var userResponse = await _restClient.GetUser(token.AccessToken);

            return new User(Name, userResponse.Id)
            {
                Name = userResponse.Username,
                Data = new Dictionary<string, string>()
                {
                    {"avatarUrl", userResponse.GetAvatarUrl()}
                }
            };
        }
    }
}
