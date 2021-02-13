using StatelessIdentity.Domain;
using StatelessIdentity.Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace StatelessIdentity
{
    public class StatelessIdentity
    {
        private readonly Dictionary<Guid, IUserProvider> _providers;

        public StatelessIdentity()
        {
            _providers = new Dictionary<Guid, IUserProvider>();
        }

        public void RegisterUserProvidier(IUserProvider provider)
        {
            _providers.Add(provider.Id, provider);
        }

        public Identity Create(AuthorizationContext context)
        {
            var providerGuid = context.ProviderIdAsGuid();

            var found = _providers.TryGetValue(providerGuid, out IUserProvider provider);
            if (!found)
                throw new UnknownUserProviderException(context.ProviderId);

            var user = provider.GetUser(context);

            return new Identity(providerGuid, user);
        }

        public Identity FromToken(string token)
        {
            return new Identity(Guid.NewGuid(), null);
        }
    }
}
