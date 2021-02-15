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
        public Guid Id => Guid.Parse("9a69aba6-d0b2-4e35-886e-de54e7c7a307");

        private readonly DiscordRestClient _restClient;

        public DiscordUserProvider(DiscordUserProviderOptions options)
        {
            _restClient = new DiscordRestClient(options);
        }

        public DiscordUserProvider(DiscordUserProviderOptions options, HttpClient httpClient)
        {
            _restClient = new DiscordRestClient(options, httpClient);
        }

        public async Task<User> GetUserAsync(AuthorizationContext authorizationContext)
        {
            var code = authorizationContext.GetData("code");

            var token = await _restClient.ExchangeCode(code);
            var userResponse = await _restClient.GetUser(token.AccessToken);

            return new User()
            {
                ProviderId = Id,
                ExternalId = userResponse.Id,
                Name = userResponse.Username,
                Data = new Dictionary<string, string>()
                {
                    {"avatarUrl", userResponse.GetAvatarUrl()}
                }
            };
        }
    }
}
