using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManager1.Areas.Organizer.Models
{
    public class OrganizerPay
    {
        public string page { get; set; }
        public string subtype { get; set; }
        public string total { get; set; }
        public string eventId { get; set; }
        public string EventName { get; set; }
        public string ImageUrl { get; set; }
        public string AdminFee { get; set; }
        public decimal? AdminFeeSMS { get; set; }
        public decimal? AdminFeeEmail { get; set; }
        public int SendInviteType { get; set; }        
    }
    
    
}