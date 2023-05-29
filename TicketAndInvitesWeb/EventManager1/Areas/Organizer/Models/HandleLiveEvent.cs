using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventManager1.DBCon;
using System.Data.Entity;
using EventManager1.Models;

namespace EventManager1.Areas.Organizer.Models
{
    public class HandleLiveEvent
    {
        public  List<liveevent> GetLiveEvents()
        {
            EventmanagerEntities db = new EventmanagerEntities();
            List<liveevent> res = new List<liveevent>();
            var ev1 = db.Events.Include("MultimediaContents").Where(i => i.Eventtype == 2 && i.Company_Id == ManageSession.CompanySession.Id && i.CreatedOnWebsite == (int)Ticketforwebsite.Stream233);
            foreach(var i in ev1)
            {
                //string ImageURL = i.MultimediaContents.FirstOrDefault(x => x.Mul_MainPic == true).URL == null ? "" : i.MultimediaContents.FirstOrDefault(x => x.Mul_MainPic == true).URL;

                string ImageURL = "";
                if (i.MultimediaContents.FirstOrDefault(x => x.Mul_MainPic == true)!=null)
                {
                    ImageURL = i.MultimediaContents.FirstOrDefault(x => x.Mul_MainPic == true).URL;
                }


                res.Add(new liveevent { date = Convert.ToDateTime(i.EndDate), EventKey= i.LiveURL, eventName = i.Event_name, id = i.Id, image = ImageURL, streamType= i.streamType, IsStreaming=i.IsStreamimg });

            }
            return res;
        }
        public int updateEventID(int id)
        {
            try
            {
                using (var db = new EventmanagerEntities())
                {
                    var ev = db.Events.FirstOrDefault(x => x.Id == id);
                    ev.LiveURL = CommonDbLogic.RandomString(6) + DateTime.Now.Second;
                    db.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }
        public int updateStreamtype(int id, int stype)
        {
            try
            {
                using (var db = new EventmanagerEntities())
                {
                    var ev = db.Events.FirstOrDefault(x => x.Id == id);
                    ev.streamType = stype;                    
                    db.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }
        public int StartStream(int id)
        {
            try
            {
                using (var db = new EventmanagerEntities())
                {
                    var ev = db.Events.FirstOrDefault(x => x.Id == id);
                    //if (ev.IsStreamimg == false || ev.IsStreamimg == null)
                    //{
                    //    ev.IsStreamimg = true;
                    //}
                    ev.IsStreamimg = true;
                    db.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }
        public int StopStream(int id)
        {
            try
            {
                using (var db = new EventmanagerEntities())
                {
                    var ev = db.Events.FirstOrDefault(x => x.Id == id);
                    ev.IsStreamimg = false;
                    db.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }
    }
    
}