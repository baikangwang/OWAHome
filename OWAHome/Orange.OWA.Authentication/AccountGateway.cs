using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Orange.OWA.Interface;
using Orange.OWA.Model;

namespace Orange.OWA.Gateway
{
    public static class AccountGateway
    {
        public static string AccountSettingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Accounts.xml");
        
        public static void Init()
        {
                AccountCollection accounts = new AccountCollection()
                                                 {
                                                     Accounts = new Account[]
                                                                    {
                                                                        new Account(Guid.NewGuid(), "corp\bkwang", "R8ll#qqO2",
                                                                                    "webmail.taylorcorp.com",
                                                                                    "bkwang@nltechdev.com",true),
                                                                        new Account(Guid.NewGuid(),"corp\bkwang", "R8ll#qqO2",
                                                                                    "legacymail.taylorcorp.com",
                                                                                    "bkwang@nltechdev.com")
                                                                    }
                                                 };

            Save(accounts);

        }

        public static AccountCollection Read()
        {
            XmlSerializer xml=new XmlSerializer(typeof(AccountCollection));

            AccountCollection accounts;
            
            using (FileStream fs=new FileStream(AccountSettingPath,FileMode.Open))
            {
                accounts = xml.Deserialize(fs) as AccountCollection;
            }

            return accounts??new AccountCollection();
        }

        private static void Save(AccountCollection input)
        {
            XmlSerializer xml = new XmlSerializer(typeof(AccountCollection));
            using (XmlWriter xwr = new XmlTextWriter(AccountSettingPath, Encoding.UTF8))
            {
                xml.Serialize(xwr, input);
                xwr.Flush();
            }
        }

        public static IAccount Add(string userName, string password, string host, string email)
        {
            AccountCollection accounts = Read();

            IAccount account = new Account(Guid.NewGuid(), userName, password, host, email);
            
            accounts.Accounts = accounts.Accounts.Concat(new Account[] {account as Account}).ToArray();

            Save(accounts);

            return account;
        }

        public static IAccount Update(IAccount account)
        {
            AccountCollection accounts = Read();

            foreach (Account a in accounts.Accounts)
            {
                if (a.Id == (account as Account).Id)
                {
                    a.UserName = account.UserName;
                    a.Password = account.Password;
                    a.Host = account.Host;
                    a.Email = account.Email;
                    break;
                }
            }

            Save(accounts);

            return account;
        }
    }
}
