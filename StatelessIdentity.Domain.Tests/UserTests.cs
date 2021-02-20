using NUnit.Framework;
using System;

namespace StatelessIdentity.Domain.Tests
{
    [TestFixture]
    public class UserTests
    {
        [Test]
        [TestCase("b01c16bb-f15e-444a-a300-0a2e22f757a7", "externalId", "U8bInS7AG23jxoIFfXHffG3p5WvlGUkVa1HAt4FxeKE4JTwNUpqDwM2cIGNcYmbHuakk3TeCQCUmshQ6BsRl0w==")]
        [TestCase("b01c16bb-f15e-444a-a300-0a2e22f757a7", "", "7Cdu0Tyh9F9nrd3w6MX++81mniPs+Q7XXlKkY2ZzvGthobMlV84hCR2GfKMnV4H0f9lvoRisKo6W7REfYIY24w==")]
        [TestCase("b01c16bb-f15e-444a-a300-0a2e22f757a7", null, "7Cdu0Tyh9F9nrd3w6MX++81mniPs+Q7XXlKkY2ZzvGthobMlV84hCR2GfKMnV4H0f9lvoRisKo6W7REfYIY24w==")]
        public void TestCreatesExpectedHash(string provider, string externalId, string expectedHash)
        {
            var user = new User(Guid.Parse(provider).ToString(), externalId);

            Assert.AreEqual(expectedHash, user.Digest);
        }
    }
}