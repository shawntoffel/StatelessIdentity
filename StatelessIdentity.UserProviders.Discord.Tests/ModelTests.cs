using NUnit.Framework;
using StatelessIdentity.UserProviders.Discord.RestClient.Models;

namespace StatelessIdentity.UserProviders.Discord.Tests
{
    [TestFixture]
    public class ModelTests
    {
        [Test]
        public void TestUserResponseGetAvatarUrl()
        {
            var userResponse = new UserResponse()
            {
                Id = "my_id",
                Avatar = "my_avatar"
            };

            var expected = $"https://cdn.discordapp.com/avatars/{userResponse.Id}/{userResponse.Avatar}.png";

            Assert.AreEqual(expected, userResponse.GetAvatarUrl());
        }
    }
}
