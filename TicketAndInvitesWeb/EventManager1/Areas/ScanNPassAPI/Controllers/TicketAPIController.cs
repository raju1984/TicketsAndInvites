using EventManager1.DBCon;
using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Text;
using EventManager1.Areas.Organizer.Models;
using EventManager1.Areas.Admin.Models;
using System.Threading.Tasks;

namespace EventManager1.Areas.ScanNPassAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TicketAPIController : ApiController
    {
        EventmanagerEntities dbcon = new EventmanagerEntities();
        [HttpGet]
        public bool UpdateTicketPayment(int payId)
        {
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    //var pendingstatus = dbConn.TickeUserMaps.Where(x => x.Status == 0 || x.Status == 2).ToList();
                    //if (pendingstatus.Count > 0)
                    //{
                    int paymentstaus;
                    //    foreach (var i in pendingstatus)
                    //    {
                    string RefId = dbcon.Payments.FirstOrDefault(x => x.Id == payId).TransactionId;
                    StatusResponse status = HandlePayment.CheckPayment_Status(RefId, "");
                    switch (status.status)
                    {
                        case "SUCCESSFUL":
                            paymentstaus = (int)PaymentStatus.PaymentSuccess;
                            break;
                        case "PENDING":
                            paymentstaus = (int)PaymentStatus.PaymentPending;
                            break;
                        case "SERVER ERROR":
                            paymentstaus = (int)PaymentStatus.CacelledPayment;
                            break;
                        default:
                            paymentstaus = (int)PaymentStatus.PaymentFailed;
                            break;
                    }
                    try
                    {
                        var payment = dbConn.Payments.Include(a => a.TickeUserMaps).Where(a => a.Id == payId).FirstOrDefault();
                        var user = dbcon.Users.FirstOrDefault(x => x.Id == payment.UserId);
                        var ticketusermap = dbcon.TickeUserMaps.FirstOrDefault(x => x.PaymemtId == payment.Id);
                        var ev_ticket = dbcon.EventTickets.FirstOrDefault(x => x.Id == ticketusermap.TicketId);
                        DateTime ticketdate = (DateTime)ev_ticket.StartDate;
                        if (paymentstaus == 1)
                        {
                            payment.Status = paymentstaus;
                            payment.ResponseTransactionId = status.financialTransactionId;
                            foreach (var item in payment.TickeUserMaps)
                            {
                                //TickeUserMap map = new TickeUserMap();
                                //item.BarCodeNumber = common.AlphanumbericNumber() + item.TicketId;
                                item.BarCodeNumber = common.GetUniqueBarCode(item.TicketId);
                                item.Status = paymentstaus;// (int)PaymentStatus.PaymentSuccess;
                                string firstname = user.FirstName;
                                string EmailId = user.Email;
                                var request = HttpContext.Current.Request;
                                var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
                                string rootpath = HttpContext.Current.Server.MapPath("/Views/EmailTemplate/");
                                Thread thread = new Thread(() => PaymentLogic.GenerateBarcodeandsendTicke(item.BarCodeNumber, firstname, EmailId, request.ToString(), address, rootpath, item.Id, item.TicketId, ticketdate));
                                thread.Start();

                                var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(item.Id)));
                                var Link = HttpUtility.UrlDecode(address + "/Ticket?id=" + IdConvert);
                                var EventName = item.EventTicket.Event.Event_name;
                                //GenerateBarcodeandsendTicke(item.BarCodeNumber);
                                CommonSMSModal commonSMSModal = new CommonSMSModal()
                                {
                                    PhoneReciever = user.PhoneNo,// "+918882262496",
                                    Text = "Hi " + user.FirstName + "  You have bought the ticket of Event " + EventName + ". " + Link,
                                    PhoneSender = "" // dbConn.Users.Where(x => x.Id == ManageSession.UserSession.Id).FirstOrDefault().PhoneNo
                                };
                                Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                threadSMS.Start();
                            }
                        }
                        else if (paymentstaus == 2)
                        {

                        }
                        else
                        {
                            foreach (var item in payment.TickeUserMaps)
                            {
                                item.Status = paymentstaus;
                                var x = PaymentLogic.ReduceQuanity(item.TicketId, Convert.ToInt32(item.Qty), 1);
                            }
                        }
                        dbConn.SaveChanges();
                        Log4Net.Debug("Payment API Called:-" + DateTime.UtcNow);
                    }
                    catch (Exception ex)
                    {
                        Log4Net.Debug("Payment API Exception:- " + ex.ToString());
                    }
                }
                return true;
                //}                    
                //return false;
                //}

            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, "TicketAPIController", "UpdatePayment");
                return false;
            }

        }
        [HttpGet]
        public bool UpdatePayment(int payId)
        {
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    int paymentstaus;

                    string RefId = dbcon.Payments.FirstOrDefault(x => x.Id == payId).TransactionId;
                    StatusResponse status = HandlePayment.CheckPayment_Status(RefId, "");
                    switch (status.status)
                    {
                        case "SUCCESSFUL":
                            paymentstaus = (int)PaymentStatus.PaymentSuccess;
                            break;
                        case "PENDING":
                            paymentstaus = (int)PaymentStatus.PaymentPending;
                            break;
                        case "SERVER ERROR":
                            paymentstaus = (int)PaymentStatus.CacelledPayment;
                            break;
                        default:
                            paymentstaus = (int)PaymentStatus.PaymentFailed;
                            break;
                    }
                    try
                    {
                        if (status.code == "Subscription")
                        {
                            if (!string.IsNullOrEmpty(status.ExtId) && !string.IsNullOrEmpty(status.type))
                            {
                                var Resp = OrganizerDbOperation.UpdateSubscription(Convert.ToInt32(status.ExtId), Convert.ToInt32(status.type), payId);
                            }
                        }
                        else if (status.code == "Invitation")
                        {

                        }
                        else if (status.code == "BroadCast")
                        {


                        }
                        else if (status.code == "Withdraw")
                        {
                            if (paymentstaus == 1)
                            {
                                var result = AdminDbLogic.UpdateWithdrawal(Convert.ToInt32(status.ExtId), RefId, DateTime.UtcNow.ToString(), payId, 1);
                            }
                        }
                        else
                        {
                            var payment = dbConn.Payments.Include(a => a.TickeUserMaps).Where(a => a.Id == payId).FirstOrDefault();
                            var user = dbcon.Users.FirstOrDefault(x => x.Id == payment.UserId);
                            var ticketusermap = dbcon.TickeUserMaps.FirstOrDefault(x => x.PaymemtId == payment.Id);
                            var ev_ticket = dbcon.EventTickets.FirstOrDefault(x => x.Id == ticketusermap.TicketId);
                            DateTime ticketdate = (DateTime)ev_ticket.StartDate;
                            if (paymentstaus == 1)
                            {
                                payment.Status = paymentstaus;
                                foreach (var item in payment.TickeUserMaps)
                                {
                                    //TickeUserMap map = new TickeUserMap();
                                    item.BarCodeNumber = common.GetUniqueBarCode(item.TicketId);
                                    item.Status = paymentstaus;// (int)PaymentStatus.PaymentSuccess;
                                    string firstname = user.FirstName;
                                    string EmailId = user.Email;
                                    var request = HttpContext.Current.Request;
                                    var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
                                    string rootpath = HttpContext.Current.Server.MapPath("/Views/EmailTemplate/");
                                    Thread thread = new Thread(() => PaymentLogic.GenerateBarcodeandsendTicke(item.BarCodeNumber, firstname, EmailId, request.ToString(), address, rootpath, item.Id, item.TicketId, ticketdate));
                                    thread.Start();

                                    var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(item.Id)));
                                    var Link = HttpUtility.UrlDecode(address + "/Ticket?id=" + IdConvert);
                                    var EventName = item.EventTicket.Event.Event_name;
                                    //GenerateBarcodeandsendTicke(item.BarCodeNumber);
                                    CommonSMSModal commonSMSModal = new CommonSMSModal()
                                    {
                                        PhoneReciever = user.PhoneNo,// "+918882262496",
                                        Text = "Hi " + user.FirstName + "  You have bought the ticket of Event " + EventName + ". " + Link,
                                        PhoneSender = "" // dbConn.Users.Where(x => x.Id == ManageSession.UserSession.Id).FirstOrDefault().PhoneNo
                                    };
                                    Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                    threadSMS.Start();
                                }
                            }
                            else if (paymentstaus == 2)
                            {

                            }
                            else
                            {
                                foreach (var item in payment.TickeUserMaps)
                                {
                                    item.Status = paymentstaus;
                                    var x = PaymentLogic.ReduceQuanity(item.TicketId, Convert.ToInt32(item.Qty), 1);
                                }
                            }
                            dbConn.SaveChanges();
                            Log4Net.Debug("Payment API Called:-" + DateTime.UtcNow);
                        }
                        var updatepay = PaymentLogic.UpdatePaymentStatus(payId, status.status, status.financialTransactionId);
                    }
                    catch (Exception ex)
                    {
                        Log4Net.Debug("Payment API Exception:- " + ex.ToString());
                    }
                }
                return true;
                //}                    
                //return false;
                //}

            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, "TicketAPIController", "UpdatePayment");
                return false;
            }

        }
        [HttpGet]
        public bool SendInvitation(int InvId)
        {
            try
            {
                if (InvId > 0)
                {
                    Log4Net.Error("SendInvitation Api Called");
                    try
                    {
                        var EventTicke = dbcon.EventTickets.FirstOrDefault(x => x.Inviation_Id == InvId);
                        var Invi = dbcon.Invitations.FirstOrDefault(x => x.Id == InvId);
                        Log4Net.Error("SendInvitationEmailSMS Api :" + Invi.EmailAddress.ToString());
                        if (EventTicke != null)
                        {
                            //send inviation email to user
                            ModelInviation.SendInvitation(Convert.ToInt32(EventTicke.Event_Id), EventTicke.Id, Invi.EmailAddress, Invi.FirstName + " " + Invi.LastName);
                            Invi.IsMailSend = true;
                            dbcon.SaveChanges();
                        }
                    }
                    catch { }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }
        [HttpGet]
        public bool SendInvitationBySMS(int InvId)
        {
            try
            {
                if (InvId > 0)
                {
                    Log4Net.Error("SendInvitationBySMS Api Called");
                    try
                    {
                        var EventTicke = dbcon.EventTickets.FirstOrDefault(x => x.Inviation_Id == InvId);
                        var Invi = dbcon.Invitations.FirstOrDefault(x => x.Id == InvId);
                        string Name = string.Empty;
                        if (Invi.User != null)
                        {

                            Name = Invi.User.FirstName + " " + Invi.User.LastName;
                            //Name = dbcon.Users.FirstOrDefault(x => x.Id == InvId).FirstName;
                        }
                        else if (Invi.Company != null)
                        {
                            Name = Invi.Company.Name_of_business;
                            //Name = dbcon.Companies.FirstOrDefault(x => x.Id == InvId).Name_of_business;
                        }
                        if (!string.IsNullOrEmpty(Name))
                        {
                            Log4Net.Error("SendInvitationEmailSMS Api :" + Invi.EmailAddress.ToString());
                            EventmanagerEntities DbEntity = new EventmanagerEntities();
                            if (EventTicke != null)
                            {
                                var request = System.Web.HttpContext.Current.Request;
                                var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);

                                var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(EventTicke.Event_Id)));
                                var Link = HttpUtility.UrlDecode(address + "/Promotion/SendTicket?EventTickeId=" + EventTicke.Event_Id + "&Status=" + 1);
                                var Event_Id = Convert.ToInt32(EventTicke.Event_Id);
                                var EventName = DbEntity.Events.Where(x => x.Id == Event_Id).FirstOrDefault().Event_name; // item.EventTicket.Event.Event_name;
                                                                                                                          //GenerateBarcodeandsendTicke(item.BarCodeNumber);
                                CommonSMSModal commonSMSModal = new CommonSMSModal()
                                {
                                    PhoneReciever = Invi.MobileNumber, //"+918882262496"
                                    Text = Name + "  has Sent you an invite using ticketsandinvites. " + "" + " " + Link,
                                    PhoneSender = ""// DbEntity.Companies.Where(x => x.Id == ManageSession.CompanySession.Id).FirstOrDefault().Business_contact_number

                                };
                                Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                threadSMS.Start();
                                //send inviation email to user
                                // ModelInviation.SendInvitation(Convert.ToInt32(EventId), EventTickeId, InvitationDetail.EmailAddress, InvitationDetail.FirstName + " " + InvitationDetail.LastName);
                            }
                            Invi.IsSmsSend = true;
                            dbcon.SaveChanges();
                            Log4Net.Error("SendInvitationBySMS Api response done");
                        }

                    }
                    catch { }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        [HttpGet]
        public bool SendInvitationEmailSMS(int InvId)
        {
            try
            {
                if (InvId > 0)
                {
                    try
                    {
                        Log4Net.Error("SendInvitationEmailSMS Api Called");
                        var EventTicke = dbcon.EventTickets.FirstOrDefault(x => x.Inviation_Id == InvId);
                        var Invi = dbcon.Invitations.FirstOrDefault(x => x.Id == InvId);
                        string Name = string.Empty;
                        if (Invi.User != null)
                        {

                            Name = Invi.User.FirstName + " " + Invi.User.LastName;
                            //Name = dbcon.Users.FirstOrDefault(x => x.Id == InvId).FirstName;
                        }
                        else if (Invi.Company != null)
                        {
                            Name = Invi.Company.Name_of_business;
                            //Name = dbcon.Companies.FirstOrDefault(x => x.Id == InvId).Name_of_business;
                        }
                        EventmanagerEntities DbEntity = new EventmanagerEntities();

                        if (EventTicke != null && !string.IsNullOrEmpty(Name))
                        {
                            var request = System.Web.HttpContext.Current.Request;
                            var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);

                            var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(EventTicke.Event_Id)));
                            var Link = HttpUtility.UrlDecode(address + "/Promotion/SendTicket?EventTickeId=" + EventTicke.Event_Id + "&Status=" + 1);
                            Log4Net.Error("Send Invitation Api URL:-" + Link.ToString());
                            var Event_Id = Convert.ToInt32(EventTicke.Event_Id);
                            var EventName = DbEntity.Events.Where(x => x.Id == Event_Id).FirstOrDefault().Event_name; // item.EventTicket.Event.Event_name;
                                                                                                                      //GenerateBarcodeandsendTicke(item.BarCodeNumber);
                            CommonSMSModal commonSMSModal = new CommonSMSModal()
                            {
                                PhoneReciever = Invi.MobileNumber, //"+918882262496"
                                Text = Name + "  has Sent you an invite using ticketsandinvites. " + "" + ":- " + Link,
                                PhoneSender = ""// DbEntity.Companies.Where(x => x.Id == ManageSession.CompanySession.Id).FirstOrDefault().Business_contact_number
                            };

                            if (Invi.MobileNumber != null)
                            {
                                Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                threadSMS.Start();
                                Log4Net.Error("SendInvitationBySMS Api response sms done");
                            }
                            //send inviation email to user
                            ModelInviation.SendInvitation(Convert.ToInt32(EventTicke.Event_Id), EventTicke.Id, Invi.EmailAddress, Invi.FirstName + " " + Invi.LastName);
                            Invi.IsSmsSend = true;
                            Invi.IsMailSend = true;
                            dbcon.SaveChanges();
                            Log4Net.Error("SendInvitationBySMS Api response email done");
                        }
                    }
                    catch { }

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }

        [HttpGet]
        public async Task<string> SendInvitationEmailNew(int EventId, int EventTicketId, string EmailAdress, string recivername)
        {
            try
            {
                if (EventId > 0 && EventTicketId > 0 && !string.IsNullOrEmpty(EmailAdress) && !string.IsNullOrEmpty(recivername))
                {
                    Log4Net.Error("SendInvitationEmailNew Api Called");
                    try
                    {
                        //var EventTicke = dbcon.EventTickets.FirstOrDefault(x => x.Inviation_Id == InvId);
                        //var Invi = dbcon.Invitations.FirstOrDefault(x => x.Id == InvId);
                        Log4Net.Error("SendInvitationEmailNew Api :" + EmailAdress.ToString());
                        //send inviation email to user
                        if(await ModelInviation.SendInvitation(EventId, EventTicketId, EmailAdress, recivername))
                        {
                            return "success";
                        }
                        else
                        {
                            return "email send failed";
                        }
                    }
                    catch(Exception ex)
                    {
                        return ex.Message;
                    }
                }
                else
                {
                    return "invalid Parameter supply to api";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        [HttpGet]
        public bool SendBroadcast(int BrodId)
        {
            try
            {
                if (BrodId > 0)
                {
                    try
                    {
                        //var EventTicke = dbcon.EventTickets.FirstOrDefault(x => x.Inviation_Id == InvId);
                        var Invi = dbcon.Broadcasts.Include(y => y.BroadcastMessage).FirstOrDefault(x => x.Id == BrodId);

                        if (Invi != null)
                        {
                            BroadcastEmails em = new BroadcastEmails();
                            em.Message = Invi.BroadcastMessage.Message; em.Subject = Invi.BroadcastMessage.Subject;
                            var mail = ModelInviation.SendBroadcast(Convert.ToInt32(Invi.Event_Id), Invi.EmailAddress, em);
                            var update = ModelInviation.updatebroadcast(Invi.Id);
                            //send broadcast email to user
                        }
                    }
                    catch (Exception ex) { }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }

        [HttpGet]
        public StatusResponse GetPaymentStatus(string TransactionId,int type=0)
        {
            StatusResponse status = new StatusResponse();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                   // string RefId = dbcon.Payments.FirstOrDefault(x => x.Id == payId).TransactionId;
                    status = HandlePayment.CheckPayment_Status(TransactionId, "",type);
                    return status;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, "TicketAPIController", "UpdatePayment");
                status.message = ex.Message;
                return status;
            }

        }

        public string GetSendTicketToUser(string BarCodeNumber,string firstname,string EmailId,string eventname,int TicketMapId,int TicketId)
        {
            try
            {
                EventmanagerEntities dbConn = new EventmanagerEntities();
                var objTicket = dbConn.EventTickets.Where(a => a.Id == TicketId).FirstOrDefault();
                var request = HttpContext.Current.Request;
                DateTime ticketdate = (DateTime)objTicket.StartDate;
                var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
                string rootpath = HttpContext.Current.Server.MapPath("/Views/EmailTemplate/");
                Log4Net.Error("GetSendTicketToUser:"+ "BarCodeNumber:- "+BarCodeNumber + "firstname: -" + firstname + "EmailId :-"+EmailId
                 +"request: " + request.ToString()+ "address: " + address + "rootpath :" + rootpath+ "TicketMapId :"+TicketMapId + "TicketId :" + TicketId);
                PaymentLogic.GenerateBarcodeandsendTicke(BarCodeNumber, firstname, EmailId, request.ToString(), address, rootpath, TicketMapId, TicketId, ticketdate);
                return "success";
            }
            catch(Exception ex)
            {
                return "failed";
            }
        }

    }
}