using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auth0Glue.Test
{
    public class MassPasswordReset
    {
        public static async Task runAsync(string jsonList1, string jsonList2, IAuth0Client auth0Client)
        {
            var emails = EmailCollator.CollateFromJson(jsonList1, jsonList2);
            foreach (var email in emails)
            {
                await auth0Client.SendPasswordResetEmail(email);
            }
        }
    }
}