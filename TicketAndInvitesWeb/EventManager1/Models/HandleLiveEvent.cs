using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventManager1.DBCon;
using System.Data.Entity;

namespace EventManager1.Models
{
    public class HandleLiveEvent
    {
        public bool InsertURL(string url, int eventId)
        {
            EventmanagerEntities db = new EventmanagerEntities();
            try
            {
                var ev = db.Events.FirstOrDefault(s => s.Id == eventId);
                ev.webrtcURL = url;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
            }

            return false;
        }
        public liveevent getwebrtcEvent(int id)
        {

            liveevent res = new liveevent();
            try
            {
                using (EventmanagerEntities db = new EventmanagerEntities())
                {
                    var ev = db.Events.Include("MultimediaContents").Where(i => i.Id == id && i.Eventtype == 2).FirstOrDefault();
                    res.eventName = ev.Event_name; res.image = ev.MultimediaContents.FirstOrDefault(c => c.Mul_MainPic == true) != null ? ev.MultimediaContents.FirstOrDefault(c => c.Mul_MainPic == true).URL : ev.MultimediaContents.FirstOrDefault(c => c.Mul_Type == 1).URL;
                }
            }
            catch (Exception ex)
            {
            }
            return res;
        }

        public List<liveevent> GetLiveURL()
        {
            EventmanagerEntities db = new EventmanagerEntities();
            List<liveevent> res = new List<liveevent>();
            try
            {
                var tick = db.TickeUserMaps.Where(x => x.UserId == ManageSession.UserSession.Id && x.Qty > 0).OrderByDescending(a => a.CreateDate).ToList();

                foreach (var j in tick)
                {
                    var ev1 = db.Events.Include("MultimediaContents").Where(i => i.Id == db.EventTickets.FirstOrDefault(x => x.Id == j.TicketId).Event_Id && i.Eventtype == 2);
                    foreach (var i in ev1)
                    {
                        var imgurls = i.MultimediaContents.FirstOrDefault(x => x.Mul_MainPic == true);
                        string imgurl;
                        if (imgurls == null)
                        {
                            imgurl = i.MultimediaContents.FirstOrDefault().URL;
                        }
                        else
                        {
                            imgurl = imgurls.URL;
                        }
                        res.Add(new liveevent
                        {
                            date = Convert.ToDateTime(i.EndDate),
                            sdate = Convert.ToDateTime(i.StartDate),
                            eventName = i.Event_name,
                            EventKey = i.LiveURL,
                            EventURLWebrtc = i.webrtcURL,
                            streamType = i.streamType,
                            evid = EnryptString(i.Id.ToString()),
                            id = i.Id,
                            status = j.Payment.Status,
                            image = imgurl
                        });

                    }
                }
            }
            catch (Exception ex) { }

            return res;
        }
        public int UserLoggedForLiveStreaming(int UserId, int? VideoId,int VideoSession)
        {
            EventmanagerEntities db = new EventmanagerEntities();
            var UserLogged = db.UserLoggedTables.FirstOrDefault(x=>x.UserId== UserId && x.VideoId == VideoId);
            int i = 0;
            try
            {
                if (UserLogged == null)
                {
                    if (UserId > 0 && VideoId > 0)
                    {
                        UserLoggedTable user = new UserLoggedTable();
                       
                        user.UserId = UserId;
                        user.VideoId = VideoId;
                        user.LastActiveDate = DateTime.UtcNow;
                        user.IsSessionStop = true;
                        user.VideoSession = VideoSession;
                        db.UserLoggedTables.Add(user);
                        if (db.SaveChanges() > 0)
                        {
                            i = 1;
                        }
                    }
                }
                else
                {
                    UserLogged.VideoSession = VideoSession;
                    if (db.SaveChanges() > 0)
                    {
                        i = 1;
                    }

                }
            }
            catch (Exception ex)
            {
            }
            return i;
        }
        public UserLoggedforVideoStreaming GetUserStatusForVideoStreaming(int UserId,int VideoId)
        {
            EventmanagerEntities db = new EventmanagerEntities();
            UserLoggedforVideoStreaming video = new UserLoggedforVideoStreaming();
            try
            {
                var v = db.UserLoggedTables.Where(u => u.UserId == UserId && u.VideoId == VideoId).OrderBy(u=> u.ID).FirstOrDefault();
                
                if (v!=null)
                {
                    video.ID = v.ID;
                    video.UserId = v.UserId;
                    video.VideoId = v.VideoId;
                    video.LastActiveDate = v.LastActiveDate;
                    video.IsSessionStop = v.IsSessionStop;
                    video.VideoSession = v.VideoSession;
                }
                
            }
            catch (Exception ex)
            {
            }
            return video;
        }
        public int UpdateLoggedForLiveStreaming(int UserId, int? VideoId, int VideoSession)
        {
            EventmanagerEntities db = new EventmanagerEntities();
            int i = 0;
            try
            {
                if (UserId > 0)
                {
                    var VideoDate = db.UserLoggedTables.Where(x => x.UserId == UserId).FirstOrDefault();
                    if (VideoDate != null)
                    {
                        VideoDate.LastActiveDate = DateTime.UtcNow;
                        VideoDate.IsSessionStop = false;
                        VideoDate.VideoSession = VideoSession;
                        db.Entry(VideoDate).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            i = 1;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return i;
        }
        public int DeleteLoggedForLiveStreaming(int UserId)
        {
            EventmanagerEntities db = new EventmanagerEntities();
            //UserLoggedforVideoStreaming uservideo = new UserLoggedforVideoStreaming();
            //uservideo = GetUserStatusForVideoStreaming(UserId);
            int i = 0;
            try
            {
                if (UserId > 0)
                {
                    var UserData = db.UserLoggedTables.Where(x => x.UserId == UserId).FirstOrDefault();
                    if (UserData != null)
                    {
                        db.Entry(UserData).State = EntityState.Deleted;
                        if (db.SaveChanges() > 0)
                        {
                            i = 1;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return i;
        }
        public string EnryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }
    }
    public class liveevent
    {
        public int id { get; set; }
        public string evid { get; set; }
        public string eventName { get; set; }
        public string image { get; set; }
        public string EventKey { get; set; }

        public int status { get; set; }
        public string EventURLWebrtc { get; set; }
        public int? streamType { get; set; }
        public DateTime date { get; set; }
        public DateTime sdate { get; set; }
        public bool? IsStreaming { get; set; }
    }
    public class UserLoggedforVideoStreaming
    {
        public int ID { get; set; }
        public int? UserId { get; set; }
        public int? VideoId { get; set; }
        public DateTime? LastActiveDate { get; set; }
        public bool? IsSessionStop { get; set; }
        public int? VideoSession { get; set; }
       
    }
}