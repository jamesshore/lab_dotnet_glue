using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Auth0Glue
{
    public class RestClient
    {
        private string host;

        public RestClient(string host)
        {
            this.host = host;
        }

        public async Task PostAsync(string endpoint)
        {
            HttpClient client = new HttpClient();
            string responseBody = await client.GetStringAsync($"{host}{endpoint}");

            Console.WriteLine(responseBody);
        }
    }
}
