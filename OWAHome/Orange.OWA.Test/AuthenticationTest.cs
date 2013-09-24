using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security.Authentication;
using System.Text;
using KlerksSoft;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orange.OWA.Gateway;
using Orange.OWA.Interface;

namespace Orange.OWA.Test
{
    [TestClass]
    public class AuthenticationTest
    {
        [TestInitialize]
        public void Init()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        [TestMethod]
        public void LogonTest()
        {
            IList<Cookie> cookies = Authentication.AuthenticationManager.Current.CookieCache;
            
            Assert.IsTrue(cookies!=null);
            
            Console.WriteLine("Count:{0}",cookies.Count);
            foreach (Cookie cookie in cookies)
            {
                Console.WriteLine("Index:{0}",cookies.IndexOf(cookie));
                Console.WriteLine("\tDomain:{0}", cookie.Domain);
                Console.WriteLine("\tName:{0}", cookie.Name);
                string value = cookie.Value;
                if (cookie.Name.ToLower() == "cadata")
                {
                    byte[] content = Convert.FromBase64String(value.Substring(1, value.Length - 2));
                    Encoding encoding = TextFileEncodingDetector.DetectTextByteArrayEncoding(content); //Encoding.GetEncoding("gzip");
                    if (encoding != null)
                    {
                        value = encoding.GetString(content);
                        Console.WriteLine("\tBase64 Value:{0}", cookie.Value);
                    }
                }
                Console.WriteLine("\tValue:{0}", value);
                Console.WriteLine("\tPort:{0}", string.IsNullOrEmpty(cookie.Port)?"N/A":cookie.Port);
                Console.WriteLine("\tPath:{0}", string.IsNullOrEmpty(cookie.Port) ? "N/A" : cookie.Path);
                Console.WriteLine("\tHttpOnly:{0}", cookie.HttpOnly);
                Console.WriteLine("\tComment:{0}", string.IsNullOrEmpty(cookie.Port) ? "N/A" : cookie.Comment);
            }

            Assert.IsTrue(cookies.Count!=0);
        }

        [TestMethod]
        public void RefreshTest()
        {
            IList<Cookie> cookies = new List<Cookie>();

            #region Test email host domain

            //IList<IAccount> accounts = AccountGateway.GetAll();

            //foreach (IAccount account in accounts)
            //{
            //    account.IsDefault = account.Host == "legacymail.taylorcorp.com";
            //}

            //accounts= AccountGateway.Update(accounts);

            #endregion

            #region Test wrong email host domain

            IAccount a = AccountGateway.GetDefault();
            a.Host = "test.test.com";
            a = AccountGateway.Update(a);

            #endregion

            Process.Start(AccountGateway.AccountSettingPath);

            try
            {
                Authentication.AuthenticationManager.Refresh();
                cookies = Authentication.AuthenticationManager.Current.CookieCache;
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(AuthenticationException));
                Console.WriteLine(e.Message);
            }

            Assert.IsTrue(cookies.Count < 2);

            #region Test email host domain

            //foreach (IAccount account in accounts)
            //{
            //    account.IsDefault = account.Host == "webmail.taylorcorp.com";
            //}
            //AccountGateway.Update(accounts);

            #endregion

            #region Test wrong email host domain

            a.Host = "webmail.taylorcorp.com";
            a = AccountGateway.Update(a);

            #endregion

            Process.Start(AccountGateway.AccountSettingPath);

            Authentication.AuthenticationManager.Refresh();

            cookies = Authentication.AuthenticationManager.Current.CookieCache;

            Console.WriteLine("Count:{0}", cookies.Count);

            Assert.IsTrue(cookies.Count >= 2);
        }

        [TestMethod]
        public void SingletonTest()
        {
            IList<Cookie> cookies1 = Authentication.AuthenticationManager.Current.CookieCache;
            IList<Cookie>  cookies2 = Authentication.AuthenticationManager.Current.CookieCache;
            Assert.AreEqual(cookies1,cookies2);
            IList<Cookie> cookies3 = Authentication.AuthenticationManager.Current.CookieCache;
            Assert.AreEqual(cookies2, cookies3);
        }

    }
}
