namespace StatelessIdentity.Domain
{
    public interface IStatelessIdentityProvider
    {
        /// <summary>
        /// Registers the IUserProvider as an available provider.
        /// </summary>
        /// <param name="provider"></param>
        void RegisterUserProvider(IUserProvider provider);

        /// <summary>
        /// Creates an Identity using the provided AuthorizationContext.
        /// </summary>
        /// <param name="context"></param>
        Identity CreateIdentity(AuthorizationContext context);
    }
}
