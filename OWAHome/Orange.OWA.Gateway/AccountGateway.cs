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
    public class AccountGateway
    {
        static AccountGateway()
        {
            if (!File.Exists(AccountSettingPath))
            {
                Init();
            }
        }
        
        public static string AccountSettingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Accounts.xml");
        
        private static void Init()
        {
            //_host = "legacymail.taylorcorp.com";//"webmail.taylorcorp.com";
            //_userName = "corp\\bkwang";
            //_password = "R8ll#qqO2";
            //_emailAddress = "bkwang@nltechdev.com";

            IList<Account> accounts = new List<Account>()
                {
                    new Account(Guid.NewGuid(), "corp\\bkwang", "R8ll#qqO2",
                                "webmail.taylorcorp.com",
                                "bkwang@nltechdev.com", true),
                    new Account(Guid.NewGuid(), "corp\\bkwang", "R8ll#qqO2",
                                "legacymail.taylorcorp.com",
                                "bkwang@nltechdev.com")
                };

            Save(accounts);
        }

        private static IList<Account> Read()
        {
            XmlSerializer xml = new XmlSerializer(typeof(List<Account>));

            IList<Account> accounts;

            using (FileStream fs = new FileStream(AccountSettingPath, FileMode.Open))
            {
                accounts = xml.Deserialize(fs) as IList<Account>;
            }

            return accounts ?? new List<Account>();
        }

        private static void Save(IList<Account> input)
        {
            XmlSerializer xml = new XmlSerializer(typeof(List<Account>));
            using (XmlWriter xwr = new XmlTextWriter(AccountSettingPath, Encoding.UTF8))
            {
                xml.Serialize(xwr, input);
                xwr.Flush();
            }
        }


        public static IAccount Add(string userName, string password, string host, string email,bool isDefault=false)
        {
            IList<Account> accounts = Read();
            if (isDefault)
            {
                foreach (Account a in accounts)
                {
                    a.IsDefault = false;
                }
            }
            Guid id = Guid.NewGuid();
            Account account = new Account(id, userName, password, host, email,isDefault);

            accounts.Add(account);

            Save(accounts);

            return Get(id);
        }

        public static IAccount Update(IAccount account)
        {
            IList<Account> accounts = Read();

            IAccount target = null;
            IAccount orgDefault = null;

            //Make sure default account is unique
            foreach (Account a in accounts)
            {
                //remember original default account
                if (a.IsDefault)
                    orgDefault = a;
                
                //reset all account
                if (account.IsDefault)
                    a.IsDefault = false;

                if (a.Id == account.Id)
                {
                    target = a;
                }
            }

            if (target != null)
            {
                target.UserName = account.UserName;
                target.Password = account.Password;
                target.Host = account.Host;
                target.Email = account.Email;
                target.IsDefault = account.IsDefault;
            }
            else
            {
                //orgDefault is always existing.
                //When no account to be updated, rollback default accout setting
                if (orgDefault != null)
                    orgDefault.IsDefault = true;
            }

            Save(accounts);

            return Get(account.Id);
        }

        public static IList<IAccount> Update(IList<IAccount> accounts)
        {
            return accounts.Select(account => Update(account)).ToList();
        }

        public static IAccount GetDefault()
        {
            return Read().SingleOrDefault(a => a.IsDefault);
        }

        public static IList<IAccount> GetAll()
        {
            return Read().Cast<IAccount>().ToList();
        }

        public static IAccount Get(Guid id)
        {
            return Read().SingleOrDefault(a => a.Id == id);
        }
    }
}
