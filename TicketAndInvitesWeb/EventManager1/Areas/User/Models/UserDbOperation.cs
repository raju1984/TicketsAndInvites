using EventManager1.Areas.Organizer.Models;
using EventManager1.DBCon;
using EventManager1.Models;
using EventManager1.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace EventManager1.Areas.User.Models
{
    public class UserDbOperation
    {
        public static List<InvitationDetail> GetInvition(int EventId)
        {
            List<InvitationDetail> result = new List<InvitationDetail>();
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                dbConn.Configuration.LazyLoadingEnabled = false;
                try
                {
                    List<Invitation> invitation = new List<Invitation>();
                    if (EventId > 0)
                    {
                        invitation = dbConn.Invitations.Include(a => a.Payment).Include(a => a.Event).Include(a => a.EventTickets).Where(b => b.Event_Id == EventId).OrderByDescending(a=>a.Created_at).ToList();
                    }
                    else
                    {
                        invitation = dbConn.Invitations.Include(a => a.Payment).Include(a => a.Event).Include(a => a.EventTickets).Where(b => b.UserId == ManageSession.UserSession.Id).OrderByDescending(a => a.Created_at).ToList();
                    }
                    if (invitation != null)
                    {
                        result = (from r in invitation
                                  where r.Payment != null
                                  select new InvitationDetail
                                  {
                                      FirstName = r.FirstName,
                                      EventName = r.Event.Event_name,
                                      EmailAddress = r.EmailAddress,
                                      SeatNumber = r.EventTickets.FirstOrDefault() != null ? r.EventTickets.FirstOrDefault().Seat : "",
                                      TableNumber = r.EventTickets.FirstOrDefault() != null ? r.EventTickets.FirstOrDefault().tableNo : "",
                                      ColorCode = r.EventTickets.FirstOrDefault() != null ? r.EventTickets.FirstOrDefault().ColorArea : "",
                                      Status = r.Status,
                                      PaymentStatus = r.Payment.Status,
                                      Remark = OrganizerDbOperation.GetInvitationStatus(r.Payment.Status, r.SendType, r.IsMailSend, r.IsSmsSend)
                                  }).ToList();
                    }
                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }
        public static List<Dropdownlist> GetEventCreatedByUser()
        {
            List<Dropdownlist> Resp = new List<Dropdownlist>();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    Resp = (from r in dbConn.Events
                            where r.User_Id == ManageSession.UserSession.Id
                            select new Dropdownlist
                            {
                                Id=r.Id,
                                Text=r.Event_name
                            }).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return Resp;
        }

        public static ApiResponse UpdatePassword(string passowrd,string Oldpassword, int UserId)
        {
            ApiResponse Resp = new ApiResponse();
            try
            {
                Resp.Code = (int)ApiResponseCode.fail;
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var result = dbConn.Users.Where(a => a.Id == UserId).FirstOrDefault();
                    if (result != null && result.Password == Oldpassword)
                    {
                        result.Password = passowrd;
                        dbConn.SaveChanges();
                        Resp.Code = (int)ApiResponseCode.ok;
                        return Resp;
                    }
                    else
                    {
                        Resp.Msg = ApplicationStrings.OldPassordNotmacth;
                    }
                }
            }
            catch(Exception ex)
            {
                Resp.Msg = ex.Message;
            }
            return Resp;
        }
        public static ApiResponse UpdateDetail(string email,string firstname,string lastname,int UserId)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                var result = dbConn.Users.Where(a => a.Id == UserId).FirstOrDefault();
                if (result != null)
                {
                    result.Email = email;
                    result.FirstName = firstname;
                    result.LastName = lastname;
                    dbConn.SaveChanges();
                    ManageSession.UserSession.FirstName = firstname;
                    ManageSession.UserSession.LastName = lastname;
                    ManageSession.UserSession.EmailId = email;
                    Resp.Code = (int)ApiResponseCode.ok;
                    return Resp;
                }
                else
                {
                    Resp.Msg = ApplicationStrings.Somethingwrmg;
                }
            }
            return Resp;
        }
    }
}