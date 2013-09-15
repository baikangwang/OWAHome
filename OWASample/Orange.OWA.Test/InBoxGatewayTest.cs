using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orange.OWA.Gateway;
using Orange.OWA.Interface;

namespace Orange.OWA.Test
{
    [TestClass]
    public class InBoxGatewayTest
    {
        [TestMethod]
        public void GetEmailSimpleListTest()
        {
            string actual = InBoxGateway.GetEmailSimpleList(0, 24);
            Console.WriteLine(actual);
            Assert.IsTrue(!string.IsNullOrEmpty(actual));
        }

        [TestMethod]
        public void GetEmailFullListTest()
        {
            string actual = InBoxGateway.GetEmailFullList(0, 1);
            Console.WriteLine(actual);
            Assert.IsTrue(!string.IsNullOrEmpty(actual));
        }

        [TestMethod]
        public void GetEmailTest()
        {
            //string url = "https://webmail.taylorcorp.com/exchange/bkwang@nltechdev.com/Inbox/test-2.EML";
            string id = "<42A26A4D7327004090E5E8144E9B080808E031AA@tcex04.corp.tcc.inet>";
            IEmail email = InBoxGateway.GetEmail(id);
            if(email==null)
                return;
            Console.WriteLine("id:{0}",email.Id);
            Console.WriteLine("url:{0}", email.Url);
            Console.WriteLine("from:{0}", email.From);
            Console.WriteLine("to:{0}",email.To);
            Console.WriteLine("cc:{0}", email.Cc);
            Console.WriteLine("subject:{0}", email.Subject);
            Console.WriteLine("daterecieved:{0}", email.DateRecieved);
            Console.WriteLine("submitted:{0}", email.Submitted);
            Console.WriteLine("hasattachment:{0}", email.HasAttachment);
            Console.WriteLine("priority:{0}", email.Priority);
            Console.WriteLine("read:{0}", email.Read);
            Console.WriteLine("textdescription:{0}", email.TextDescription);
            Console.WriteLine("htmldescription:{0}", email.HtmlDescription);
        }
    }
}
