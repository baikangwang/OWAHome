using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        }

        protected CookieContainer Authenticate()
        {
            return new CookieContainer();
        }


    }
}
