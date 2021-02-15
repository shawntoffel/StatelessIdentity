using StatelessIdentity.Domain;
using StatelessIdentity.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StatelessIdentity
{
    public class StatelessIdentityProvider : IStatelessIdentityProvider
    {
        private readonly Dictionary<Guid, IUserProvider> _providers;

        public StatelessIdentityProvider()
        {
            _providers = new Dictionary<Guid, IUserProvider>();
        }

        public void RegisterUserProvider(IUserProvider provider)
        {
            _providers.Add(provider.Id, provider);
        }

        public Identity CreateIdentity(AuthorizationContext context)
        {
            var providerGuid = Guid.Parse(context.ProviderId);

            var found = _providers.TryGetValue(providerGuid, out IUserProvider provider);
            if (!found)
                throw new UnknownUserProviderException(context.ProviderId);

            var task = provider.GetUserAsync(context);
            task.Wait();

            var user = task.Result;

            return new Identity(user);
        }
    }
}
