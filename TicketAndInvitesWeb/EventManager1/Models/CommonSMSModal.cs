using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManager1.Models
{
    public class CommonSMSModal
    {
        public string PhoneSender { get; set; }
        public string PhoneReciever { get; set; }
        public string Text { get; set; }
        public string Countrycode { get; set; }
    }

    public class CommonSMSRModal
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string SMS_ID { get; set; }
    }
}