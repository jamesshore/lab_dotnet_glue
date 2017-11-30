using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Auth0Glue.Test
{
    [TestClass]
    public class RestClientTests
    {
        private static TestHarnessServer server;

        [ClassInitialize]
        public static void BeforeAll(TestContext context)
        {
            server = new TestHarnessServer();
            server.Start();
        }

        [ClassCleanup]
        public static void TearDown()
        {
            server.Stop();
        }

        [TestMethod]
        public async Task ShouldCallEndpointUsingPostMethod()
        {
            RestClient client = new RestClient(TestHarnessServer.Host);
            await client.PostAsync("/post-path");
        }
    }

    internal class TestHarnessServer
    {
        internal const string Host = "http://localhost:8080/";
        private HttpListener listener = new HttpListener();

        internal void Start()
        {
            listener.Start();
            listener.Prefixes.Add(Host);
            IAsyncResult result = listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
        }

        public static void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }

        internal void Stop()
        {
            listener.Stop();
        }
    }
}
