using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Security.Authentication;
using System.Text;
using Orange.OWA.Core;
using Orange.OWA.Gateway;
using Orange.OWA.Interface;

namespace Orange.OWA.Authentication
{
    public class AuthenticationManager:IAuthenticationManager
    {
        private IList<Cookie> _cookieCache = null;
        private readonly IAccount _currentAccount;
        private static AuthenticationManager _mgr;//= new AuthenticationManager();

        public IList<Cookie> CookieCache
        {
            get { return _cookieCache ?? (_cookieCache = Authenticate(_currentAccount)); }
        }

        public IAccount Account
        {
            get { return _currentAccount; }
        }

        public static AuthenticationManager Current
        {
            get { return _mgr ?? (_mgr = new AuthenticationManager()); }
        }

        public static void Refresh()
        {
            _mgr = new AuthenticationManager();
        }

        protected AuthenticationManager()
        {
            Log.Debug("Authentication Manager initialized.");
            _cookieCache=new List<Cookie>();
            _currentAccount = AccountGateway.GetDefault();
            if(_currentAccount==null)
                throw new AuthenticationException("Message: No default account found in Account Setting.");

            try
            {
                _cookieCache = Authenticate(_currentAccount);
            }
            catch (AuthenticationException e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Authentication Exception, are you using a valid login?");
                sb.AppendLine("   Msg: " + e.Message);
                sb.AppendLine("   Note: You must use a valid login / password for authentication.");
                string message = sb.ToString();
                Log.Error(message);
                throw new AuthenticationException(message);
            }
            catch (SecurityException e)
            {
                StringBuilder sb=new StringBuilder();
                sb.AppendLine("Security Exception");
                sb.AppendLine("   Msg: " + e.Message);
                sb.AppendLine("   Note: The application may not be trusted if run from a network share.");
                string message = sb.ToString();
                Log.Error(message);
                throw new AuthenticationException(message);
            }
            catch (WebException e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Web Exception");
                sb.AppendLine("   Status: " + e.Status);
                sb.AppendLine("   Reponse: " + e.Response);
                sb.AppendLine("   Msg: " + e.Message);
                string message = sb.ToString();
                Log.Error(message);
                throw new AuthenticationException(message);
            }
            catch (Exception ex)
            {
                if (ex is AuthenticationException)
                    throw;
                
                Log.ErrorFormat("Message: Authentication failed to email host ({0}). {1}", _currentAccount.Host,ex.Message);
                throw new AuthenticationException(string.Format("Authentication failed to email host ({0})",
                                                                _currentAccount.Host));
            }

            if (_cookieCache.Count < 2)
            {
                string message = "Login failed. Is the login / password correct?";
                Log.Error(message);
                throw new AuthenticationException(message);
            }
        }

        protected IList<Cookie> Authenticate(IAccount account)
        {
            IList<Cookie> cookies = AuthenticationGateway.Authenticate(account);

            return cookies;
        }
    }
}
