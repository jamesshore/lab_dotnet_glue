using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Auth0Glue.Test
{
    [TestClass]
    public class RestClientTests
    {
        private const string IrrelevantEndpoint = "/irrelevant_endpoint";
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
        public async Task CallsEndpointUsingPostMethod()
        {
            await newClient().PostAsync("/post-path");

            var request = await clientRequest;
            Assert.AreEqual("POST", request.Method, "method");
            Assert.AreEqual("/post-path", request.EndPoint, "endpoint");
        }

        [TestMethod]
        public async Task ProvidesEmptyBodyWhenThereAreNoParameters()
        {
            await newClient().PostAsync(IrrelevantEndpoint);
            Assert.AreEqual("", (await clientRequest).Body);
        }

        [TestMethod]
        public async Task ConvertsParametersToJsonObject()
        {
            var parameters = new Dictionary<string, string>()
            {
                ["parm1"] = "one",
                ["parm2"] = "two"
            };
            await newClient().PostAsync(IrrelevantEndpoint, parameters);

            TestHarnessRequest request = await clientRequest;
            Assert.AreEqual("{\"parm1\":\"one\",\"parm2\":\"two\"}", request.Body, "body");
            Assert.AreEqual("application/json; charset=utf-8", request.Headers["content-type"], "content-type header");
        }

        private RestClient newClient()
        {
            return new RestClient(TestHarnessServer.Host);
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
            var context = await listener.GetContextAsync();
            var request = context.Request;

            var body = new StreamReader(request.InputStream, request.ContentEncoding).ReadToEnd();

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
                EndPoint = request.RawUrl,
                Headers = request.Headers,
                Body = body
            };
        }
    }

    internal class TestHarnessRequest
    {
        public string Method { get; set; }
        public string EndPoint { get; set; }
        public NameValueCollection Headers { get; set; }
        public string Body { get; set; }
    }
}
