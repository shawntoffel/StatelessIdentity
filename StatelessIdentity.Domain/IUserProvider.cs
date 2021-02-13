using System;

namespace StatelessIdentity.Domain
{
    public interface IUserProvider
    {
        Guid Id { get; }
        User GetUser(AuthorizationContext authorizationContext);
    }
}
