using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManager1.Areas.Organizer.Models
{
    public class EventticketModel
    {
        public int Id { get; set; }
        public string TicketName  { get; set; }
        public string Name { get; set; }
        public string Eventname { get; set; }
        public string TicketbuyerEmail { get; set; }
        public bool CheckInStatus { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime Startdate { get; set; }
        public string Venuename { get; set; }
    }

    public class OvertrackModels
    {
        public List<EventticketModel> Tickets { get; set; }
        public List<Dropdownlist> Events { get; set; }
    }
}