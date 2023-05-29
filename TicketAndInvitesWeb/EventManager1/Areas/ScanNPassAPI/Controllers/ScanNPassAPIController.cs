using EventManager1.Areas.Organizer.Models;
using EventManager1.Areas.ScanNPassAPI.Models;
using EventManager1.DBCon;
using EventManager1.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EventManager1.Areas.ScanNPassAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ScanNPassAPIController : ApiController
    {
        // GET api/<controller>
        EventmanagerEntities dbcon = new EventmanagerEntities();
        string path = ConfigurationManager.AppSettings["ServerLink"];
        [HttpPost]
        public IHttpActionResult login(Models.LoginViewModel model)
        {
            Models.CompanyModel res = new Models.CompanyModel();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (model != null)
            {
                try
                {

                    dbcon = new EventmanagerEntities();
                    res = (from e in dbcon.ScanCompanyUsers
                           join comp in dbcon.Companies on e.CompanyId equals comp.Id
                           where e.Email == model.Email && e.Password == model.Password && e.Status == (int)ScannUserStatus.Active
                           select new Models.CompanyModel
                           {
                               Id = e.Id,
                               CompName = comp.Name_of_business,
                               Name = e.Name,
                               CompanyId = e.CompanyId ?? 0,
                               EmailId = e.Email,
                               Type = 0
                           }).FirstOrDefault();

                    if (res != null)
                    {
                        return Ok(new { results = res });
                    }
                    else
                    {
                        dbcon = new EventmanagerEntities();
                        res = (from e in dbcon.Users
                               where e.Email == model.Email && e.Password == model.Password && e.UserStatus == (int)userstatus.active
                               select new Models.CompanyModel
                               {
                                   Id = e.Id,
                                   CompName = e.UserName,
                                   Name = e.UserName,
                                   CompanyId = e.Id,
                                   EmailId = e.Email,
                                   Type = 1
                               }).FirstOrDefault();
                        return Ok(new { results = res });

                    }


                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, "ScanNPassAPIController", "login");
                    return BadRequest();
                }
            }
            return NotFound();
        }
        [HttpGet]
        public dynamic Events(int id, int Type)
        {

            List<EventModel> res = new List<EventModel>();
            if (id > 0)
            {
                try
                {
                    using (EventmanagerEntities dbcon = new EventmanagerEntities())
                    {
                        if (Type == 0)
                        {
                            DateTime date = new DateTime();
                            date = DateTime.UtcNow;
                            res = (
                                  from scanURel in dbcon.ScannerEventRels
                                  join e in dbcon.Events on scanURel.EventId equals e.Id
                                  join adr in dbcon.Addresses on e.Address_Id equals adr.Id
                                  join mul in dbcon.MultimediaContents on e.Id equals mul.Event_Id
                                  where scanURel.ScanerId == id && e.Status == 2 && scanURel.status == (int)ScannUserStatus.Active && (e.StartDate <= date.Date || e.StartDate >= date.Date) && e.EndDate >= date.Date
                                  select (new EventModel
                                  {
                                      Id = e.Id,
                                      EventName = e.Event_name,
                                      CityName = adr.City,
                                      StartDate = e.StartDate,
                                      EndDate = e.EndDate,
                                      Venue = e.Venue
                                  })).Distinct().ToList();
                            if (res.Count > 0)
                            {
                                foreach (var item in res)
                                {
                                    var mult = dbcon.MultimediaContents.Where(x => x.Event_Id == item.Id && x.Mul_Type == 1).ToList();
                                    if (mult != null)
                                    {

                                        var r = mult.FirstOrDefault(x => x.Mul_MainPic == true);
                                        if (r != null) { item.ImagePath = r.URL.Replace("../..", ""); } else { item.ImagePath = mult.FirstOrDefault().URL.Replace("../..", ""); }
                                    }
                                }
                                return res;
                            }
                            else
                            {
                                return res;
                            }
                        }
                        else
                        {
                            DateTime date = new DateTime();
                            date = DateTime.UtcNow;
                            res = (
                                  from e in dbcon.Events
                                  join adr in dbcon.Addresses on e.Address_Id equals adr.Id
                                  join mul in dbcon.MultimediaContents on e.Id equals mul.Event_Id
                                  where e.User_Id == id && e.Status == 2 && (e.StartDate <= date.Date || e.StartDate >= date.Date) && e.EndDate >= date.Date
                                  select (new EventModel
                                  {
                                      Id = e.Id,
                                      EventName = e.Event_name,
                                      CityName = adr.City,
                                      StartDate = e.StartDate,
                                      EndDate = e.EndDate,
                                      Venue = e.Venue
                                  })).Distinct().ToList();
                            if (res.Count > 0)
                            {
                                foreach (var item in res)
                                {
                                    var mult = dbcon.MultimediaContents.Where(x => x.Event_Id == item.Id && x.Mul_Type == 1).ToList();
                                    if (mult != null)
                                    {

                                        var r = mult.FirstOrDefault(x => x.Mul_MainPic == true);
                                        if (r != null) { item.ImagePath = r.URL.Replace("../..", ""); } else { item.ImagePath = mult.FirstOrDefault().URL.Replace("../..", ""); }
                                    }
                                }
                                return res;
                            }
                            else
                            {
                                return res;
                            }

                        }

                    }

                }
                catch (Exception ex)
                {

                    ExceptionHandler.LogException(ex, "ScanNPassAPIController", "Events");
                }
            }
            return res;
        }
        [HttpPost]
        public EventTickets ValidateTicket(Barcodemodels barcode)
        {
            EventTickets res = new EventTickets();
            if (barcode != null)
            {
                try
                {
                    using (EventmanagerEntities dbcon = new EventmanagerEntities())
                    {

                        res = (from e in dbcon.TickeUserMaps
                               join tic in dbcon.EventTickets on e.TicketId equals tic.Id
                               join ev in dbcon.Events on tic.Event_Id equals ev.Id
                               join ad in dbcon.Addresses on ev.Address_Id equals ad.Id
                               where e.BarCodeNumber == barcode.Barcode && ev.Id == barcode.Id
                               select new EventTickets
                               {
                                   Id = ev.Id,
                                   Name = e.UserId == 0 ? dbcon.Invitations
                                                                    .Where(x => x.Id == e.InvitationId).FirstOrDefault().FirstName + " " +
                                                                    dbcon.Invitations.Where(x => x.Id == e.InvitationId).FirstOrDefault().LastName :
                                                                    e.Name,
                                   Barcode = e.BarCodeNumber,
                                   ColorArea = tic.ColorArea,
                                   TicketName = e.UserId != null ? tic.TicketName : "Invitation",
                                   EventName = ev.Event_name,
                                   CityName = ad.City,
                                   StartDate = ev.StartDate,
                                   Venue = ev.Venue,
                                   GateNo = tic.GateNo,
                                   Quantity = e.Qty,
                                   IsCheckIn = e.IsCheckIn ?? false,
                                   barcodeImageP = "/Content/Barcode/" + e.BarCodeNumber + ".jpg",
                                   ticketType = tic.Ticket_Type == (int)EventManager1.Models.TicketType.Paid ? "Paid" : "Free",
                                   updatedate = e.UpdateDate
                               }).FirstOrDefault();
                        if (res != null)
                        {
                            if (res.IsCheckIn == false)
                            {
                                var data = dbcon.TickeUserMaps.Where(x => x.BarCodeNumber == barcode.Barcode).FirstOrDefault();
                                if (data != null)
                                {
                                    data.IsCheckIn = true;
                                    data.UpdateDate = DateTime.UtcNow;
                                    dbcon.SaveChanges();
                                }
                            }
                        }
                        var mult = dbcon.MultimediaContents.Where(x => x.Event_Id == res.Id && x.Mul_Type == 1).ToList();
                        if (mult != null)
                        {
                            var r = mult.FirstOrDefault(x => x.Mul_MainPic == true);
                            if (r != null) { res.ImagePath = r.URL.Replace("../..", ""); } else { res.ImagePath = mult.FirstOrDefault().URL.Replace("../..", ""); }
                        }
                    }

                    //return Ok(new { results = res });
                }
                catch (Exception ex)
                {

                    ExceptionHandler.LogException(ex, "ScanNPassAPIController", "ValidateTicket");
                    return null;
                }
            }
            return res;

        }


        [HttpGet]
        public dynamic Events(int id, string Event, int Type)
        {
            string path = ConfigurationManager.AppSettings["ServerLink"];
            List<EventModel> res = new List<EventModel>();
            if (id > 0)
            {
                try
                {
                    using (EventmanagerEntities dbcon = new EventmanagerEntities())
                    {
                        if (Type == 0)
                        {
                            DateTime date = new DateTime();
                            date = DateTime.UtcNow;
                            res = (from scanURel in dbcon.ScannerEventRels
                                   join e in dbcon.Events on scanURel.EventId equals e.Id
                                   join adr in dbcon.Addresses on e.Address_Id equals adr.Id
                                   join mul in dbcon.MultimediaContents on e.Id equals mul.Event_Id
                                   where scanURel.ScanerId == id && e.Status == 2 && e.Event_name.ToUpper().Contains(Event.ToUpper()) && scanURel.status == (int)ScannUserStatus.Active && e.StartDate <= date.Date && e.EndDate >= date.Date
                                   select (new EventModel
                                   {
                                       Id = e.Id,
                                       EventName = e.Event_name,
                                       CityName = adr.City,
                                       StartDate = e.StartDate,
                                       EndDate = e.EndDate,
                                       Venue = e.Venue
                                   })).Distinct().ToList();
                            if (res.Count > 0)
                            {
                                foreach (var item in res)
                                {
                                    var mult = dbcon.MultimediaContents.Where(x => x.Event_Id == item.Id && x.Mul_Type == 1).ToList();
                                    if (mult != null)
                                    {

                                        var r = mult.FirstOrDefault(x => x.Mul_MainPic == true);
                                        if (r != null) { item.ImagePath = r.URL.Replace("../..", ""); } else { item.ImagePath = mult.FirstOrDefault().URL.Replace("../..", ""); }
                                    }
                                }
                                return res;
                            }
                            else
                            {
                                return res;
                            }
                        }
                        else
                        {
                            DateTime date = new DateTime();
                            date = DateTime.UtcNow;
                            res = (
                                   from e in dbcon.Events
                                   join adr in dbcon.Addresses on e.Address_Id equals adr.Id
                                   join mul in dbcon.MultimediaContents on e.Id equals mul.Event_Id
                                   where e.User_Id == id && e.Status == 2 && e.Event_name.ToUpper().Contains(Event.ToUpper()) && e.StartDate <= date.Date && e.EndDate >= date.Date
                                   select (new EventModel
                                   {
                                       Id = e.Id,
                                       EventName = e.Event_name,
                                       CityName = adr.City,
                                       StartDate = e.StartDate,
                                       EndDate = e.EndDate,
                                       Venue = e.Venue
                                   })).Distinct().ToList();
                            if (res.Count > 0)
                            {
                                foreach (var item in res)
                                {
                                    var mult = dbcon.MultimediaContents.Where(x => x.Event_Id == item.Id && x.Mul_Type == 1).ToList();
                                    if (mult != null)
                                    {

                                        var r = mult.FirstOrDefault(x => x.Mul_MainPic == true);
                                        if (r != null) { item.ImagePath = r.URL.Replace("../..", ""); } else { item.ImagePath = mult.FirstOrDefault().URL.Replace("../..", ""); }
                                    }
                                }
                                return res;
                            }
                            else
                            {
                                return res;
                            }

                        }

                    }

                }
                catch (Exception ex)
                {

                    ExceptionHandler.LogException(ex, "ScanNPassAPIController", "Events");
                }
            }
            return res;
        }


        [HttpGet]
        public EventTickets Geteventdetails(int id)
        {
            EventTickets res = new EventTickets();
            if (id > 0)
            {
                try
                {
                    using (EventmanagerEntities dbcon = new EventmanagerEntities())
                    {
                        res = (from e in dbcon.Events
                               join ad in dbcon.Addresses on e.Address_Id equals ad.Id // vanue address
                               where e.Id == id
                               select new EventTickets
                               {
                                   Id = e.Id,
                                   EventName = e.Event_name,
                                   CityName = ad.City,
                                   StartDate = e.StartDate,
                                   EndDate = e.EndDate,
                                   Venue = e.Venue
                               }).FirstOrDefault();
                        var mult = dbcon.MultimediaContents.Where(x => x.Event_Id == res.Id && x.Mul_Type == 1).ToList();
                        if (mult != null)
                        {
                            var r = mult.FirstOrDefault(x => x.Mul_MainPic == true);
                            if (r != null) { res.ImagePath = r.URL.Replace("../..", ""); } else { res.ImagePath = mult.FirstOrDefault().URL.Replace("../..", ""); }
                        }
                    }

                    //return Ok(new { results = res });
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, "ScanNPassAPIController", "Geteventdetails");
                    return null;
                }
            }
            return res;

        }

        [HttpGet]
        public dynamic ScannedTicketList(int Event_Id)
        {
            List<EventticketModel> eventsticket = new List<EventticketModel>();
            if (Event_Id > 0)
            {
                try
                {
                    using (EventmanagerEntities dbcon = new EventmanagerEntities())
                    {


                        //get user from invitation list
                        int EventId = Convert.ToInt32(Event_Id);
                        int id = Convert.ToInt32(Event_Id);
                        eventsticket = (from ev in dbcon.Events
                                        join tic in dbcon.EventTickets on ev.Id equals tic.Event_Id
                                        join e in dbcon.TickeUserMaps on tic.Id equals e.TicketId
                                        where ev.Id == EventId && e.IsCheckIn == true
                                        select new EventticketModel
                                        {
                                            Id = tic.Id,
                                            TicketName = e.UserId != null ? tic.TicketName : "Invitation",
                                            Eventname = e.UserId == 0 ? dbcon.Invitations
                                                                    .Where(x => x.Id == e.InvitationId).FirstOrDefault().FirstName + " " +
                                                                    dbcon.Invitations.Where(x => x.Id == e.InvitationId).FirstOrDefault().LastName :
                                                                    dbcon.Users.Where(x => x.Id == e.UserId).FirstOrDefault().FirstName + " " +
                                                                    dbcon.Users.Where(x => x.Id == e.UserId).FirstOrDefault().LastName,
                                            TicketbuyerEmail = e.UserId == 0 ? dbcon.Invitations.Where(x => x.Id == e.InvitationId).FirstOrDefault().EmailAddress : e.Email,
                                            CheckInStatus = e.IsCheckIn ?? false,
                                            Startdate = ev.StartDate ?? DateTime.Now,
                                            Venuename = ev.Venue,
                                            Name = e.UserId == 0 ? dbcon.Invitations
                                                                    .Where(x => x.Id == e.InvitationId).FirstOrDefault().User.FirstName + " " +
                                                                    dbcon.Invitations.Where(x => x.Id == e.InvitationId).FirstOrDefault().User.LastName :
                                                                    dbcon.Users.Where(x => x.Id == e.UserId).FirstOrDefault().FirstName + " " +
                                                                    dbcon.Users.Where(x => x.Id == e.UserId).FirstOrDefault().LastName,

                                        }).ToList();

                        return eventsticket;
                    }

                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, "ScanNPassAPIController", "ScannedTicketList");
                }
            }
            return eventsticket;
        }



        [HttpPost]
        public IHttpActionResult Sociallogin(Models.LoginSoViewModel model)
        {
            Models.CompanyModel res = new Models.CompanyModel();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (model != null)
            {
                try
                {
                    dbcon = new EventmanagerEntities();
                    res = (from e in dbcon.Users
                           where e.Email == model.Email && e.UserStatus == (int)userstatus.active
                           select new Models.CompanyModel
                           {
                               Id = e.Id,
                               CompName = e.UserName,
                               Name = e.UserName,
                               CompanyId = e.Id,
                               EmailId = e.Email,
                               Type = 1
                           }).FirstOrDefault();
                    return Ok(new { results = res });




                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, "ScanNPassAPIController", "Sociallogin");
                    return BadRequest();
                }
            }
            return NotFound();
        }

        [HttpGet]
        public dynamic getLocalData(int Id, int Type, DateTime time)
        {

            List<LocalDataModel> res = new List<LocalDataModel>();
            if (Id > 0)
            {
                try
                {
                    using (EventmanagerEntities dbcon = new EventmanagerEntities())
                    {
                        if (Type == 0)
                        {
                            DateTime date = new DateTime();
                            date = DateTime.UtcNow;
                            res = (
                                  from scanURel in dbcon.ScannerEventRels
                                  join e in dbcon.Events on scanURel.EventId equals e.Id
                                  join adr in dbcon.Addresses on e.Address_Id equals adr.Id
                                  join ETIc in dbcon.EventTickets on e.Id equals ETIc.Event_Id
                                  join tick in dbcon.TickeUserMaps on ETIc.Id equals tick.TicketId
                                  where scanURel.ScanerId == Id && e.Status == 2 && scanURel.status == (int)ScannUserStatus.Active && (e.StartDate <= date.Date || e.StartDate >= date.Date) && e.EndDate >= date.Date && (tick.UpdateDate == null ? tick.CreateDate > time : tick.UpdateDate > time)
                                  select (new LocalDataModel
                                  {
                                      Event_Id = e.Id,
                                      Event_Name = e.Event_name,
                                      City_Name = adr.City,
                                      Start_Date = e.StartDate,
                                      End_Date = e.EndDate,
                                      Event_Venue = e.Venue,

                                      Ticket_Id = tick.Id,
                                      ticket_Barcode = tick.BarCodeNumber,
                                      Ticket_Name = tick.UserId != null ? ETIc.TicketName : "Invitation",
                                      Ticket_type = ETIc.Ticket_Type == (int)EventManager1.Models.TicketType.Paid ? "Paid" : "Free",
                                      TicketColor = ETIc.ColorArea,
                                      Ticket_price = ETIc.Price ?? 0,
                                      Ticket_buyer_name = tick.UserId == 0 ? dbcon.Invitations
                                                                    .Where(x => x.Id == tick.InvitationId).FirstOrDefault().FirstName + " " +
                                                                    dbcon.Invitations.Where(x => x.Id == tick.InvitationId).FirstOrDefault().LastName :
                                                                    dbcon.Users.Where(x => x.Id == tick.UserId).FirstOrDefault().FirstName + " " +
                                                                    dbcon.Users.Where(x => x.Id == tick.UserId).FirstOrDefault().LastName,
                                      Email = tick.UserId == 0 ? dbcon.Invitations.Where(x => x.Id == tick.InvitationId).FirstOrDefault().EmailAddress : tick.Email,
                                      IsCheckIn = tick.IsCheckIn ?? false,
                                      Quantity = tick.Qty ?? 0,
                                      Gate_No = ETIc.GateNo,
                                      Update_Date = tick.UpdateDate,
                                      Ticket_User_name = tick.UserId == 0 ? dbcon.Invitations
                                                                    .Where(x => x.Id == tick.InvitationId).FirstOrDefault().User.FirstName + " " +
                                                                    dbcon.Invitations.Where(x => x.Id == tick.InvitationId).FirstOrDefault().User.LastName :
                                                                    dbcon.Users.Where(x => x.Id == tick.UserId).FirstOrDefault().FirstName + " " +
                                                                    dbcon.Users.Where(x => x.Id == tick.UserId).FirstOrDefault().LastName,
                                  })).Distinct().ToList();


                            return res;

                        }
                        else
                        {
                            DateTime date = new DateTime();
                            date = DateTime.UtcNow;
                            res = (
                                  from e in dbcon.Events
                                  join adr in dbcon.Addresses on e.Address_Id equals adr.Id
                                  join ETIc in dbcon.EventTickets on e.Id equals ETIc.Event_Id
                                  join tick in dbcon.TickeUserMaps on ETIc.Id equals tick.TicketId
                                  where e.User_Id == Id && e.Status == 2 && (e.StartDate <= date.Date || e.StartDate >= date.Date) && e.EndDate >= date.Date && (tick.UpdateDate == null ? tick.CreateDate > time : tick.UpdateDate > time)
                                  select (new LocalDataModel
                                  {
                                      Event_Id = e.Id,
                                      Event_Name = e.Event_name,
                                      City_Name = adr.City,
                                      Start_Date = e.StartDate,
                                      End_Date = e.EndDate,
                                      Event_Venue = e.Venue,

                                      Ticket_Id = tick.Id,
                                      ticket_Barcode = tick.BarCodeNumber,
                                      Ticket_Name = tick.UserId != null ? ETIc.TicketName : "Invitation",
                                      Ticket_buyer_name = tick.UserId == 0 ? dbcon.Invitations
                                                                    .Where(x => x.Id == tick.InvitationId).FirstOrDefault().FirstName + " " +
                                                                    dbcon.Invitations.Where(x => x.Id == tick.InvitationId).FirstOrDefault().LastName :
                                                                    dbcon.Users.Where(x => x.Id == tick.UserId).FirstOrDefault().FirstName + " " +
                                                                    dbcon.Users.Where(x => x.Id == tick.UserId).FirstOrDefault().LastName,
                                      Email = tick.UserId == 0 ? dbcon.Invitations.Where(x => x.Id == tick.InvitationId).FirstOrDefault().EmailAddress : tick.Email,
                                      IsCheckIn = tick.IsCheckIn ?? false,
                                      Quantity = tick.Qty ?? 0,
                                      Gate_No = ETIc.GateNo,
                                      Table_No = ETIc.tableNo,
                                      Sheet_No = ETIc.Seat,
                                      Ticket_type = ETIc.Ticket_Type == (int)EventManager1.Models.TicketType.Paid ? "Paid" : "Free",
                                      TicketColor = ETIc.ColorArea,
                                      Ticket_price = ETIc.Price ?? 0,
                                      Update_Date = tick.UpdateDate,
                                      Ticket_User_name = tick.UserId == 0 ? dbcon.Invitations
                                                                    .Where(x => x.Id == tick.InvitationId).FirstOrDefault().User.FirstName + " " +
                                                                    dbcon.Invitations.Where(x => x.Id == tick.InvitationId).FirstOrDefault().User.LastName :
                                                                    dbcon.Users.Where(x => x.Id == tick.UserId).FirstOrDefault().FirstName + " " +
                                                                    dbcon.Users.Where(x => x.Id == tick.UserId).FirstOrDefault().LastName,
                                  })).ToList();

                            return res;
                        }

                    }

                }
                catch (Exception ex)
                {

                    ExceptionHandler.LogException(ex, "ScanNPassAPIController", "Events");
                }
            }
            return res;

        }


        [HttpPost]
        public dynamic SavedataList(List<SavedateModal> SavedateModal)
        {


            if (SavedateModal.Count() > 0)
            {
                try
                {
                    using (EventmanagerEntities dbcon = new EventmanagerEntities())
                    {
                        foreach (var item in SavedateModal)
                        {
                            var data = dbcon.TickeUserMaps.Where(x => x.Id == item.ticket_Id).FirstOrDefault();
                            if (data != null)
                            {
                                if (data.IsCheckIn != true)
                                {
                                    data.IsCheckIn = true;
                                    if (item.update_date != null)
                                    {
                                        data.UpdateDate = item.update_date;
                                    }
                                    else
                                    {
                                        data.UpdateDate = DateTime.UtcNow;
                                    }

                                    dbcon.SaveChanges();
                                }
                            }
                        }

                        return "true";

                    }

                }
                catch (Exception ex)
                {

                    ExceptionHandler.LogException(ex, "ScanNPassAPIController", "getLocalData");
                }
            }
            return false;

        }
        
        public bool UpdateEvevntSubscription(string EventId, string SubscriptionType, int paymId)
        {
            try
            {
                ApiResponse Resp = new ApiResponse();
                if (!string.IsNullOrEmpty(EventId) && !string.IsNullOrEmpty(SubscriptionType))
                {
                    Resp = Areas.Organizer.Models.OrganizerDbOperation.UpdateSubscription(Convert.ToInt32(EventId), Convert.ToInt32(SubscriptionType), paymId);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }
        
        private async Task<String> getRawPostData()
        {
            using (var contentStream = await this.Request.Content.ReadAsStreamAsync())
            {
                contentStream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(contentStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        //[HttpPost]
        public HttpResponseMessage PayStackWebhookRequestResponse()
        {
            try
            {
                var contentStreams = Request.Content.ReadAsByteArrayAsync();
                //contentStream.Seek(0, SeekOrigin.Begin);
                string rawData = getRawPostData().Result;
                PayStackWebHookRoot hook = new PayStackWebHookRoot();
                Log4Net.Error("WebHook response  :  " + rawData);
                hook = JsonConvert.DeserializeObject<PayStackWebHookRoot>(rawData);
                // log it or whatever
                if (hook != null && hook.data != null)
                {
                
                   EventmanagerEntities dbConn = new EventmanagerEntities();
                    var paym = (from pd in dbConn.Payments
                                join od in dbConn.Events on pd.Id equals od.PaymetIdSubscription
                                where pd.TransactionId == hook.data.reference
                                select new PaymentStatuss
                                {
                                    PaymentId = pd.Id,
                                    eventId = od.Id,
                                    substype = od.SubscriptionType.ToString()
                                }).FirstOrDefault();
                    
                    if (hook.data.status == "success")
                    {
                        if(paym != null) { 
                        var updatepay = PaymentLogic.UpdatePaymentStatus(paym.PaymentId, "SUCCESSFUL", hook.data.reference, rawData.ToString());
                        paym.status = "SUCCESSFUL";
                        if (paym.Page == "Subscription")
                        {
                            var update = UpdateEvevntSubscription(paym.eventId.ToString(), paym.substype, paym.PaymentId);
                            
                        }
                        else
                        {
                            var resp = PaymentLogic.Addtowallet(paym.PaymentId, paym.eventId);
                            
                        }
                        }
                        else { Log4Net.Error("WebHook response not updated :  " + hook.data.reference); }
                    }
                    else
                    {
                        if (paym != null) { 
                        string stats = "Failed";
                        if (paym.status == "pending") { stats = "PENDING"; }
                        var updatepay = PaymentLogic.UpdatePaymentStatus(paym.PaymentId, stats, hook.data.reference, rawData.ToString());
                        }
                        else { Log4Net.Error("WebHook response not updated :  " + hook.data.reference); }
                    }
                    
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            
        }        
    }
}