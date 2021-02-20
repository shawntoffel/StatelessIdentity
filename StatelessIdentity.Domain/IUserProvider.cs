using System.Threading.Tasks;

namespace StatelessIdentity.Domain
{
    public interface IUserProvider
    {
        /// <summary>
        /// A unique name assigned to this provider.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Exchanges the AuthorizationContext for a User.
        /// </summary>
        /// <param name="authorizationContext"></param>
        Task<User> GetUserAsync(AuthorizationContext authorizationContext);
    }
}
