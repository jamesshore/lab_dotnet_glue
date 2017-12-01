using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Auth0Glue.Test
{
    [TestClass]
    public class Auth0ClientTests
    {
        string Auth0Domain = "auth0_domain";
        string Auth0Id = "auth0_id";
        string Auth0Secret = "auth0_secret";
        string Auth0ApiToken = "auth0_api_token";
        string Auth0Connection = "auth0_connection";

        [TestMethod]
        public async Task SendsPasswordResetEmailAsync()
        {
            var email_address = "an_email_address";

            var client = new Auth0Client(Auth0Domain, Auth0Id, Auth0Secret, Auth0ApiToken, Auth0Connection);
            await client.SendPasswordResetEmail(email_address);
        }
    }
}
