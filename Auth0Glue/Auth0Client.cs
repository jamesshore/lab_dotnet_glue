using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Auth0Glue
{
    public interface IAuth0Client
    {
        Task SendPasswordResetEmail(string emailAddress);
    }

    public class Auth0Client : IAuth0Client
    {
        private string _domain;
        private string _id;
        private string _apiToken;
        private string _connection;
        private IRestClient _restClient;

        public Auth0Client(string domain, string id, string apiToken, string connection, IRestClient restClient = null)
        {
            _domain = domain;
            _id = id;
            _apiToken = apiToken;
            _connection = connection;
            _restClient = restClient ?? new RestClient(_domain);
        }

        public async Task SendPasswordResetEmail(string emailAddress)
        {
            var headers = new Dictionary<String, String>()
            {
                { "Authorization", $"Bearer {_apiToken}" }
            };
            var parameters = new Dictionary<String, String>()
            {
                { "client_id", _id },
                { "email", emailAddress },
                { "connection", _connection }
            };
            var response = await _restClient.PostAsync("/dbconnections/change_password", headers, parameters);
            if (response.Status != HttpStatusCode.OK)
            {
                throw new Auth0Exception($"Unexpected status code from Auth0: {(int)response.Status} ({response.Status}). Response body: '{response.Body}'");
            }
        }
    }

    [Serializable]
    public class Auth0Exception : Exception
    {
        public Auth0Exception() { }
        public Auth0Exception(string message) : base(message) { }
        public Auth0Exception(string message, Exception inner) : base(message, inner) { }
        protected Auth0Exception(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}