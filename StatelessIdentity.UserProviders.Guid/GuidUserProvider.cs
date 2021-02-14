using StatelessIdentity.Domain;

namespace StatelessIdentity.UserProviders.Guid
{
    public class GuidUserProvider : IUserProvider
    {
        public System.Guid Id => System.Guid.Parse("1bbc3775-e5d1-4e85-aa34-a923fef63074");

        public User GetUser(AuthorizationContext authorizationContext)
        {
            return new User
            {
                ProviderId = Id,
                ExternalId = System.Guid.NewGuid().ToString(),
                Name = System.Guid.NewGuid().ToString(),
            };
        }
    }
}
