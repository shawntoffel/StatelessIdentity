using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using StatelessIdentity;
using StatelessIdentity.Domain;
using StatelessIdentity.Domain.Exceptions;
using StatelessIdentity.UserProviders.Guid;
using System.Collections.Generic;

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
                Provider = guidProvider.Name
            });

            Assert.NotNull(identity?.User);
        }

        [Test]
        public void TestDuplicateRegistrationThrowsDuplicateUserProviderException()
        {
            var guidProvider = new GuidUserProvider();

            var providers = new List<IUserProvider>()
            {
                guidProvider,
                guidProvider
            };

            Assert.Throws<DuplicateUserProviderException>(() => new StatelessIdentityProvider(providers));
        }

        [Test]
        public void TestDependencyInjection()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<IUserProvider, GuidUserProvider>()
                .AddTransient<IStatelessIdentityProvider, StatelessIdentityProvider>()
                .BuildServiceProvider();

            var sip = serviceProvider.GetService<IStatelessIdentityProvider>();
            Assert.NotNull(sip);
        }
    }
}
