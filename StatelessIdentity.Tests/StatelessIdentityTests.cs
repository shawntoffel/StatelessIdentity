using NUnit.Framework;
using StatelessIdentity;
using StatelessIdentity.Domain;
using StatelessIdentity.UserProviders.Guid;

namespace IdentitySession.Tests
{
    [TestFixture]
    public class IdentitySessionTests
    {
        [Test]
        public void Test()
        {
            var guidProvider = new GuidUserProvider();

            var sip = new StatelessIdentityProvider();
            sip.RegisterUserProvider(guidProvider);

            var identity = sip.CreateIdentity(new AuthorizationContext()
            {
                ProviderId = guidProvider.Id.ToString()
            });

            identity.User.ExternalId = "test";
            var hash = identity.User.GetBase64Hash();
            var key = "asdv234234^&%&^%&^hjsdfb2%%%";
            var token = identity.Token(key);

            var t = Identity.Parse(token, key);
            Assert.AreEqual(t.Id, identity.Id);

            Assert.AreEqual(hash, "PjZPcZCIKg2kPtN/BYjMQgEF2aANdjZ2mE+7+Oj29cXxCynehI8vbCYcHWnaohmTlkOA04FdKOQsBgp3vKmt5Q==");
            Assert.AreEqual(identity.User.ProviderId, guidProvider.Id);
        }
    }
}
