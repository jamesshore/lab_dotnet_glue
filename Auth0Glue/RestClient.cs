using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Auth0Glue
{
    public class RestClient
    {
        // per HttpClient documentation, there should be as few instances of HttpClient as possible, so this is shared by all instances of RestClient.
        private static HttpClient client = new HttpClient();

        private string host;

        public RestClient(string host)
        {
            this.host = host;
        }

        public async Task PostAsync(string endpoint, Dictionary<string, string> parameters = null)
        {
            var json = JsonConvert.SerializeObject(parameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{host}{endpoint}", content);
        }
    }
}
