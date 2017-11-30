using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Auth0Glue;
using System.Threading.Tasks;

namespace Auth0Glue.Test
{
    [TestClass]
    public class RestClientTests
    {
        [TestMethod]
        public async Task ShouldCallEndpointUsingPostMethod()
        {
            RestClient client = new RestClient();
            await client.PostAsync("/post-path");
        }
    }
}
