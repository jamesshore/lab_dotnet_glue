using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Telerik.JustMock;
using System.Collections.Generic;
using System.Net;

namespace Auth0Glue.Test
{
    [TestClass]
    public class Auth0ClientTests
    {
        const string PasswordEndpoint = "/dbconnections/change_password";

        const string IrrelevantEndpoint = PasswordEndpoint;
        const string IrrelevantDomain = "irrelevant_domain";
        const string IrrelevantId = "irrelevant_id";
        const string IrrelevantApiToken = "irrelevant_api_token";
        const string IrrelevantConnection = "irrelevant_connection";
        const string IrrelevantEmail = "irrelevant_email";
        readonly Dictionary<string, string> IrrelevantHeaders = new Dictionary<string, string>()
        {
            { "Authorization", $"Bearer {IrrelevantApiToken}" }
        };
        readonly Dictionary<string, string> IrrelevantParameters = new Dictionary<string, string>()
        {
            { "client_id", IrrelevantId},
            { "email", IrrelevantEmail },
            { "connection", IrrelevantConnection }
        };
        readonly RestResponse IrrelevantResponse = new RestResponse()
        {
            Status = HttpStatusCode.NotImplemented
        };

        [TestMethod]
        public async Task PasswordReset_PostsRestRequest()
        {
            var domain = "auth0_domain";
            var id = "auth0_id";
            var token = "auth0_api_token";
            var connection = "auth0_connection";
            var email_address = "an_email_address";

            var expectedHeaders = new Dictionary<String, String>()
            {
                { "Authorization", $"Bearer {token}" }
            };
            var expectedParameters = new Dictionary<String, String>()
            {
                { "client_id", id },
                { "email", email_address },
                { "connection", connection }
            };
            var restResponse = new RestResponse()
            {
                Status = HttpStatusCode.OK
            };

            var restMock = Mock.Create<IRestClient>();
            Mock.Arrange(() => restMock.PostAsync(PasswordEndpoint, expectedHeaders, expectedParameters))
                .Returns(Task.FromResult<RestResponse>(restResponse))
                .OccursOnce();

            var client = NewClient(restMock, domain: domain, id: id, apiToken: token, connection: connection);
            await client.SendPasswordResetEmail(email_address);

            Mock.Assert(restMock);
        }

        [TestMethod]
        public async Task PasswordReset_ThrowsExceptionWhenStatusIsNotOkay()
        {
            var restResponse = new RestResponse()
            {
                Status = HttpStatusCode.ServiceUnavailable,
                Body = "Auth0 error message"
            };

            var restMock = Mock.Create<IRestClient>();
            Mock.Arrange(() => restMock.PostAsync(IrrelevantEndpoint, IrrelevantHeaders, IrrelevantParameters))
                .Returns(Task.FromResult<RestResponse>(restResponse));

            var client = NewClient(restMock);

            try
            {
                await client.SendPasswordResetEmail(IrrelevantEmail);
                Assert.Fail("expected exception");
            }
            catch (Auth0Exception e)
            {
                Assert.AreEqual("Unexpected status code from Auth0: 503 (ServiceUnavailable). Response body: 'Auth0 error message'", e.Message);
            }
        }

        private Auth0Client NewClient(IRestClient restMock, string domain = IrrelevantDomain, string id = IrrelevantId, string apiToken = IrrelevantApiToken, string connection = IrrelevantConnection) {
            return new Auth0Client(domain: domain, id: id, apiToken: apiToken, connection: connection, restClient: restMock);
        }
    }
}
