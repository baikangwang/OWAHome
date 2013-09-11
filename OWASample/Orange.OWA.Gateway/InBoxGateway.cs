using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Orange.OWA.Authentication;
using Orange.OWA.HttpWeb;
using Orange.OWA.Interface;
using Orange.OWA.Model.Email;

namespace Orange.OWA.Gateway
{
    public class InBoxGateway
    {
        public static string GetEmailSimpleList(int startIndex, int endIndex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<searchrequest xmlns=\"DAV:\">");
            sb.AppendLine("	<sql>SELECT \"http://schemas.microsoft.com/exchange/smallicon\" as smicon, \"http://schemas.microsoft.com/mapi/sent_representing_name\" as from, \"urn:schemas:httpmail:datereceived\" as recvd, \"http://schemas.microsoft.com/mapi/proptag/x10900003\" as flag, \"http://schemas.microsoft.com/mapi/subject\" as subj, \"http://schemas.microsoft.com/exchange/x-priority-long\" as prio, \"urn:schemas:httpmail:hasattachment\" as fattach,\"urn:schemas:httpmail:read\" as r, \"http://schemas.microsoft.com/exchange/outlookmessageclass\" as m, \"http://schemas.microsoft.com/mapi/proptag/x10950003\" as flagcolor");
            sb.AppendLine("FROM Scope('SHALLOW TRAVERSAL OF \"\"')");
            sb.AppendLine("WHERE \"http://schemas.microsoft.com/mapi/proptag/0x67aa000b\" = false AND \"DAV:isfolder\" = false");
            sb.AppendLine("ORDER BY \"urn:schemas:httpmail:datereceived\" DESC");
            sb.AppendLine("	</sql>");
            sb.AppendFormat("	<range type=\"row\">{0}-{1}</range>{2}",startIndex,endIndex,Environment.NewLine);
            sb.AppendLine("</searchrequest>");

            byte[] content = Encoding.UTF8.GetBytes(sb.ToString());

            string url = string.Format("https://{0}/exchange/{1}/InBox/",AuthenticationManager.Current.Host,AuthenticationManager.Current.EmailAddress);

            OwaRequest request = OwaRequest.Search(url, content, AuthenticationManager.Current.CookieCache,
                                                   new Dictionary<string, string>() {{"depth", "1"}, {"Translate", "f"}});
            request.Accept = "*/*";
            request.ContentType = "text/xml";

            string result;
            using (OwaResponse response = request.Send())
            {
                using (StreamReader sr=new StreamReader(response.GetResponseStream(),Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }

        public static string GetEmailFullList(int startIndex, int endIndex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<searchrequest xmlns=\"DAV:\">");
            //sb.AppendLine("	<sql>SELECT \"http://schemas.microsoft.com/exchange/smallicon\" as smicon, \"http://schemas.microsoft.com/mapi/sent_representing_name\" as from, \"urn:schemas:httpmail:datereceived\" as recvd, \"http://schemas.microsoft.com/mapi/proptag/x10900003\" as flag, \"http://schemas.microsoft.com/mapi/subject\" as subj, \"http://schemas.microsoft.com/exchange/x-priority-long\" as prio, \"urn:schemas:httpmail:hasattachment\" as fattach,\"urn:schemas:httpmail:read\" as r, \"http://schemas.microsoft.com/exchange/outlookmessageclass\" as m, \"http://schemas.microsoft.com/mapi/proptag/x10950003\" as flagcolor");
            sb.AppendLine("    <sql>SELECT *");
            sb.AppendLine("FROM Scope('SHALLOW TRAVERSAL OF \"\"')");
            sb.AppendLine("WHERE \"http://schemas.microsoft.com/mapi/proptag/0x67aa000b\" = false AND \"DAV:isfolder\" = false");
            sb.AppendLine("ORDER BY \"urn:schemas:httpmail:datereceived\" DESC");
            sb.AppendLine("	</sql>");
            sb.AppendFormat("	<range type=\"row\">{0}-{1}</range>{2}", startIndex, endIndex, Environment.NewLine);
            sb.AppendLine("</searchrequest>");

            byte[] content = Encoding.UTF8.GetBytes(sb.ToString());

            string url = string.Format("https://{0}/exchange/{1}/InBox/", AuthenticationManager.Current.Host, AuthenticationManager.Current.EmailAddress);

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

        public static IEmail Deserialize(string content)
        {
            //XmlDocument doc = new XmlDocument();
            //doc.Load(shipAckFile);
            
            //XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            //nsmgr.AddNamespace("ack", "http://www.icsm.com/icsmxml");
            //XmlNode orderReferenceNode = doc.SelectSingleNode("/ack:ICSMXML/ack:Request/ack:ShipNoticeRequest/ack:ShipNoticePortion/ack:OrderReference", nsmgr);

            return new Email();
        }

        public static string Serialize(IEmail email)
        {
            return string.Empty;
        }
    }
}
