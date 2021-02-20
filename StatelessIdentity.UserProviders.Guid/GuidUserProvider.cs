using StatelessIdentity.Domain;
using System.Threading.Tasks;

namespace StatelessIdentity.UserProviders.Guid
{
    public class GuidUserProvider : IUserProvider
    {
        public string Name => nameof(GuidUserProvider);

        public Task<User> GetUserAsync(AuthorizationContext authorizationContext)
        {
            var user = new User(Name, System.Guid.NewGuid().ToString()) 
            { 
                Name = System.Guid.NewGuid().ToString()
            };

            return Task.FromResult(user);
        }
    }
}
