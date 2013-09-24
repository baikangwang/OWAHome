using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orange.OWA.Gateway;
using Orange.OWA.Interface;

namespace Orange.OWA.Test
{
    [TestClass]
    public class AccountGatewayTest
    {
        [TestMethod]
        public void GetAllTest()
        {
            IList<IAccount> accounts= AccountGateway.GetAll();

            Process.Start(AccountGateway.AccountSettingPath);
            
            foreach (IAccount a in accounts)
            {
                Console.WriteLine("Account("+accounts.IndexOf(a)+")");
                PrintAccount(a);
            }
        }

        [TestMethod]
        public void GetDefaultTest()
        {
            IAccount a = AccountGateway.GetDefault();

            Process.Start(AccountGateway.AccountSettingPath);

            Assert.IsTrue(a.IsDefault);

            Console.WriteLine("Account(" + "Default" + ")");
            PrintAccount(a);
        }

        [TestMethod]
        public void AddTest()
        {
            IAccount a = AccountGateway.Add("test1", "test1", "weibo.com", "baikangwang@hotmail.com");
            IAccount b = AccountGateway.Add("test2", "test2", "weibo.com", "baikangwang@hotmail.com", true);

            Process.Start(AccountGateway.AccountSettingPath);

            IList<IAccount> all = AccountGateway.GetAll();

            IAccount actual = null;
            
            foreach (IAccount account in all)
            {
                if (account.Id == a.Id || account.Id==b.Id)
                {
                    actual = account;
                }
                else
                {
                    actual = null;
                }

                if (actual != null)
                {
                    if(account.Id==a.Id)
                        Assert.IsFalse(actual.IsDefault);
                    else
                        Assert.IsTrue(actual.IsDefault);

                    Console.WriteLine("Found Account({0}) {1} default account", actual.Id == a.Id ? "a" : "b",
                                      actual.IsDefault ? "is" : "is not");

                    PrintAccount(account);
                }
            }

            IAccount actual2 = AccountGateway.GetDefault();
            Console.WriteLine("Default Account");
            PrintAccount(actual2);

            Assert.AreEqual(b.Id, actual2.Id);
        }

        [TestMethod]
        public void UpdateTest()
        {
            IAccount a = AccountGateway.GetAll().FirstOrDefault();

            Console.WriteLine("Old Values:");
            PrintAccount(a);

            a.Host = "www.test.com";
            a.UserName = "test3";
            a.IsDefault = true;
            a.Password = "test3";

            IAccount b = AccountGateway.Update(a);
            
            Console.WriteLine("New Values:");
            PrintAccount(b);

            Assert.AreEqual(a.Id,b.Id);
        }

        [TestMethod]
        public void UpdateCollectionTest()
        {
            IAccount a1= AccountGateway.Add("test1", "test1", "www.baidu.com", "email@email.com");
            IAccount a2 = AccountGateway.Add("test2", "test2", "www.google.com", "email@email.com", true);

            Process.Start(AccountGateway.AccountSettingPath);

            Thread.Sleep(1000);
            
            Console.WriteLine("Account(a1) Old Values:");
            PrintAccount(a1);

            Console.WriteLine("Account(a2) Old Values:");
            PrintAccount(a2);

            a1.UserName = "test11";
            a1.Host = "www.baidudu.com";

            a2.UserName = "test22";
            a2.Host = "www.googlegle.com";

            IList<IAccount> expect=new List<IAccount>(){a1,a2};

            IList<IAccount> actual = AccountGateway.Update(expect);

            Process.Start(AccountGateway.AccountSettingPath);

            a1 = actual.FirstOrDefault(a => a.Id == a1.Id);

            Assert.IsNotNull(a1);
            Console.WriteLine("Found Account(a1) New Values:");
            PrintAccount(a1);

            a2 = actual.FirstOrDefault(a => a.Id == a2.Id);

            Assert.IsNotNull(a2);
            Console.WriteLine("Found Account(a1) New Values:");
            PrintAccount(a2);



        }
        
        [TestMethod]
        public void GetTest()
        {
            IAccount a1 = AccountGateway.Add("test1", "test1", "www.host.com", "host@test.com");

            IAccount actual = AccountGateway.Get(a1.Id);
            
            PrintAccount(actual);

            Assert.AreEqual(a1.Id,actual.Id);

        }

        private void PrintAccount(IAccount a)
        {
            Console.WriteLine("\t{0,-10}:{1}", "Host", a.Host);
            Console.WriteLine("\t{0,-10}:{1}", "UserName", a.UserName);
            Console.WriteLine("\t{0,-10}:{1}", "Password", a.Password);
            Console.WriteLine("\t{0,-10}:{1}", "Email", a.Email);
            Console.WriteLine("\t{0,-10}:{1}", "IsDefault", a.IsDefault);
            Console.WriteLine("\t{0,-10}:{1}", "Id", a.Id);
        }
    }
}
