using System;

namespace StatelessIdentity.Domain
{
    public interface IUserProvider
    {
        /// <summary>
        /// A unique Guid assigned to this provider.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Exchanges the AuthorizationContext for a User.
        /// </summary>
        /// <param name="authorizationContext"></param>
        User GetUser(AuthorizationContext authorizationContext);
    }
}
