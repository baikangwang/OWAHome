using System;
using System.Net;
using System.Text;
using Orange.OWA.HttpWeb;

namespace Orange.OWA.Authentication
{
    public class AuthenticationManager
    {
        private CookieContainer _cookieCache;
        private static AuthenticationManager _mgr=new AuthenticationManager();

        public CookieContainer CookieCache { get { return _cookieCache; } }
        public static AuthenticationManager Current { get { return _mgr; } }
        
        protected AuthenticationManager()
        {
            _cookieCache=new CookieContainer();
            string host = "webmail.taylorcorp.com";
            string userName = "corp\bkwang";
            string password = "R8ll#qqO2";
            _cookieCache = Authenticate(host, userName, password);
        }

        protected CookieContainer Authenticate(string host,string userName,string password)
        {
            string owaUrl = string.Format("https://{0}/exchweb/bin/auth/owaauth.dll", host);
            string desUrl = string.Format("https://{0}/exchange",host);
            string query = string.Format("destination={0}&flags=0&forcedownlevel=0&trusted=0&username={1}&password={2}&SubmitCreds=Log On;", desUrl, userName, password);
            byte[] content = Encoding.UTF8.GetBytes(query);
            
            // Create the web request:
            Request request = Request.Post(owaUrl, null, null, content);

            Response response = request.Send();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(string.Format("Message: Connection failed to url ({0}). Response status code: '{1}'.", desUrl, response.StatusCode));
            }

            response.Close();

            return response.Cookies;
        }


    }
}
