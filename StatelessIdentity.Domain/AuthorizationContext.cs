using System;
using System.Collections.Generic;

namespace StatelessIdentity.Domain
{
    public class AuthorizationContext
    {
        public string ProviderId { get; set; }

        public IDictionary<string, string> Data { get; set; }

        public Guid ProviderIdAsGuid()
        {
            return Guid.Parse(ProviderId);
        }
    }
}
