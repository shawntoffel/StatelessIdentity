using StatelessIdentity.UserProviders.Discord.RestClient.Models;
using System.Threading.Tasks;

namespace StatelessIdentity.UserProviders.Discord.RestClient
{
    public interface IDiscordRestClient
    {
        Task<TokenResponse> ExchangeCode(string code);
        Task<UserResponse> GetUser(string accessToken);
    }
}
