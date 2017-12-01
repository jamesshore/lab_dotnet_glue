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
            var endpoint = "/post-path";
            await NewClient().PostAsync(endpoint);

            var request = await clientRequest;
            Assert.AreEqual("POST", request.Method, "method");
            Assert.AreEqual(endpoint, request.EndPoint, "endpoint");
        }

        [TestMethod]
        public async Task SetsHeaders()
        {
            var headers = new Dictionary<string, string>()
            {
                { "header1", "one" },
                { "header2", "two" },

            };
            await NewClient().PostAsync(IrrelevantEndpoint, headers: headers);

            TestHarnessRequest request = await clientRequest;
            Assert.AreEqual("one", request.Headers["header1"]);
            Assert.AreEqual("two", request.Headers["header2"]);
        }

        [TestMethod]
        public async Task SendsParametersAsJsonObjectInBody()
        {
            var parameters = new Dictionary<string, string>()
            {
                { "parm1", "one" },
                { "parm2", "two" }
            };
            await NewClient().PostAsync(IrrelevantEndpoint, parameters: parameters);

            TestHarnessRequest request = await clientRequest;
            Assert.AreEqual("{\"parm1\":\"one\",\"parm2\":\"two\"}", request.Body, "body");
            Assert.AreEqual("application/json; charset=utf-8", request.Headers["content-type"], "content-type header");
        }

        [TestMethod]
        public async Task SendsEmptyBodyWhenThereAreNoParameters()
        {
            await NewClient().PostAsync(IrrelevantEndpoint);
            Assert.AreEqual("", (await clientRequest).Body);
        }

        [TestMethod]
        public async Task ReturnsServerResponse()
        {
            var expectedStatus = HttpStatusCode.UpgradeRequired;
            var expectedBody = "expected_body";
            server.ConfigureResponse(status: expectedStatus, body: expectedBody);

            var response = await NewClient().PostAsync(IrrelevantEndpoint);
            Assert.AreEqual(expectedStatus, response.Status, "status");
            Assert.AreEqual(expectedBody, response.Body, "body");
        }

        private RestClient NewClient()
        {
            return new RestClient(TestHarnessServer.Host);
        }
    }

    internal class TestHarnessServer
    {
        internal const string Host = "http://localhost:8080";
        internal const HttpStatusCode IrrelevantStatus = HttpStatusCode.NotImplemented;

        private HttpListener listener = new HttpListener();
        private HttpStatusCode responseStatus;
        private string responseBody;
        
        internal void Start()
        {
            listener.Start();
            listener.Prefixes.Add($"{Host}/");
        }

        internal void Stop()
        {
            listener.Stop();
        }

        internal void ConfigureResponse(HttpStatusCode status = IrrelevantStatus, string body = "irrelevant_body")
        {
            responseStatus = status;
            responseBody = body;
        }

        internal async Task<TestHarnessRequest> WaitForRequestAsync()
        {
            ConfigureResponse();
            var context = await listener.GetContextAsync();
            var request = context.Request;

            var body = new StreamReader(request.InputStream, request.ContentEncoding).ReadToEnd();

            HttpListenerResponse response = context.Response;
            response.StatusCode = (int)responseStatus;
            byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
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
