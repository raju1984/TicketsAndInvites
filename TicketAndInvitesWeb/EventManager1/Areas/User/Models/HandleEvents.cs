using EventManager1.DBCon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using EventManager1.Models;
namespace EventManager1.Areas.User.Models
{    
    public class HandleEvents
    {
        public static EVideo GetEVideo(string evid)
        {
            EventmanagerEntities hdb = new EventmanagerEntities();
            EVideo res = new EVideo();
            try
            {
                var ress = hdb.MultimediaContents.Include(X=>X.Event).FirstOrDefault(x => x.videoId == evid && x.videotype == 1);                
                res.EventName = ress.Event.Event_name; res.EventId = ress.Event_Id;
                
                if (ress != null && ress.URL !=null)
                {
                    var user = hdb.TickeUserMaps.Include(y => y.EventTicket).Include(y => y.EventTicket.Event).Where(x => x.UserId == ManageSession.UserSession.Id).ToList();
                    foreach (var i in user)
                    {
                        if (i.EventTicket.Event.Id == ress.Event_Id)
                        {
                            DateTime now = DateTime.UtcNow;
                            int days = Convert.ToInt32( i.EventTicket.ValidDays);
                            if (now > Convert.ToDateTime(i.OrderDate).AddHours(0) && now <= Convert.ToDateTime(i.OrderDate).AddHours(24 * days))
                            { res.videoUrl = ress.URL; }
                            //var days = (DateTime.UtcNow.Date - Convert.ToDateTime(i.OrderDate).Date).Days;
                            //if (i.EventTicket.ValidDays > days)
                            //{
                            //    res.videoUrl = ress.URL;
                            //}
                        }
                    }
                    
                }
                else { res.msg = "No Record Found."; }
            }
            catch { res.msg = "Something went wrong."; }
            return res;
        }
    }
    public class EVideo
    {
        public string videoUrl { get; set; }
        public string msg { get; set; }
        public string EventName { get; set; }
        public int? EventId { get; set; }
    }
}