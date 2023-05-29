//using hubtelapi_dotnet_v1.Hubtel;
using CM.Text;
using hubtelapi_dotnet_v1.Hubtel;
using Plivo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;


namespace EventManager1.Models
{
    public class CommonSMSCallF
    {
        public static CommonSMSRModal sendSMS(CommonSMSModal _SMSRQ)
        {
            CommonSMSRModal _response = new CommonSMSRModal();
            try
            {
                if (string.IsNullOrEmpty(_SMSRQ.PhoneSender) || _SMSRQ.PhoneSender.Equals("0"))
                {
                    _SMSRQ.PhoneSender = ConfigurationManager.AppSettings["PlivoSMS_SenderNo"].ToString(); 
                }
                String Authid = ConfigurationManager.AppSettings["PlivoSMS_AUTHID"].ToString();
                String AuthToken = ConfigurationManager.AppSettings["PlivoSMS_AUTHTOKENID"].ToString();
                var plivo = new PlivoApi(Authid, AuthToken);
                if (_SMSRQ.Countrycode == null)
                {
                    _SMSRQ.Countrycode = "+233";
                }
                var resp = plivo.Message.Create(
                                   src: _SMSRQ.PhoneSender,
                                    //dst: new List<String> { "+233" + _SMSRQ.PhoneReciever },
                                    dst: new List<String> { _SMSRQ.Countrycode + _SMSRQ.PhoneReciever },
                                    text: _SMSRQ.Text

                );

             
                string _status = resp.StatusCode.ToString();
                if (_status == "BadRequest")
                {
                    _response.Code = "102";
                    _response.Description = resp.Message.ToString();
                    if (resp.MessageUuid != null)
                    { _response.SMS_ID = resp.MessageUuid[0].ToString(); }

                }
                else
                {
                    _response.Code = "100";
                    _response.Description = resp.Message;
                    _response.SMS_ID = resp.MessageUuid[0];

                }
                //Console.Write(resp.Content);

                // // Print the message_uuid
                // Console.WriteLine(resp.Data.message_uuid[0]);

                // // Print the api_id
                // Console.WriteLine(resp.Data.api_id);

                //Console.ReadLine();
                return _response;
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
                _response.Code = "103";
                _response.Description = ex.Message;
                _response.SMS_ID ="";

            }

            return _response;
        }
        public static bool SendSMSnew(string msg, string receiver)
        {
            string URL = "https://ghbulksms.com/index.php?option=com_spc&comm=spc_api&username=adequateinfosoft&password=Adk1234&sender=roverman&recipient=" + receiver + "&message=" + msg;
            try
            {
                // Get HTML data   
                WebClient client = new WebClient();
                Stream data = client.OpenRead(URL);
                StreamReader reader = new StreamReader(data);
                string str = "";
                str = reader.ReadLine();
                while (str != null)
                {
                    Console.WriteLine(str);
                    str = reader.ReadLine();
                }
                data.Close();
            }
            catch (WebException exp)
            {

            }
            return true;
        }
        public static bool SendSMSHubtel(string msgs, string receiver)
        {
             string clientId = ConfigurationManager.AppSettings["hubtelclientId"]; 
             string clientSecret = ConfigurationManager.AppSettings["hubtelclientSecret"];
            try
            {
                var host = new ApiHost(new BasicAuth(clientId, clientSecret));
                var messageApi = new MessagingApi(host);
                MessageResponse msg = messageApi.SendQuickMessage("stream233", receiver, msgs, true);
                Console.WriteLine(msg.Status);
            }
            catch (Exception e)
            {
                Log4Net.Error("LoginException :" + e.Message);
                //if (e.GetType() == typeof(HttpRequestException))
                //{
                //    var ex = e as HttpRequestException;
                //    if (ex != null && ex.HttpResponse != null)
                //    {
                //        Console.WriteLine("Error Status Code " + ex.HttpResponse.Status);
                //    }
                //}

            }
            return true;
        }
        public static bool SendSMSCM(string msgs, string receiver)
        {
            var client = new TextClient(new Guid(ConfigurationManager.AppSettings["CM_PRODUCT_TOKEN"]));
            //var result = client.SendMessageAsync(msgs, "Ticketandinvite", new List<string> { receiver }, "Your_Reference").ConfigureAwait(false);

            string URL = "https://gw.cmtelecom.com/gateway.ashx?producttoken=7E34AEA5-D79F-4EFD-A198-A6A1AD0FC5F4&body=" + msgs + "&to=" + receiver + "&from=VIRTUALOTP&reference=cdfvcc";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();
            try
            {

            }
            catch (Exception ex)
            {

            }
            return false;
        }

        //public static bool SendHubtelSMS(string mobile, string message)
        //{
        //    string clientId = ConfigurationManager.AppSettings["hubtelSMSClientID"].ToString();
        //    string clientSecret = ConfigurationManager.AppSettings["HubtelSMSKey"].ToString();
        //    try
        //    {
        //        var host = new ApiHost(new BasicAuth(clientId, clientSecret));
        //        var messageApi = new MessagingApi(host);
        //        MessageResponse msg = messageApi.SendQuickMessage("Roverman", mobile, message, true);
        //        string status = msg.Status.ToString();
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        if (e.GetType() == typeof(HttpRequestException))
        //        {
        //            var ex = e as HttpRequestException;
        //            if (ex != null && ex.HttpResponse != null)
        //            {
        //                string status =  ex.HttpResponse.Status.ToString();
        //            }
        //        }

        //    }
        //    return false;
        //}
    }
}