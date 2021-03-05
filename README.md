# StatelessIdentity

"Identity" without server-side state.

Uses 3rd party social identity providers to obtain user information such as username and avatar. User info is placed into a signed JWT (JWE available).

The token can be stored in a browser cookie and later validated and exchanged for it's contents. 

The JWT also contains a digest (SHA256 hash) of the identity provider name concatenated with the user's external id. Although the digest is not guaranteed to be unique it is often unique enough to be used as an identifier for non-critical use cases. 
