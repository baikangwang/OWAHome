﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Web;
using System.Xml;
using Orange.OWA.Authentication;
using Orange.OWA.HttpWeb;
using Orange.OWA.Interface;
using Orange.OWA.Model.Email;

namespace Orange.OWA.Gateway
{
    public class InBoxGateway
    {
        public static string InBoxUrl
        {
            get { return string.Format("https://{0}/exchange/{1}/InBox/", AuthenticationManager.Current.Host, AuthenticationManager.Current.EmailAddress); }
        }

        public static T GetEmailSimpleList<T>(int startIndex, int endIndex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<searchrequest xmlns=\"DAV:\">");
            sb.AppendLine("	<sql>SELECT \"urn:schemas:mailheader:message-id\" as id, \"urn:schemas:httpmail:from\" as from, \"urn:schemas:httpmail:datereceived\" as datereceived, \"urn:schemas:httpmail:cc\" as cc, \"urn:schemas:mailheader:subject\" as subject, \"urn:schemas:httpmail:to\" as to, \"urn:schemas:httpmail:read\" as read, \"urn:schemas:httpmail:thread-topic\" as topic, \"urn:schemas:httpmail:submitted\" as submitted, \"urn:schemas:httpmail:priority\" as priority");
            sb.AppendLine(string.Format("FROM Scope('SHALLOW TRAVERSAL OF \"{0}\"')", InBoxUrl));
            sb.AppendLine("WHERE \"DAV:isfolder\" = false");
            sb.AppendLine("ORDER BY \"urn:schemas:httpmail:datereceived\" DESC");
            sb.AppendLine("	</sql>");
            sb.AppendFormat("	<range type=\"row\">{0}-{1}</range>{2}",startIndex,endIndex,Environment.NewLine);
            sb.AppendLine("</searchrequest>");

            byte[] content = Encoding.UTF8.GetBytes(sb.ToString());

            string url = InBoxUrl;//string.Format("https://{0}/exchange/{1}/InBox/",AuthenticationManager.Current.Host,AuthenticationManager.Current.EmailAddress);

            OwaRequest request = OwaRequest.Search(url, content, AuthenticationManager.Current.CookieCache,
                                                   new Dictionary<string, string>() {{"depth", "1"}, {"Translate", "f"}});
            request.Accept = "*/*";
            request.ContentType = "text/xml";

            object result=null;

            Type t = typeof (T);

            if (t == typeof( string))
            {
                using (OwaResponse response = request.Send())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }
            else if(t==typeof(IList<IEmail>))
            {
                using (OwaResponse response = request.Send())
                {
                    result = Deserialize(response.GetResponseStream());
                }
            }

            return result == null ? default(T) : (T) Convert.ChangeType(result, t);
        }

        public static string GetEmailFullList(int startIndex, int endIndex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<searchrequest xmlns=\"DAV:\">");
            //sb.AppendLine("	<sql>SELECT \"http://schemas.microsoft.com/exchange/smallicon\" as smicon, \"http://schemas.microsoft.com/mapi/sent_representing_name\" as from, \"urn:schemas:httpmail:datereceived\" as recvd, \"http://schemas.microsoft.com/mapi/proptag/x10900003\" as flag, \"http://schemas.microsoft.com/mapi/subject\" as subj, \"http://schemas.microsoft.com/exchange/x-priority-long\" as prio, \"urn:schemas:httpmail:hasattachment\" as fattach,\"urn:schemas:httpmail:read\" as r, \"http://schemas.microsoft.com/exchange/outlookmessageclass\" as m, \"http://schemas.microsoft.com/mapi/proptag/x10950003\" as flagcolor");
            sb.AppendLine("    <sql>SELECT *");
            sb.AppendLine("FROM Scope('SHALLOW TRAVERSAL OF \"\"')");
            sb.AppendLine("WHERE \"http://schemas.microsoft.com/mapi/proptag/0x67aa000b\" = false AND \"DAV:isfolder\" = false AND \"urn:schemas:mailheader:message-id\" = '" + System.Security.SecurityElement.Escape("<42A26A4D7327004090E5E8144E9B080808E031AA@tcex04.corp.tcc.inet>") + "'");
            sb.AppendLine("ORDER BY \"urn:schemas:httpmail:datereceived\" DESC");
            sb.AppendLine("	</sql>");
            sb.AppendFormat("	<range type=\"row\">{0}-{1}</range>{2}", startIndex, endIndex, Environment.NewLine);
            sb.AppendLine("</searchrequest>");

            byte[] content = Encoding.UTF8.GetBytes(sb.ToString());

            string url = InBoxUrl;//string.Format("https://{0}/exchange/{1}/InBox/", AuthenticationManager.Current.Host, AuthenticationManager.Current.EmailAddress);

            OwaRequest request = OwaRequest.Search(url, content, AuthenticationManager.Current.CookieCache,
                                                   new Dictionary<string, string>() { { "depth", "1" }, { "Translate", "f" } });
            request.Accept = "*/*";
            request.ContentType = "text/xml";

