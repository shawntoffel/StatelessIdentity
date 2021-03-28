using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using StatelessIdentity.Domain;
using StatelessIdentity.UserProviders.Discord.RestClient;
using StatelessIdentity.UserProviders.Discord.RestClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace StatelessIdentity.UserProviders.Discord.Tests
{
    public class DiscordUserProviderTests
    {
        private UserResponse _defaultUserResponse;
        private Mock<IDiscordRestClient> _mockDiscordRestClient;
        private IUserProvider _userProvider;
        private IStatelessIdentityProvider _statelessIdentityProvider;

        [SetUp]
        public void Setup()
        {
            _mockDiscordRestClient = new Mock<IDiscordRestClient>();
            _mockDiscordRestClient.Setup(m => m.ExchangeCode(It.IsAny<string>())).Returns(Task.FromResult(new TokenResponse()));

            _defaultUserResponse = new UserResponse()
            {
                Id = "id",
                Avatar = "1234",
                Username = "test"
            };

            _mockDiscordRestClient.Setup(m => m.GetUser(It.IsAny<string>())).Returns(Task.FromResult(_defaultUserResponse));

            var discordOpts = new DiscordUserProviderOptions();
            _userProvider = new DiscordUserProvider(_mockDiscordRestClient.Object);
            _statelessIdentityProvider = new StatelessIdentityProvider();
            _statelessIdentityProvider.RegisterUserProvider(_userProvider);
        }

        [Test]
        public void TestSetsUser()
        {
            var auth = new AuthorizationContext()
            {
                Provider = _userProvider.Name,
                Data = new Dictionary<string, string>()
                {
                    {"code", "test"}
                } 
            };

            var identity = _statelessIdentityProvider.CreateIdentity(auth);
            Assert.NotNull(identity?.User);
            Assert.AreEqual(_defaultUserResponse.Username, identity.User.Name);

            var data = identity.User?.Data;
            Assert.NotNull(data);
            Assert.IsTrue(data.ContainsKey("avatarUrl"));
            Assert.AreEqual(_defaultUserResponse.GetAvatarUrl(), data["avatarUrl"]);
        }

        [Test]
        public void TestDependencyInjection()
        {
            const string expectedClientId = "clientId";

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    {DiscordUserProviderOptions.ConfigurationSection + ":ClientId", expectedClientId}
                })
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddOptions()
                .Configure<DiscordUserProviderOptions>((s) => configuration.GetSection(DiscordUserProviderOptions.ConfigurationSection).Bind(s))
                .AddTransient<IUserProvider, DiscordUserProvider>()
                .BuildServiceProvider();

            var options = serviceProvider.GetService<IOptions<DiscordUserProviderOptions>>();
            Assert.AreEqual(expectedClientId, options.Value.ClientId);

            var userProvider = serviceProvider.GetService<IUserProvider>();
            Assert.NotNull(userProvider);
        }
    }
}