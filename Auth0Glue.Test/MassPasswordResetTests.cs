using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Telerik.JustMock;
using System.Threading.Tasks;

namespace Auth0Glue.Test
{
    [TestClass]
    public class MassPasswordResetTests
    {
        [TestMethod]
        public async Task SendsPasswordResetEmailsToCollatedListsOfPeople()
        {
            var list1 = new List<string>()
            {
                "email1",
                "email2"
            };
            var list2 = new List<string>()
            {
                "email1",
            };

            var auth0Mock = Mock.Create<IAuth0Client>();
            Mock.Arrange(() => auth0Mock.SendPasswordResetEmail("email1")).Returns(Task.CompletedTask).OccursOnce();
            Mock.Arrange(() => auth0Mock.SendPasswordResetEmail("email2")).Returns(Task.CompletedTask).OccursOnce();

            await MassPasswordReset.runAsync(list1, list2, auth0Mock);
            Mock.Assert(auth0Mock);
        }
    }
}
