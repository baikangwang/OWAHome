using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Orange.OWA.Interface;

namespace Orange.OWA.Model
{
    public class Account:IAccount
    {
        public Account(Guid id, string userName, string password, string host, string email)
        {
            Id = id;
            UserName = userName;
            Password = password;
            Host = host;
            Email = email;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Email { get; set; }
        public Guid Id { get; set; }
    }

    [XmlRoot]
    public class AccountCollection
    {
        [XmlElement]
        public Account[] Accounts { get; set; }

        public AccountCollection()
        {
            Accounts=new Account[0];
        }
    }
}
