using System;
using System.Security.Cryptography;
using System.Text;

namespace StatelessIdentity.Domain
{
    public class Identity
    {
        public Guid Id { get; set; }

        public Guid ProviderId { get; }

        public User User { get; set; }

        public Identity(Guid providerGuid, User user)
        {
            Id = Guid.NewGuid();
            ProviderId = providerGuid;
            User = user;
        }

        public string Token()
        {
            return "";
        }

        public string GetHash()
        {
            return  ComputeBase64Hash(ProviderId.ToString() + User.ExternalId);
        }

        private string ComputeBase64Hash(string input)
        {
            using var sha512Managed = new SHA512Managed();
            var bytes = Encoding.ASCII.GetBytes(input);
            var hash = sha512Managed.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static Identity Parse(string token)
        {
            return new Identity(Guid.NewGuid(), null);
        }
    }
}
