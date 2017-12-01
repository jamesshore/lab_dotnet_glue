﻿using System;
using System.Net.Http;
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

        public async Task PostAsync(string endpoint)
        {
            Console.WriteLine($"URL: '{host}{endpoint}'");
            string responseBody = await client.GetStringAsync($"{host}{endpoint}");
        }
    }
}
