using Moq;
using NUnit.Framework;
using StatelessIdentity.Domain;
using StatelessIdentity.UserProviders.Discord.RestClient;
using StatelessIdentity.UserProviders.Discord.RestClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            _userProvider = new DiscordUserProvider(discordOpts, _mockDiscordRestClient.Object);
            _statelessIdentityProvider = new StatelessIdentityProvider();
            _statelessIdentityProvider.RegisterUserProvider(_userProvider);
        }

        [Test]
        public void TestSetsUser()
        {
            var auth = new AuthorizationContext()
            {
                ProviderId = _userProvider.Id.ToString(),
                Data = new Dictionary<string, string>()
                {
                    {"code", "test"}
                } 
            };

            var identity = _statelessIdentityProvider.CreateIdentity(auth);
            Assert.NotNull(identity?.User);

            Assert.AreEqual(identity.User.ProviderId, _userProvider.Id);
            Assert.AreEqual(identity.User.ExternalId, _defaultUserResponse.Id);
            Assert.AreEqual(identity.User.Name, _defaultUserResponse.Username);

            var data = identity.User?.Data;
            Assert.NotNull(data);
            Assert.IsTrue(data.ContainsKey("avatarUrl"));
            Assert.AreEqual(data["avatarUrl"], _defaultUserResponse.GetAvatarUrl());
        }
    }
}