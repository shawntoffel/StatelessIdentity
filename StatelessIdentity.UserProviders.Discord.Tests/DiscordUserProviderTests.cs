using NUnit.Framework;
using StatelessIdentity.Domain;

namespace StatelessIdentity.UserProviders.Discord.Tests
{
    public class DiscordUserProviderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Ignore("integration test")]
        public void Test1()
        {
            var discordOpts = new DiscordUserProviderOptions()
            {
                ClientId = "",
                ClientSecret = "",
                RedirectUri = "",
            };

            var dup = new DiscordUserProvider(discordOpts);

            var sip = new StatelessIdentityProvider();
            sip.RegisterUserProvider(dup);

            var auth = new AuthorizationContext()
            {
                ProviderId = dup.Id.ToString(),
            };
            auth.Data.Add("code", "");

            var identity = sip.CreateIdentity(auth);

            var opts = new TokenOptions()
            {
                Audience = "test",
                Issuer = "test",
            };

            var key = "asdv234234^&%&^%&^hjsdfb2%%%";

            var token = identity.Token(key, opts);
            var parsed = Identity.Parse(token, key, opts);
        }
    }
}