            string result;
            using (OwaResponse response = request.Send())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }

        public static IEmail Deserialize(XmlNode content, XmlNamespaceManager xnsMgr)
        {
            IEmail email = new Email();
            email.Url = HttpUtility.UrlDecode(content.GetNodeValue<string>("a:href", xnsMgr));
            email.From = content.GetNodeValueByKey<string>("from", xnsMgr);
            email.Cc = content.GetNodeValueByKey<string>("cc", xnsMgr);
            email.To = content.GetNodeValueByKey<string>("to", xnsMgr);
            email.Topic = content.GetNodeValueByKey<string>("topic", xnsMgr);
            email.Subject = content.GetNodeValueByKey<string>("subject", xnsMgr);
            email.DateRecieved = content.GetNodeValueByKey<DateTime>("datereceived", xnsMgr);
            email.TextDescription = content.GetNodeValueByKey<string>("textdescription", xnsMgr);
            email.HtmlDescription = content.GetNodeValueByKey<string>("htmldescription", xnsMgr);
            email.HasAttachment = content.GetNodeValueByKey<bool>("hasAttachment", xnsMgr);
            email.Priority = content.GetNodeValueByKey<int>("priority", xnsMgr);
            email.Read = content.GetNodeValueByKey<bool>("read", xnsMgr);
            email.Submitted = content.GetNodeValueByKey<bool>("submitted", xnsMgr);
            email.Id = content.GetNodeValueByKey<string>("id", xnsMgr);
            return email;
        }

        public static IList<IEmail> Deserialize(string content)
        {

            byte[] buffer = Encoding.UTF8.GetBytes(content);

            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Deserialize(ms);
            }
        }

        public static IList<IEmail> Deserialize(Stream content)
        {
            IList<IEmail> emails = new List<IEmail>();
            
            XmlDocument doc = new XmlDocument();
            doc.Load(content);

            XmlNamespaceManager xnsMgr = new XmlNamespaceManager(doc.NameTable);
            xnsMgr.AddNamespace("b", "urn:uuid:c2f41010-65b3-11d1-a29f-00aa00c14882/");
            xnsMgr.AddNamespace("c", "xml:");
            xnsMgr.AddNamespace("a", "DAV:");

            XmlNodeList nodes = doc.SelectNodes("/a:multistatus/a:response",xnsMgr);
            if (nodes == null) return emails;
            foreach (XmlNode node in nodes)
            {
                if (node != null)
                    emails.Add(Deserialize(node,xnsMgr));
            }

            return emails;
        }

        public static T GetEmail<T>(string id)
        {
            StringBuilder sb = new StringBuilder();

            //string inBoxUrl = string.Format("https://{0}/exchange/{1}/InBox/", AuthenticationManager.Current.Host, AuthenticationManager.Current.EmailAddress);
            
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<searchrequest xmlns=\"DAV:\">");
            sb.AppendLine("	<sql>SELECT \"urn:schemas:mailheader:message-id\" as id, \"urn:schemas:httpmail:from\" as from, \"urn:schemas:httpmail:datereceived\" as datereceived, \"urn:schemas:httpmail:cc\" as cc, \"urn:schemas:mailheader:subject\" as subject, \"urn:schemas:httpmail:to\" as to,\"urn:schemas:httpmail:htmldescription\" as htmldescription,\"urn:schemas:httpmail:textdescription\" as textdescription, \"urn:schemas:httpmail:hasattachment\" as hasattachment,\"urn:schemas:httpmail:read\" as read, \"urn:schemas:httpmail:thread-topic\" as topic, \"urn:schemas:httpmail:submitted\" as submitted, \"urn:schemas:httpmail:priority\" as priority");
            sb.AppendLine(string.Format("FROM Scope('SHALLOW TRAVERSAL OF \"{0}\"')", InBoxUrl));
            sb.AppendLine("WHERE \"urn:schemas:mailheader:message-id\" = '" + System.Security.SecurityElement.Escape(id) + "' AND \"DAV:isfolder\" = false");
            sb.AppendLine("	</sql>");
            sb.AppendLine("</searchrequest>");

            string query = sb.ToString();

            byte[] content = Encoding.UTF8.GetBytes(query);

            string url = InBoxUrl;

            OwaRequest request = OwaRequest.Search(url, content, AuthenticationManager.Current.CookieCache,
                                                   new Dictionary<string, string>() { { "depth", "1" }, { "Translate", "f" } });
            request.Accept = "*/*";
            request.ContentType = "text/xml";

            object result = null;
            Type t = typeof (T);

            if (t == typeof(string))
            {
                using (OwaResponse response = request.Send())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }
            else if (t == typeof(IEmail))
            {
                IList<IEmail> emails;
                using (OwaResponse response = request.Send())
                {
                    emails = Deserialize(response.GetResponseStream());
                }
                result=emails.SingleOrDefault();
            }

            return result == null ? default(T) : (T)Convert.ChangeType(result, t);
        }

        public static string Serialize(IEmail email)
        {
            return string.Empty;
        }
    }
}
