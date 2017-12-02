using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auth0Glue.Test
{
    public class MassPasswordReset
    {
        public static async Task runAsync(List<string> list1, List<string> list2, IAuth0Client auth0Client)
        {
            var emails = EmailCollator.Collate(list1, list2);
            foreach (var email in emails)
            {
                await auth0Client.SendPasswordResetEmail(email);
            }
        }
    }
}