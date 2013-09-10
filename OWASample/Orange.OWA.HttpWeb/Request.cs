using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Orange.OWA.HttpWeb
{
    public class Request
    {
        private readonly HttpWebRequest _request;
        private readonly byte[] _content;

        protected Request(string method, string url, params object[] args)
        {
            _request = (HttpWebRequest)System.Net.WebRequest.Create(url);
            _request.Method = method;
            _request.Accept = "text/html, application/xhtml+xml, */*";
            _request.Headers.Add("Accept-Language", "en-US");
            _request.Headers.Add("Accept-Encoding", "gzip,deflate");
            _request.UserAgent = "Orange.OWA";
            _request.ContentType = "application/x-www-form-urlencoded";
            _request.KeepAlive = true;
            _request.Headers.Add("Cache-Control", "no-cache");

            if (args == null)
                return;

            if (args.Length > 0)
            {
                IDictionary<string, string> headers = args[0] as IDictionary<string, string>;
                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        _request.Headers.Add(key, headers[key]);
                    }
                }
            }

            if (args.Length > 1)
            {
                CookieContainer cookies = args[1] as CookieContainer;
                _request.CookieContainer = cookies ?? new CookieContainer();
            }

            if (args.Length > 2)
            {
                byte[] content = args[2] as byte[];
                if (content != null)
                {
                    _request.ContentLength = content.Length;
                    this._content = content;
                }
            }
        }

        public Response Send()
        {
            if (_content != null)
            {
                using (Stream s = _request.GetRequestStream())
                {
                    s.Write(_content, 0, _content.Length);
                    s.Flush();
                }
            }

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)_request.GetResponse();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Message: request failed to url ({0}). {1}", _request.RequestUri, ex.Message));
            }

            Response owaResponse=new Response(response);

            return owaResponse;
        }

        public static Request Get(string url)
        {
            return new Request("Get",url);
        }

        public static Request Post(string url, IDictionary<string, string> headers, CookieContainer cookies, byte[] content)
        {
            return new Request("POST",url,headers,cookies,content);
        }

        public static Request Search(string url, IDictionary<string, string> headers, CookieContainer cookies, byte[] content)
        {
            return new Request("SEARCH",url,headers,cookies,content);
        }
    }
}
