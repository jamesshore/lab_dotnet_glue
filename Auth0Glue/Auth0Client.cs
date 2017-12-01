using System;
using System.Threading.Tasks;

namespace Auth0Glue
{
    public class Auth0Client
    {
        private object auth0Domain;
        private object auth0Id;
        private object auth0Secret;
        private object auth0ApiToken;
        private object auth0Connection;

        public Auth0Client(object auth0Domain, object auth0Id, object auth0Secret, object auth0ApiToken, object auth0Connection)
        {
            this.auth0Domain = auth0Domain;
            this.auth0Id = auth0Id;
            this.auth0Secret = auth0Secret;
            this.auth0ApiToken = auth0ApiToken;
            this.auth0Connection = auth0Connection;
        }

        public async Task SendPasswordResetEmail(string email)
        {

        }
    }
}