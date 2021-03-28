using StatelessIdentity.Domain;
using StatelessIdentity.Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace StatelessIdentity
{
    public class StatelessIdentityProvider : IStatelessIdentityProvider
    {
        private readonly Dictionary<string, IUserProvider> _userProviders;

        public StatelessIdentityProvider()
        {
            _userProviders = new Dictionary<string, IUserProvider>();
        }

        public StatelessIdentityProvider(IEnumerable<IUserProvider> userProviders) : this()
        {
            if (userProviders == null)
                throw new ArgumentNullException(nameof(userProviders));

            foreach (var userProvider in userProviders)
                RegisterUserProvider(userProvider);
        }

        public void RegisterUserProvider(IUserProvider userProvider)
        {
            if (userProvider == null)
                throw new ArgumentNullException(nameof(userProvider));

            if (!_userProviders.TryAdd(userProvider.Name, userProvider))
                throw new DuplicateUserProviderException(userProvider.Name);
        }

        public Identity CreateIdentity(AuthorizationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrEmpty(context.Provider))
                throw new UnknownUserProviderException(context.Provider);

            var found = _userProviders.TryGetValue(context.Provider, out IUserProvider provider);
            if (!found)
                throw new UnknownUserProviderException(context.Provider);
            
            var task = provider.GetUserAsync(context);
            task.Wait();

            var user = task.Result;

            return new Identity(user);
        }
    }
}
