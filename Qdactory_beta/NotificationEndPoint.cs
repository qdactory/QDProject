using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Qdactory_beta
{
    public class NotificationEndPoint
    {
        DoctorEndPoint docApi = new DoctorEndPoint();
        MainApp mainApi = new MainApp();
        
        //const string connetionString = "Data Source=209.124.89.43;Initial Catalog=qayimdac_qd;User ID=qayimdac_qdactory; Password=7hyp^X11";
        const string connetionString = "Data Source=209.124.89.43;Initial Catalog=fanarpsk_sandbox;User ID=devTeam; Password=qG9mr#82";
        IDictionary<string, string> SendGridTempaltes = new Dictionary<string, string>();

        public NotificationEndPoint()
        {
            SetUpSendGridTemplates(); 
        }
        public void SetUpSendGridTemplates()
        {
            SendGridTempaltes.Add("new_user_verification", "d-7d063570541c4fb0b12d7202f781dc9b");
            SendGridTempaltes.Add("user_like_comment", "d-922cc17c79c34842b9615cb537fe02cc");
            SendGridTempaltes.Add("user_add_comment", "d-31ae3e672fd74e14a77d404c58f998c6");
        }

        public void NotifyDoctor(string SEND_GRID_TITLE,object DATA, int DOC_ID)
        {
            /* SEND NOTIFICATION TO DOCTOR */
            var email = docApi.GetDocEmail(DOC_ID);
            email = "numay.alghalib@gmail.com";
            Notification_send(email, "notify", SendGridTempaltes[SEND_GRID_TITLE], DATA);
        }
        public void NotifyUser(string SEND_GRID_TITLE,object DATA, int USER_ID,string email)
        {
            /* SEND NOTIFICATION TO USER */
            if (USER_ID > -1)
            {
                email = mainApi.GetUserEmail(USER_ID);
                //email = "numay.alghalib@gmail.com";
                Notification_send(email, "notify", SendGridTempaltes[SEND_GRID_TITLE], DATA);
            }
            else
            {
                Notification_send(email, "notify", SendGridTempaltes[SEND_GRID_TITLE], DATA);
            }
        }
        public string SendSms(string sms, string phone)
        {
          
            try
            {
                
                const string accountSid = "developer@qayimdactory.com";
                const string authToken = "@Fanar4321!@";
                const string senderID = "QDactory";
                var to = phone;
                var body = sms;
                string url = "http://api.unifonic.com/wrapper/sendSMS.php?userid=" + accountSid + "&password=" + authToken + "&msg=" + body + "&sender=" + senderID + "&to=" + to;
                HttpWebRequest request = HttpWebRequest.Create("http://api.unifonic.com/wrapper/sendSMS.php?userid=developer@qayimdactory.com&password=@Fanar4321!@&msg="+sms+"&sender=QDactory&to="+to) as HttpWebRequest;
                request.KeepAlive = false;
                return request.GetResponse().ToString();
            }
            catch (Exception ex)
            {
                
                //NotifyUser("d-bd1365656a68493aaad6ed1bb8b843e0", null, 3);
            }
            return "ops";
        }
        public void Notification_send(string EmailTo, string Subject, String TemplateID, Object JSON)
        {
            Execute(EmailTo, Subject, TemplateID, JSON).Wait();
        }
        static async Task Execute(string EmailTo, string Subject, String TemplateID, Object JSON)
        {
            string EmailFrom = "no_reply@qayimdactory.com";

            if (TemplateID == "d-ab773db0353244839c258a59458cd28c" || TemplateID == "d-4590bd4958fa42ca8eb57506e671f368")
            {
                EmailFrom = "customer_care@qayimdactory.com";
            }

            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress(EmailFrom, "موقع قيم دكتوري"));
            msg.AddTo(new EmailAddress(EmailTo, Subject));
            msg.SetTemplateId(TemplateID);
            if (JSON != null)
                msg.SetTemplateData(JSON);
            var response = await client.SendEmailAsync(msg);
        }
        public class TemplateData_name
        {
            [JsonProperty("fname")]
            public string Fname { get; set; }
        }
    }
}
