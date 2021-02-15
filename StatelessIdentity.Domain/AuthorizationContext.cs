using System.Collections.Generic;

namespace StatelessIdentity.Domain
{
    public class AuthorizationContext
    {
        public string ProviderId { get; set; }

        public IDictionary<string, string> Data { get; set; }

        public AuthorizationContext()
        {
            Data = new Dictionary<string, string>();
        }

        public string GetData(string key)
        {
            if (Data == null)
                return null;

            return Data.TryGetValue(key, out string value) ? value : null;
        }
    }
}
