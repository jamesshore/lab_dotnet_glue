using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Auth0Glue
{
    public interface IRestClient
    {
        Task<RestResponse> PostAsync(string endpoint, Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null);
    }

    public class RestClient : IRestClient
    {
        // This is static because there should be as few HttpClient instances as possible (per HttpClient documentation).
        private static HttpClient client = new HttpClient();

        private string _host;

        public RestClient(string host)
        {
            this._host = host;
        }

        public async Task<RestResponse> PostAsync(string endpoint, Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null)
        {
            var json = parameters == null ? "" : JsonConvert.SerializeObject(parameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            headers = headers ?? new Dictionary<string, string>();
            foreach (string name in headers.Keys)
            {
                content.Headers.Add(name, headers[name]);
            }

            var httpResponse = await client.PostAsync($"{_host}{endpoint}", content);
            return new RestResponse()
            {
                Status = httpResponse.StatusCode,
                Body = await httpResponse.Content.ReadAsStringAsync()
            };
        }
    }

    public class RestResponse
    {
        public HttpStatusCode Status;
        public string Body;
    }
}
