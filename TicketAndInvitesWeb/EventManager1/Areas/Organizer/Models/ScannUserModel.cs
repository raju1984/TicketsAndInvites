using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManager1.Areas.Organizer.Models
{
    public class ScannUserModel
    {
        public  int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class ScanUserManageModel
    {
        public List<ScannUserModel> UserModel { get; set; }
        public ApiResponse Resp { get; set; }
    }

    public class EdituserEventPermisionModel
    {
        public  List<EventDataModel> EventDataModel { get; set; }
        public ScannUserModel UserDetail { get; set; }
        public bool Showbutton { get; set; }
    }

    public class EventDataModel
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public bool permission { get; set; }
        public DateTime CreateDate { get; set; }
    }
}