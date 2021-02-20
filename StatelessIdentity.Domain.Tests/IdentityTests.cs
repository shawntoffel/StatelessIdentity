using Microsoft.IdentityModel.Logging;
using NUnit.Framework;
using StatelessIdentity.Domain.Exceptions;
using System.Security.Cryptography;

namespace StatelessIdentity.Domain.Tests
{
    [TestFixture]
    public class IdentityTests
    {
        private const string SymmetricKey = "asdv234234^&%&^%&^hjsdfb2%%%";
        private readonly RSA AsymmetricKey = RSA.Create(2048);

        [SetUp]
        public void SetUp()
        {
            IdentityModelEventSource.ShowPII = true;
        }

        [Test]
        public void TestCreateAndParseTokenSymmetricKey()
        {
            var user = new User("provider", "externalId");
            var identity = new Identity(user);

            var token = identity.Token(SymmetricKey);
            Assert.NotNull(token);

            var parsedIdentity = Identity.Parse(token, SymmetricKey);
            Assert.AreEqual(identity.Id, parsedIdentity.Id);
            Assert.AreEqual(identity.User.Digest, parsedIdentity.User.Digest);
        }

        [Test]
        public void TestCreateAndParseTokenAsymmetricKey()
        {
            var user = new User("provider", "externalId");

            var identity = new Identity(user);

            var token = identity.Token(AsymmetricKey);
            Assert.NotNull(token);

            var parsedIdentity = Identity.Parse(token, AsymmetricKey);
            Assert.AreEqual(identity.Id, parsedIdentity.Id);
            Assert.AreEqual(identity.User.Digest, parsedIdentity.User.Digest);
        }

        [Test]
        public void TestSymmetricKeyCreate_SmallKey_ThrowsException()
        {
            var identity = new Identity(new User("provider", "externalId"));
            Assert.Throws<TokenException>(() => identity.Token("test"));
        }
    }
}
