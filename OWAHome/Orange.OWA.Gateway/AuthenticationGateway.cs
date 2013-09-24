using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Orange.OWA.HttpWeb;
using Orange.OWA.Interface;

namespace Orange.OWA.Gateway
{
    public class AuthenticationGateway
    {
        public static IList<Cookie> Authenticate(IAccount account)
        {
            string owaUrl = string.Format("https://{0}/exchweb/bin/auth/owaauth.dll", account.Host);
            string desUrl = string.Format("https://{0}/exchange", account.Host);
            string query = string.Format("destination={0}&flags=0&forcedownlevel=0&trusted=0&username={1}&password={2}&SubmitCreds=Log On;", desUrl, account.UserName, account.Password);
            byte[] content = Encoding.UTF8.GetBytes(query);

            // Create the web OwaRequest:
            OwaRequest owaRequest = OwaRequest.Post(owaUrl, content);

            OwaResponse response = owaRequest.Send();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(string.Format("Message: Connection failed to url ({0}). Response status code: '{1}'.", desUrl, response.StatusCode));
            }

            response.Close();

            return response.Cookies;
        }
    }
}
