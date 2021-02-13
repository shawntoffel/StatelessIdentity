using NUnit.Framework;
using StatelessIdentity;
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

            var identity = sip.CreateIdentity(new StatelessIdentity.Domain.AuthorizationContext()
            {
                ProviderId = guidProvider.Id.ToString()
            });

            identity.User.ExternalId = "test";
            var hash = identity.GetHash();

            Assert.AreEqual(hash, "PjZPcZCIKg2kPtN/BYjMQgEF2aANdjZ2mE+7+Oj29cXxCynehI8vbCYcHWnaohmTlkOA04FdKOQsBgp3vKmt5Q==");
            Assert.AreEqual(identity.ProviderId, guidProvider.Id);
        }
    }
}
