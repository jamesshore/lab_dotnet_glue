using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Auth0Glue.Test
{
    [TestClass]
    public class RestClientTests
    {
        private static TestHarnessServer server;
        private Task<TestHarnessRequest> clientRequest;

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

        [TestInitialize]
        public void BeforeEach()
        {
            clientRequest = server.WaitForRequestAsync();
        }

        [TestMethod]
        public async Task ShouldCallEndpointUsingPostMethod()
        {
            RestClient client = new RestClient(TestHarnessServer.Host);
            await client.PostAsync("/post-path");
            Assert.AreEqual(
                new TestHarnessRequest()
                {
                    Method = "GET",
                    EndPoint = "/post-path"
                },
                await clientRequest
            );
        }

        internal static void log(string message)
        {
            TestHarnessServer.log(message);
        }
    }

    internal class TestHarnessServer
    {
        internal const string Host = "http://localhost:8080";

        private HttpListener listener = new HttpListener();

        internal void Start()
        {
            listener.Start();
            listener.Prefixes.Add($"{Host}/");
        }

        internal void Stop()
        {
            listener.Stop();
        }

        internal async Task<TestHarnessRequest> WaitForRequestAsync()
        {
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;

            HttpListenerResponse response = context.Response;
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            return new TestHarnessRequest()
            {
                Method = request.HttpMethod,
                EndPoint = request.RawUrl
            };
        }

        internal static void log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToFileTime()}: {message}");
        }
    }

    internal class TestHarnessRequest
    {
        public string EndPoint { get; set; }
        public string Method { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            TestHarnessRequest that = (TestHarnessRequest)obj;

            return this.EndPoint == that.EndPoint && this.Method == that.Method;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"TestHarnessRequest: Method '{Method}', EndPoint '{EndPoint}'";
        }
    }
}
