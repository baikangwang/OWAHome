using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Orange.OWA.HttpWeb
{
    public class Response:IDisposable
    {
        private HttpWebResponse _response;
        private CookieContainer _cookies;

        internal Response(HttpWebResponse response)
        {
            _response = response;

            _cookies = new CookieContainer();
            
            if (response.Cookies != null)
            {
                foreach (Cookie cookie in response.Cookies)
                {
                    _cookies.Add(cookie);
                }
            }
        }

        public CookieContainer Cookies { get { return _cookies; } }

        public Stream ResponseStream
        {
            get { return _response.GetResponseStream(); }
        }

        public string GetResponseHeader(string key)
        {
            return _response.GetResponseHeader(key);
        }

        public HttpStatusCode StatusCode
        {
            get { return _response.StatusCode; }
        }

        public void Close()
        {
            _response.Close();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool cleanupAll)
        {
            if (cleanupAll)
            {
                Close();
                Dispose(false);
            }
            else
            {
                _cookies = null;
            }
        }
    }
}
