using StatelessIdentity.Domain;
using StatelessIdentity.Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace StatelessIdentity
{
    public class StatelessIdentityProvider : IStatelessIdentityProvider
    {
        private readonly Dictionary<string, IUserProvider> _providers;

        public StatelessIdentityProvider()
        {
            _providers = new Dictionary<string, IUserProvider>();
        }

        public void RegisterUserProvider(IUserProvider provider)
        {
            _providers.Add(provider.Name, provider);
        }

        public Identity CreateIdentity(AuthorizationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrEmpty(context.Provider))
                throw new UnknownUserProviderException(context.Provider);

            var found = _providers.TryGetValue(context.Provider, out IUserProvider provider);
            if (!found)
                throw new UnknownUserProviderException(context.Provider);

            var task = provider.GetUserAsync(context);
            task.Wait();

            var user = task.Result;

            return new Identity(user);
        }
    }
}
