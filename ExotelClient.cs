using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Configuration;

namespace rbJustDial.Exotel
{

    public class ExotelClient
    {
        private static string EXOTEL_SID = ConfigurationManager.AppSettings["exotelSID"].ToString();
        private static string EXOTEL_TOKEN = ConfigurationManager.AppSettings["exotelToken"].ToString();
        private static string EXOTEL_CALLER_ID = ConfigurationManager.AppSettings["exotelCampaignNumber"].ToString();
        private static string EXOTEL_CALLER_FLOW_ID = "your_flow_id";
        private static string EXOTEL_CALLER_FLOW_URL = "http://my.exotel.in/exoml/start/" + EXOTEL_CALLER_FLOW_ID;
        private static string EXOTEL_CALL_FLOW_POST_URL = "https://" + EXOTEL_SID + ":" + EXOTEL_TOKEN + "@twilix.exotel.in/v1/Accounts/" + EXOTEL_SID + "/Calls";
        private static string EXOTEL_SEND_SMS_POST_URL = "https://" + EXOTEL_SID + ":" + EXOTEL_TOKEN + "@twilix.exotel.in/v1/Accounts/" + EXOTEL_SID + "/SMS/send";
        private static string EXOTEL_CALL_CONNECT_POST_URL = "https://" + EXOTEL_SID + ":" + EXOTEL_TOKEN + "@twilix.exotel.in/v1/Accounts/" + EXOTEL_SID + "/Calls/connect";
        static NetworkCredential exotel_cred = new NetworkCredential(EXOTEL_SID, EXOTEL_TOKEN);

        public static string ExotelSmsPostUrl
        {
            get { return EXOTEL_SEND_SMS_POST_URL; }
        }

        public static string ExotelSID
        {
            get { return EXOTEL_SID; }
        }

        public static string ExotelToken
        {
            get { return EXOTEL_TOKEN; }
        }

        public static string ExotelCallerId
        {
            get { return EXOTEL_CALLER_ID; }
        }


        /// <summary>
        /// This should be an overriden class, so we can swap in other phone providers later...
        /// need to switch over to different Exotel API that uses a flow...
        /// 
        /// </summary>
        /// <param name="fromPhone">the call centre's number, basically, the first number that the call should be placed at</param>
        /// <param name="toPhone">the customer's number, basically, the first number that the call should be placed at</param>
        /// <param name="callResponse">exotel call response</param>
        /// <returns></returns>
        public static ExotelCallResponse ConnectCall(string fromPhone, string toPhone, out ExotelCallResponse callResponse)
        {
            XmlDocument xmlResponse = new XmlDocument();
            callResponse = new ExotelCallResponse();
            
            BypassCertificateError();
            using (var client = new WebClient())
            {
                try
                {
                    string responseString;
                    bool callUsingFlow = false;

                    var data = new NameValueCollection();
                    client.UseDefaultCredentials = false;
                    client.Credentials = exotel_cred;
                    byte[] result;
                    
                    if (callUsingFlow)
                    {
                        data.Add("Caller", fromPhone);
                        data.Add("Called", toPhone);

                        data.Add("Url", EXOTEL_CALLER_FLOW_URL);
                        
                        //not in exotel API...
                        data.Add("callerid", EXOTEL_CALLER_ID);

                        result = client.UploadValues(EXOTEL_CALL_FLOW_POST_URL, data);
                        
                    }
                    else
                    {
                        data.Add("From", fromPhone);
                        data.Add("To", toPhone);
                        data.Add("callerid", EXOTEL_CALLER_ID);
                        data.Add("StatusCallback", "<your target url>");
                        
                        result = client.UploadValues(EXOTEL_CALL_CONNECT_POST_URL, data);
                    }
                    responseString = Convert.ToBase64String(result);
                     

                    using (var stream = new MemoryStream(result))
                    {
                        xmlResponse.Load(stream);
                    }

                    xmlResponse.LoadXml(xmlResponse.InnerXml);

                    XmlNodeList callSid = xmlResponse.GetElementsByTagName("Sid");
                    callResponse.CallSID = callSid[0].InnerText;

                    XmlNodeList PhoneNumberSid = xmlResponse.GetElementsByTagName("PhoneNumberSid");
                    callResponse.PhoneNumberSid = PhoneNumberSid[0].InnerText;

                    XmlNodeList Status = xmlResponse.GetElementsByTagName("Status");
                    callResponse.Status = Status[0].InnerText; 

                }
                catch (Exception exc)
                {                    
                    throw;
                }
            }

            return callResponse;
           
        }

        /// <summary>
        /// solution for exception
        /// System.Net.WebException: 
        /// The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel. ---> System.Security.Authentication.AuthenticationException: The remote certificate is invalid according to the validation procedure.
        /// </summary>
        public static void BypassCertificateError()
        {
            ServicePointManager.ServerCertificateValidationCallback +=

                delegate(
                    Object sender1,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
        }


    }
}
