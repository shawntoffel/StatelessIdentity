using Microsoft.Extensions.Options;
using StatelessIdentity.Domain.Exceptions;
using StatelessIdentity.UserProviders.Discord.RestClient.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace StatelessIdentity.UserProviders.Discord.RestClient
{
    public class DiscordRestClient : IDiscordRestClient
    {
        private readonly HttpClient _httpClient;
        private readonly DiscordUserProviderOptions _discordUserProviderOptions;

        public DiscordRestClient(DiscordUserProviderOptions options)
        {
            _discordUserProviderOptions = options ?? new DiscordUserProviderOptions();

            _httpClient = new HttpClient
            {
                Timeout = Defaults.HttpClientTimeout
            };
        }

        public DiscordRestClient(DiscordUserProviderOptions options, HttpClient httpClient)
        {
            _discordUserProviderOptions = options ?? new DiscordUserProviderOptions();
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<TokenResponse> ExchangeCode(string code)
        {
            var body = new Dictionary<string, string>() {
                {"client_id", _discordUserProviderOptions.ClientId},
                {"client_secret", _discordUserProviderOptions.ClientSecret},
                {"grant_type", "authorization_code"},
                {"code", code},
                {"redirect_uri", _discordUserProviderOptions.RedirectUri},
                {"scope", _discordUserProviderOptions.Scope},
            };

            using var httpContent = new FormUrlEncodedContent(body);
            httpContent.Headers.Clear();
            httpContent.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var response = await _httpClient.PostAsync(Defaults.TokenUrl, httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
                throw new UserProviderException(response.StatusCode, responseContent);

            return JsonSerializer.Deserialize<TokenResponse>(responseContent);
        }

        public async Task<UserResponse> GetUser(string accessToken)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(Defaults.GetUserUrl),
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
                throw new UserProviderException(response.StatusCode, responseContent);

            return JsonSerializer.Deserialize<UserResponse>(responseContent);
        }
    }
}
