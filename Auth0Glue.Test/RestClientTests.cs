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
            Task serverRequest = server.WaitForRequestAsync();
            log("Waiting to post");
            await client.PostAsync("/post-path");
            log("Post complete");
            log("Waiting for server");
            await serverRequest;
            log("Server complete");
            Assert.AreEqual(
                new TestHarnessRequest()
                {
                    Method = "GET",
                    EndPoint = "/post-path"
                }, 
                server.LastRequest
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
        private TestHarnessRequest lastRequest;
        private IAsyncResult lastAsyncGet;

        //public TestHarnessRequest LastRequest {
        //    get
        //    {
        //        log("Starting LastRequest; waiting");
        //        lastAsyncGet.AsyncWaitHandle.WaitOne();
        //        log("WaitOne() done");
        //        return lastRequest;
        //    }
        //}

        public TestHarnessRequest LastRequest
        {
            get
            {
                log("GETTING LastRequest");
                return lastRequest;
            }
        }

        internal void Start()
        {
            listener.Start();
            listener.Prefixes.Add($"{Host}/");
            //IAsyncResult result = listener.BeginGetContext(new AsyncCallback(ListenerCallback), this);
            //lastAsyncGet = result;
        }

        internal async Task WaitForRequestAsync()
        {
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;

            log("WaitForRequestAsync generating output");
            HttpListenerResponse response = context.Response;
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            log("WaitForRequestAsync setting lastRequest");
            lastRequest = new TestHarnessRequest()
            {
                Method = request.HttpMethod,
                EndPoint = request.RawUrl
            };

            log("SET COMPLETED for LastRequest");
        }

        public static void ListenerCallback(IAsyncResult result)
        {
            log("Starting ListenerCallback");
            TestHarnessServer server = (TestHarnessServer)result.AsyncState;
            HttpListenerContext context = server.listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;

            log("ListenerCallback generating output");
            HttpListenerResponse response = context.Response;
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            log("ListenerCallback setting lastRequest");
            server.lastRequest = new TestHarnessRequest()
            {
                Method = request.HttpMethod,
                EndPoint = request.RawUrl
            };

            log("SET COMPLETED for LastRequest");
        }

        internal static void log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToFileTime()}: {message}");
        }
        
        internal void Stop()
        {
            listener.Stop();
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
