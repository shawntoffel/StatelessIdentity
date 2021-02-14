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
        public void TestCreateIdentity()
        {
            var guidProvider = new GuidUserProvider();

            var sip = new StatelessIdentityProvider();
            sip.RegisterUserProvider(guidProvider);

            var identity = sip.CreateIdentity(new AuthorizationContext()
            {
                ProviderId = guidProvider.Id.ToString()
            });

            Assert.AreEqual(identity.User.ProviderId, guidProvider.Id);
        }
    }
}
