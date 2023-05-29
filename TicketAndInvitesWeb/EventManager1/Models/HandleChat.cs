using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventManager1.DBCon;

namespace EventManager1.Models
{
    public class HandleChat
    {
        public static Livechatmodel GetMessage(int eventId)
        {
            Livechatmodel responce = new Livechatmodel();
            List<Livechat> res = new List<Livechat>();
            try
            {
                using (EventmanagerEntities db = new EventmanagerEntities())
                {
                    res = (from r in db.LiveStreamChats
                           join u in db.Users on r.userId equals u.Id
                           where r.eventID == eventId
                           select new Livechat { Message = r.Message, ProfilePic = u.ProfilePic, UserName = u.FirstName }).ToList();
                    var like = db.Likedislikes.Where(x => x.eventID == eventId).ToList();
                    responce.Totaldislike = like.Where(x => x.Likes == (int)LikedislikeEnum.dislike).Count();
                    responce.Totallike = like.Where(x => x.Likes == (int)LikedislikeEnum.Like).Count();
                    responce.Totalviewer = db.LiveStreamUsers.Where(x => x.eventID == eventId).Count();
                    responce.chatlist = res;
                }
            }
            catch (Exception ex)
            {
            }
            return responce;
        }
        public static Livechatmodel UpdateLike(int eventId, int userid, int value)
        {
            Livechatmodel responce = new Livechatmodel();

            try
            {
                using (EventmanagerEntities db = new EventmanagerEntities())
                {
                    var like = db.Likedislikes.Where(x => x.eventID == eventId);
                    if (like.FirstOrDefault(x => x.userId == userid) != null) { var ulike = like.FirstOrDefault(x => x.userId == userid); ulike.Likes = value; ulike.UpdatedOn = DateTime.UtcNow; }
                    else { Likedislike l = new Likedislike(); l.eventID = eventId; l.Likes = value; l.UpdatedOn = DateTime.UtcNow; l.userId = userid; db.Likedislikes.Add(l); }
                    db.SaveChanges();
                    responce.Totaldislike = like.Where(x => x.Likes == (int)LikedislikeEnum.dislike).Count();
                    responce.Totallike = like.Where(x => x.Likes == (int)LikedislikeEnum.Like).Count();
                    responce.Totalviewer = db.LiveStreamUsers.Where(x => x.eventID == eventId).Count();
                }
            }
            catch (Exception ex)
            {
            }
            return responce;
        }
        public static bool insertMessage(string user, string eventID, string message)
        {

            try
            {
                using (EventmanagerEntities db = new EventmanagerEntities())
                {
                    LiveStreamChat c = new LiveStreamChat();
                    c.Date = DateTime.UtcNow; c.eventID = Convert.ToInt32(eventID); c.Message = message; c.userId = Convert.ToInt32(user);
                    db.LiveStreamChats.Add(c);
                    if (db.SaveChanges() > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
    public class Livechat
    {
        public int eventID { get; set; }
        public int userId { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public string ProfilePic { get; set; }

    }
    public class Livechatmodel
    {
        public List<Livechat> chatlist { get; set; }
        public int Totallike { get; set; }
        public int Totaldislike { get; set; }
        public int Totalviewer { get; set; }
    }
    public enum LikedislikeEnum
    {
        NoAnswer = 0,
        Like = 1,
        dislike = 2,
    }
}