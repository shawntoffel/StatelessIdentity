using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Logging;
using NUnit.Framework;
using StatelessIdentity.Domain.Exceptions;

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
            var user = new User() {
                ProviderId = Guid.Parse("b01c16bb-f15e-444a-a300-0a2e22f757a7"),
                ExternalId = "externalId"
            };

            var identity = new Identity(user);

            var token = identity.Token(SymmetricKey);
            Assert.NotNull(token);

            var parsedIdentity = Identity.Parse(token, SymmetricKey);
            Assert.AreEqual(parsedIdentity.Id, identity.Id);
            Assert.AreEqual(parsedIdentity.User.ProviderId, identity.User.ProviderId);
            Assert.AreEqual(parsedIdentity.User.ExternalId, identity.User.ExternalId);
        }

        [Test]
        public void TestCreateAndParseTokenAsymmetricKey()
        {
            var user = new User()
            {
                ProviderId = Guid.Parse("b01c16bb-f15e-444a-a300-0a2e22f757a7"),
                ExternalId = "externalId"
            };

            var identity = new Identity(user);

            var token = identity.Token(AsymmetricKey);
            Assert.NotNull(token);

            var parsedIdentity = Identity.Parse(token, AsymmetricKey);
            Assert.AreEqual(parsedIdentity.Id, identity.Id);
            Assert.AreEqual(parsedIdentity.User.ProviderId, identity.User.ProviderId);
            Assert.AreEqual(parsedIdentity.User.ExternalId, identity.User.ExternalId);
        }

        [Test]
        public void TestSymmetricKeyCreate_SmallKey_ThrowsException()
        {
            var identity = new Identity(new User());
            Assert.Throws<TokenException>(() => identity.Token("test"));
        }
    }
}
