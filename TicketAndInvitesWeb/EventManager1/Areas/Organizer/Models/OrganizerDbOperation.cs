using EventManager1.DBCon;
using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using EventManager1.Resource;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EventManager1.Areas.Organizer.Models
{
    public class OrganizerDbOperation
    {
        public static List<TicketsModelView> GetTickets(int Id)
        {
            List<TicketsModelView> result = new List<TicketsModelView>();
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                dbConn.Configuration.LazyLoadingEnabled = false;
                try
                {


                    var Tickets = dbConn.EventTickets.Include(b => b.Event).Where(a => a.Event_Id == Id).ToList();
                    var ttype = dbConn.TicketTypes.ToList();
                    if (Tickets != null && Tickets.Count > 0)
                    {
                        result = (from r in Tickets
                                  select new TicketsModelView
                                  {
                                      Id = r.Id,
                                      EventName = r.Event.Event_name,
                                      Venuname = r.Event.Venue,
                                      StartDate = r.Event.StartDate,
                                      TicketType = r.Ticket_Type.ToString(),
                                      TicketName = r.TicketName,
                                      Table = r.tableNo,
                                      Seat = r.Seat,
                                      Colorarea = r.ColorArea,
                                      //Colorarea=r.,
                                      Price = r.Price,
                                      Quantity = r.Quantity,
                                      isEnabled = r.IsEnable,
                                  }).ToList();
                    }
                    else
                    {
                        var events = dbConn.Events.Where(a => a.Id == Id).FirstOrDefault();
                        result.Add(new Models.TicketsModelView { EventName = events.Event_name, StartDate = events.StartDate, Venuname = events.Venue });
                    }

                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }
        public static ExceldataModelView GetBroadcastList(int EventId)
        {
            ExceldataModelView result = new ExceldataModelView();
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                dbConn.Configuration.LazyLoadingEnabled = false;
                try
                {

                    var Broadcastt = dbConn.Broadcasts.Include(a => a.Payment).Where(b => b.Event_Id == EventId).ToList();
                    if (Broadcastt != null)
                    {
                        var results = (from r in Broadcastt
                                       where r.Payment != null
                                       orderby r.Created_at descending
                                       select new EmailList
                                       {
                                           Mobile = r.MobileNumber,
                                           EmailAddress = r.EmailAddress,
                                           Status = GetBroadcastStatusText(r.Payment.Status, r.IsMailSend)
                                       }).ToList();
                        result.EmailList = results;
                        result.TotalAmount = (results.Count() * dbConn.PaymentSetups.FirstOrDefault().Broadcast).ToString();

                    }
                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }
        public static string GetBroadcastStatusText(int paymentsts, bool? issend)
        {
            if (paymentsts == (int)PaymentStatus.PaymentSuccess && issend == true)
            {
                return "email send successfully";
            }
            else if (paymentsts == (int)PaymentStatus.PaymentSuccess && issend == false)
            {
                return "email in queue";
            }
            else if (paymentsts == (int)PaymentStatus.PaymentPending)
            {
                return "waiting for payment confirmation";
            }
            else
            {
                return "payment failed for this broadcast";
            }
        }

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
                        invitation = dbConn.Invitations.Include(a => a.Payment).Include(a => a.Event).Include(a => a.EventTickets).Where(b => b.Event_Id == EventId && b.Event.CreatedOnWebsite == (int)Ticketforwebsite.Stream233).OrderByDescending(a => a.Created_at).ToList();
                    }
                    else
                    {
                        invitation = dbConn.Invitations.Include(a => a.Payment).Include(a => a.Event).Include(a => a.EventTickets).Where(b => b.CompanyId == ManageSession.CompanySession.Id && b.Event.CreatedOnWebsite == (int)Ticketforwebsite.Stream233).OrderByDescending(a => a.Created_at).ToList();
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
                                      Remark = GetInvitationStatus(r.Payment.Status, r.SendType, r.IsMailSend, r.IsSmsSend)
                                  }).ToList();
                    }
                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }
        public static string GetInvitationStatus(int PmStatus, int? Invitationtype, bool? isemailsend, bool? ismssend)
        {
            if (PmStatus == (int)PaymentStatus.PaymentSuccess)
            {
                string final = string.Empty;
                if (Invitationtype == (int)SendInvitationType.Email)
                {
                    final = isemailsend == true ? "Email send to User" : "Email in Queue";
                }
                else if (Invitationtype == (int)SendInvitationType.SMS)
                {
                    final = ismssend == true ? "Sms send to User" : "SMS in Queue";
                }
                else
                {//elase both
                    final = isemailsend == true ? "Email send to User" : "Email in Queue";
                    final = final + " , " + (ismssend == true ? "SMS send to User" : "SMS in Queue");
                }
                return final;
            }
            else if (PmStatus == (int)PaymentStatus.PaymentPending)
            {
                return "Pending";
            }
            else
            {
                return "Invitation not send due to Payment Fail";
            }
        }
        public static List<TransactionHistory> GetSales(int EventId)
        {
            List<TransactionHistory> result = new List<TransactionHistory>();
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                dbConn.Configuration.LazyLoadingEnabled = false;
                try
                {
                    var eve = dbConn.Events.Include("EventTickets").Where(x => x.Id == EventId).FirstOrDefault();
                    var adminfee = dbConn.PaymentSetups.FirstOrDefault().Adminfee;
                    var tickk = eve.EventTickets.ToList();
                    foreach (var j in tickk)
                    {
                        if (j.Ticket_Type != 2)
                        {
                            var payments = dbConn.TickeUserMaps.Where(y => y.TicketId == j.Id).ToList();
                            var tic = payments.Where(y => (y.Status == 1 || y.Status == 2) && y.Qty > 0 && y.Amount > 0).ToList();
                            if (tic.Count() > 0)
                            {
                                double total = j.Quantity ?? 0;
                                foreach (var k in tic)
                                {
                                    var totalearn = k.Qty * k.Amount;
                                    var comition = Math.Round(Convert.ToDecimal(totalearn * adminfee / 100), 2);
                                    var earned = totalearn - comition;
                                    result.Add(new TransactionHistory { Date = Convert.ToDateTime(k.CreateDate).ToString("dd/MM/yyyy "), Total = total.ToString(), EventDate = Convert.ToDateTime(eve.StartDate).ToString("dd/MM/yyyy hh:mmtt"), Eventname = eve.Event_name, Booked = k.Qty.ToString(), GrossTotal = totalearn.ToString(), Transactioncosts = comition.ToString(), TotalEarned = earned.ToString() });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }
        public static List<Dropdownlist> GetTicketType(int EventId)
        {
            try
            {
                EventmanagerEntities Db = new EventmanagerEntities();
                List<Dropdownlist> result = (from r in Db.EventTickets
                                             where r.Event_Id == EventId && r.Price > 0
                                             select new Dropdownlist
                                             {
                                                 Id = r.Id,
                                                 Text = r.TicketName
                                             }).ToList();
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        //public static string GetTicketType(int? value)
        //{
        //    string str = "";
        //    try
        //    {
        //        //if (Enum.IsDefined(typeof(TicketType), value))
        //        //    str = ((TicketType)value).ToString();
        //        //else
        //        //    str = "";
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return str;
        //}
        public static List<Dropdownlist> GetEventName(string prefix, int allBroad = 0)
        {

            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    if (allBroad > 0)
                    {
                        List<Dropdownlist> result = (from r in dbConn.Events
                                                     join e in dbConn.Broadcasts on r.Id equals e.Event_Id
                                                     where r.Company_Id == ManageSession.CompanySession.Id
                                                     && r.CreatedOnWebsite == (int)Ticketforwebsite.Stream233
                                                     select new Dropdownlist
                                                     {
                                                         Id = r.Id,
                                                         Text = r.Event_name
                                                     }).Distinct().ToList();
                        return result;
                    }
                    else if (!string.IsNullOrEmpty(prefix))
                    {
                        List<Dropdownlist> result = (from r in dbConn.Events
                                                     where r.Event_name.ToLower().Contains(prefix.ToLower()) && r.Company_Id == ManageSession.CompanySession.Id && r.Status == (int)EventStatus.Pending && r.CreatedOnWebsite == (int)Ticketforwebsite.Stream233
                                                     select new Dropdownlist
                                                     {
                                                         Id = r.Id,
                                                         Text = r.Event_name
                                                     }).ToList();
                        return result;
                    }
                    else
                    {
                        List<Dropdownlist> result = (from r in dbConn.Events
                                                     where r.Company_Id == ManageSession.CompanySession.Id && (r.Status > (int)EventStatus.Pending || r.Status == (int)EventStatus.Active) && r.EndDate >= DateTime.UtcNow && r.CreatedOnWebsite == (int)Ticketforwebsite.Stream233
                                                     select new Dropdownlist
                                                     {
                                                         Id = r.Id,
                                                         Text = r.Event_name
                                                     }).ToList();
                        return result;
                    }



                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public static List<SubcriptionView> GetSubcriptionHistory(int EventId)
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                List<SubcriptionView> result = new List<SubcriptionView>();
                dbConn.Configuration.LazyLoadingEnabled = false;
                try
                {
                    List<Event> events = new List<Event>();
                    if (EventId > 0)
                    {
                        events = dbConn.Events.Include(a => a.Payment).Where(a => a.Id == EventId).OrderByDescending(a => a.Created_at).ToList();
                    }
                    else
                    {
                        events = dbConn.Events.Include(a => a.Payment).Where(a => a.PaymetIdSubscription != null && a.PaymetIdSubscription > 0 && a.Company_Id == ManageSession.CompanySession.Id).OrderByDescending(a => a.Created_at).ToList();
                    }
                    if (events != null)
                    {
                        result = (from r in events
                                  select new SubcriptionView
                                  {
                                      eventname = r.Event_name,
                                      subcriptiontype = r.SubscriptionType,
                                      status = r.Payment.Status == (int)PaymentStatus.PaymentSuccess ? "Subcribption Approved" : r.Payment.Status == (int)PaymentStatus.PaymentPending ? "Payment is Pending" : "Payment Failed"
                                  }).ToList();
                    }
                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }

        public static ApiResponse CloneEvent(int EventId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    var events = dbConn.Events.Include(a => a.MultimediaContents).Include(a => a.RSVPs).Where(a => a.Id == EventId).FirstOrDefault();
                    if (events != null)
                    {
                        Event obj = new Event();
                        obj.Event_name = events.Event_name;
                        obj.Location = events.Location;
                        obj.Direction = events.Direction;
                        obj.GPS_Location = events.GPS_Location;
                        obj.Address_Id = events.Address_Id;
                        obj.Description = events.Description;
                        obj.longitude = events.longitude;
                        obj.latitude = events.latitude;
                        obj.StartDate = events.StartDate;
                        obj.EndDate = events.EndDate;
                        obj.Created_at = events.Created_at;
                        obj.Updated_at = events.Updated_at;
                        obj.Venue = events.Venue;
                        obj.Company_Id = events.Company_Id;
                        obj.User_Id = events.User_Id;
                        obj.SubscriptionType = null;
                        obj.Status = null;
                        obj.PublishDate = DateTime.UtcNow;

                        if (events.RSVPs != null && events.RSVPs.Count > 0)
                        {
                            foreach (var rsvp in events.RSVPs)
                            {
                                obj.RSVPs.Add(new RSVP { Namer = rsvp.Namer, Phone = rsvp.Phone });
                            }
                        }
                        if (events.MultimediaContents != null && events.MultimediaContents.Count > 0)
                        {
                            foreach (var mul in events.MultimediaContents)
                            {
                                obj.MultimediaContents.Add(new MultimediaContent
                                {
                                    Description = mul.Description,
                                    URL = mul.URL,
                                    Mul_Type = mul.Mul_Type,
                                    Mul_MainPic = mul.Mul_MainPic,
                                    Created_at = DateTime.UtcNow,
                                    Updated_at = DateTime.UtcNow
                                });
                            }
                        }
                        dbConn.Events.Add(obj);
                        dbConn.SaveChanges();
                        resp.Code = (int)ApiResponseCode.ok;
                        return resp;
                    }


                }
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return resp;
        }
        public static List<Dropdownlist> GetEventNamebyUser(int Company_Id)
        {

            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    List<Dropdownlist> result = (from r in dbConn.Events
                                                 where r.Company_Id == Company_Id && r.Status == (int)EventStatus.Active
                                                 select new Dropdownlist
                                                 {
                                                     Id = r.Id,
                                                     Text = r.Event_name
                                                 }).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public static List<Dropdownlist> GetEventNamebyUsers(int Company_Id)
        {

            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    List<Dropdownlist> result = (from r in dbConn.Events
                                                 where r.User_Id == Company_Id && r.Status == (int)EventStatus.Active
                                                 select new Dropdownlist
                                                 {
                                                     Id = r.Id,
                                                     Text = r.Event_name
                                                 }).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public static ExceldataModelView GetExceldata(int Id)
        {
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    ExceldataModelView result = (from r in dbConn.Events
                                                 where r.Id == Id
                                                 select new ExceldataModelView
                                                 {
                                                     EventName = r.Event_name,
                                                     Venue = r.Venue,
                                                     Datetime = r.StartDate
                                                 }).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public static List<InvitationDetail> GetUserbyEvent(int Event_Id)
        {
            List<InvitationDetail> result = new List<InvitationDetail>();
            try
            {

                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    //get user from invitation list
                    result = (from r in dbConn.Invitations
                              where r.Event_Id == Event_Id
                              select new InvitationDetail
                              {
                                  Title = r.Title,
                                  FirstName = r.FirstName,
                                  LastName = r.LastName,
                                  EmailAddress = r.EmailAddress,
                                  MobileNumber = r.MobileNumber,
                                  //SeatNumber = r.EventTicket != null ? r.EventTicket.Seat : "",
                                  //TableNumber = r.EventTicket != null ? r.EventTicket.tableNo : "",
                                  //ColorCode = r.EventTicket != null ? r.EventTicket.ColorArea : "",
                              }).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public static List<OverviewEventModel> GetEventOverviewTickets(string Event_Id, int CompnayId)
        {
            List<OverviewEventModel> result = new List<OverviewEventModel>();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    //get user from invitation list
                    List<Event> events = new List<Event>();
                    if (!string.IsNullOrEmpty(Event_Id))
                    {
                        int id = Convert.ToInt32(Event_Id);
                        events = dbConn.Events.Include(a => a.EventTickets).Where(a => a.Id == id).ToList();//a => a.StartDate >= DateTime.UtcNow    && a.Status == (int)EventStatus.Active
                    }
                    else
                    {
                        events = dbConn.Events.Include(a => a.EventTickets).Where(a => a.Company_Id == ManageSession.CompanySession.Id).ToList(); //a => a.StartDate >= DateTime.UtcNow  &&&& a.Status == (int)EventStatus.Active
                    }
                    var tticket = dbConn.TicketTypes.ToList();

                    if (events != null && events.Count() > 0)
                    {
                        result = (from r in events
                                  where r.Status > 0
                                  select new OverviewEventModel
                                  {
                                      Venuename = r.Venue,
                                      EventName = r.Event_name,
                                      Startdate = r.StartDate,
                                      Id = r.Id,
                                      Tickets = (from t in r.EventTickets
                                                 where t.Inviation_Id == null
                                                 select new OverviewModel
                                                 {
                                                     TickeType = tticket.FirstOrDefault(m => m.id == Convert.ToInt32(t.Ticket_Type)) != null ? tticket.FirstOrDefault(m => m.id == Convert.ToInt32(t.Ticket_Type)).TicketTypes : "", //GetTicketType(t.Ticket_Type),
                                                     TicketName = t.TicketName,
                                                     SoldTicket = dbConn.TickeUserMaps.Where(x => x.TicketId == t.Id && x.Status == 1).Count(),
                                                     Quantity = t.Quantity,
                                                     TicketPrice = t.Price
                                                 }).ToList()
                                  }).ToList();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public static ApiResponse UpdatePassword(string passowrd, string Oldpassword, int CompanyId)
        {
            ApiResponse Resp = new ApiResponse();
            try
            {
                Resp.Code = (int)ApiResponseCode.fail;
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var result = dbConn.Companies.Where(a => a.Id == CompanyId).FirstOrDefault();
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
            catch (Exception ex)
            {
                Resp.Msg = ex.Message;
            }
            return Resp;
        }
        public static ApiResponse UpdateDetail(string email, string compnay, int CompanyId)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                var result = dbConn.Companies.Where(a => a.Id == CompanyId).FirstOrDefault();
                if (result != null)
                {
                    result.Business_Email_address = email;
                    result.Name_of_business = compnay;
                    dbConn.SaveChanges();
                    Resp.Code = (int)ApiResponseCode.ok;
                    ManageSession.CompanySession.EmailId = email;
                    ManageSession.CompanySession.CompName = compnay;
                    return Resp;
                }
                else
                {
                    Resp.Msg = ApplicationStrings.Somethingwrmg;
                }
            }
            return Resp;
        }

        public static ApiResponse UpdateCompanyDetail(string txtemail, string txtcmname, string txtnumber, string txtaddress, string CountryId)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                var result = dbConn.Companies.Include(a => a.Address).Where(a => a.Id == ManageSession.CompanySession.Id).FirstOrDefault();
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(txtemail))
                    {
                        result.Business_Email_address = txtemail;
                    }
                    if (!string.IsNullOrEmpty(txtcmname))
                    {
                        result.Name_of_business = txtcmname;
                    }
                    if (!string.IsNullOrEmpty(txtnumber))
                    {
                        result.Business_contact_number = txtnumber;
                    }
                    if (!string.IsNullOrEmpty(txtaddress))
                    {

                        if (result.Address != null)
                        {
                            result.Address.AddressLine = txtaddress;
                        }
                        else
                        {
                            result.Address = new Address();
                            result.Address.AddressLine = txtaddress;
                        }
                    }
                    if (!string.IsNullOrEmpty(CountryId))
                    {
                        result.Country_Id = Convert.ToInt32(CountryId);
                    }
                    dbConn.SaveChanges();
                    Resp.Code = (int)ApiResponseCode.ok;
                    ManageSession.CompanySession.EmailId = txtemail;
                    ManageSession.CompanySession.CompName = txtcmname;
                    return Resp;
                }
                else
                {
                    Resp.Msg = ApplicationStrings.Somethingwrmg;
                }
            }
            return Resp;
        }

        public static ApiResponse UpdateAddBankDetail(BankDetails BankDetails)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                var details = dbConn.BankDetalMappings.Where(a => a.Company_Id == ManageSession.CompanySession.Id).FirstOrDefault();

                if (BankDetails != null && BankDetails.BankId > 0)
                {
                    if (!string.IsNullOrEmpty(BankDetails.Account_Number) && !string.IsNullOrEmpty(BankDetails.Account_Holder_Name) && !string.IsNullOrEmpty(BankDetails.Bank_Registration_Number))
                    {
                        if (details != null)
                        {
                            details.BankDetail_Id = BankDetails.BankId;
                            details.AccountNumber = BankDetails.Account_Number;
                            details.AccountHolder = BankDetails.Account_Holder_Name;
                            details.BankRegNo = BankDetails.Bank_Registration_Number;
                        }
                        else
                        {
                            details = new BankDetalMapping();
                            details.BankDetail_Id = BankDetails.BankId;
                            details.AccountNumber = BankDetails.Account_Number;
                            details.AccountHolder = BankDetails.Account_Holder_Name;
                            details.BankRegNo = BankDetails.Bank_Registration_Number;
                            details.CreatedDate = DateTime.Now;
                            details.Company_Id = ManageSession.CompanySession.Id;
                            dbConn.BankDetalMappings.Add(details);
                        }
                    }
                }
                if (BankDetails != null && BankDetails.MobileAccountId > 0)
                {
                    if (!string.IsNullOrEmpty(BankDetails.mobile_money_UniqueId))
                    {
                        if (details != null)
                        {
                            details.MobileMoneyDetail_Id = BankDetails.MobileAccountId;
                            details.Mobile_Money_Unique = BankDetails.mobile_money_UniqueId;

                        }
                        else
                        {
                            details = new BankDetalMapping();
                            details.MobileMoneyDetail_Id = BankDetails.MobileAccountId;
                            details.Mobile_Money_Unique = BankDetails.mobile_money_UniqueId;
                            details.CreatedDate = DateTime.Now;
                            details.Company_Id = ManageSession.CompanySession.Id;
                            dbConn.BankDetalMappings.Add(details);
                        }
                    }

                }
                dbConn.SaveChanges();
                Resp.Code = (int)ApiResponseCode.ok;
            }
            return Resp;
        }

        public static CompnayProfile GetComnayProfile(int CompnayId)
        {
            CompnayProfile result = new CompnayProfile();
            try
            {
                result.BankDetails = new BankDetails();

                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    result.Banks = (from bk in dbConn.BankDetails
                                    where bk.Type == 1 //for bank
                                    select new Dropdownlist
                                    {
                                        Id = bk.Id,
                                        Text = bk.BankName
                                    }).ToList();
                    result.MobileWallet = (from bk in dbConn.BankDetails
                                           where bk.Type == 2 //for mobile
                                           select new Dropdownlist
                                           {
                                               Id = bk.Id,
                                               Text = bk.BankName
                                           }).ToList();
                    result.Country = (from bk in dbConn.Countries
                                      select new Dropdownlist
                                      {
                                          Id = bk.Id,
                                          Text = bk.Name
                                      }).ToList();
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    var companyprofile = (from p in dbConn.Companies.Include(a => a.Address).Include(a => a.BankDetalMappings)
                                          where p.Id == CompnayId
                                          select p).FirstOrDefault();
                    result.Email = companyprofile.Business_Email_address;
                    result.CompnayName = companyprofile.Name_of_business;
                    result.CountryId = companyprofile.Country_Id;
                    result.Contact_Number = companyprofile.Business_contact_number;
                    if (companyprofile.Address != null)
                    {
                        result.AddressId = companyprofile.Address.Id;
                        result.Address = companyprofile.Address.AddressLine;

                    }
                    result.BankDetails = new BankDetails();
                    if (companyprofile.BankDetalMappings != null && companyprofile.BankDetalMappings.Count() > 0)
                    {
                        var detail = companyprofile.BankDetalMappings.FirstOrDefault();
                        result.BankDetails.Account_Holder_Name = detail.AccountHolder;
                        result.BankDetails.Account_Number = detail.AccountNumber;
                        result.BankDetails.Bank_Registration_Number = detail.BankRegNo;
                        result.BankDetails.mobile_money_UniqueId = detail.Mobile_Money_Unique;
                        var bankdetail = dbConn.BankDetails.Where(a => a.Id == detail.BankDetail_Id).FirstOrDefault();
                        if (bankdetail != null)
                        {
                            result.BankDetails.BankId = bankdetail.Id;
                            result.BankDetails.MobileAccountId = detail.MobileMoneyDetail_Id;
                        }
                        if (detail.MobileMoneyDetail_Id != null)
                        {
                            result.BankDetails.MobileAccountId = detail.MobileMoneyDetail_Id;
                            result.BankDetails.mobile_money_UniqueId = detail.Mobile_Money_Unique;
                        }

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        #region Usermanagament
        public static List<ObjectMasterModel> GetObjectmaster()
        {

            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    //cmpny
                    var objectmaster = (from r in dbConn.ObjectMasters
                                        where r.ModuleType == "cmpny"
                                        select new ObjectMasterModel
                                        {
                                            Id = r.Id,
                                            Name = r.ObjectName,
                                            EC = true,
                                            Watch = true,
                                            Delete = true
                                        }).ToList();

                    return objectmaster;

                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public static decimal? GetwithdrawalAmount()
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                decimal? wamount = 0;
                try
                {
                    wamount = dbConn.Wallets.OrderByDescending(x => x.Id).FirstOrDefault(x => x.CompanyId == ManageSession.CompanySession.Id).Balance;
                    var reqamt = dbConn.WithdrawReqs.Where(x => x.CompanyId == ManageSession.CompanySession.Id).ToList();
                    var PendingtotalReq = reqamt.Where(x => x.Status != 1).Sum(x => x.RequestAmount);
                    wamount = wamount - PendingtotalReq;
                }
                catch { }

                return wamount;
            }
        }
        public static int GetbankDetails()
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                // 1 details complete, 2 mobile money a
                try
                {
                    var acc = dbConn.BankDetalMappings.FirstOrDefault(x => x.Company_Id == ManageSession.CompanySession.Id);
                    if (acc != null)
                    {
                        if (acc.Mobile_Money_Unique != null)
                        {
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    return 3;
                }
                catch { return 3; }


            }
        }
        public static decimal Getminamount(decimal amount)
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                // 1 details complete, 2 mobile money a
                try
                {
                    var amt = Convert.ToDecimal(dbConn.AdminSetups.FirstOrDefault().AutoTransferMinAmount);
                    return amt;
                }
                catch { return 0; }


            }
        }

        public static WithdrawalModel Getwithdrawals(int type, string startdate, string enddate)
        {
            WithdrawalModel res = new WithdrawalModel();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    int days = -7;
                    try
                    {
                        List<WithdrawModels> list = new List<WithdrawModels>();
                        if (type != 3)
                        {
                            switch (type)
                            {
                                case 0:
                                    days = -7;
                                    break;
                                case 1:
                                    days = -30;
                                    break;
                                case 2:
                                    days = -365;
                                    break;
                                default:
                                    days = -7;
                                    break;
                            }
                            var sdate = DateTime.UtcNow.AddDays(days);

                            res.walletBalance = dbConn.Wallets.OrderByDescending(x => x.Id).FirstOrDefault(y => y.CompanyId == ManageSession.CompanySession.CompanyId).Balance;
                            var result = dbConn.WithdrawReqs.Where(x => x.CompanyId == ManageSession.CompanySession.CompanyId && x.RequestDate >= sdate && x.Status != 2).ToList();
                            res.PendingtotalReq = result.Where(x => x.Status != 1).Sum(x => x.RequestAmount);
                            res.Availablebalance = res.walletBalance - res.PendingtotalReq;

                            foreach (var i in result)
                            {
                                if (i.withdrawDate != null)
                                {
                                    //var pay = dbConn.Payments.FirstOrDefault(x => x.Id == i.PaymentId);                                
                                    var trx = dbConn.Payments.FirstOrDefault(x => x.Id == i.PaymentId);
                                    if (trx.Status == 1)
                                    {
                                        list.Add(new WithdrawModels { Id = i.Id, Amount = i.RequestAmount, ReqDate = i.RequestDate, Status = i.Status, TransactionId = trx.TransactionId, TransDate = i.withdrawDate });
                                    }
                                    else { list.Add(new WithdrawModels { Id = i.Id, Amount = i.RequestAmount, ReqDate = i.RequestDate, Status = i.Status }); }
                                }
                                else { list.Add(new WithdrawModels { Id = i.Id, Amount = i.RequestAmount, ReqDate = i.RequestDate, Status = i.Status }); }
                            }
                            res.Withdraw = list;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(startdate))
                            {
                                DateTime to = Convert.ToDateTime(startdate);
                                DateTime end = Convert.ToDateTime(enddate);
                                res.walletBalance = dbConn.Wallets.OrderByDescending(x => x.Id).FirstOrDefault(y => y.CompanyId == ManageSession.CompanySession.Id).Balance;
                                var result = dbConn.WithdrawReqs.Where(x => x.CompanyId == ManageSession.CompanySession.Id && x.RequestDate >= to && x.RequestDate <= end).ToList();
                                res.PendingtotalReq = result.Where(x => x.Status != 1).Sum(x => x.RequestAmount);
                                res.Availablebalance = res.walletBalance - res.PendingtotalReq;

                                foreach (var i in result)
                                {
                                    if (i.withdrawDate != null)
                                    {
                                        //var pay = dbConn.Payments.FirstOrDefault(x => x.Id == i.PaymentId);                                
                                        var trx = dbConn.Payments.FirstOrDefault(x => x.Id == i.PaymentId);
                                        if (trx.Status == 1)
                                        {
                                            list.Add(new WithdrawModels { Id = i.Id, Amount = i.RequestAmount, ReqDate = i.RequestDate, Status = i.Status, TransactionId = trx.TransactionId, TransDate = i.withdrawDate });
                                        }
                                        else { list.Add(new WithdrawModels { Id = i.Id, Amount = i.RequestAmount, ReqDate = i.RequestDate, Status = i.Status }); }
                                    }
                                    else { list.Add(new WithdrawModels { Id = i.Id, Amount = i.RequestAmount, ReqDate = i.RequestDate, Status = i.Status }); }
                                }
                                res.Withdraw = list;
                            }

                        }
                    }
                    catch { }
                    return res;

                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public static bool reqwithdrawals(decimal amount)
        {
            WithdrawalModel res = new WithdrawalModel();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    //var count = dbConn.WithdrawReqs.OrderByDescending(y => y.Id).FirstOrDefault(x => x.CompanyId == ManageSession.CompanySession.Id && x.Status == 0);
                    //if (count == null)
                    //{
                    WithdrawReq w = new WithdrawReq();
                    w.CompanyId = ManageSession.CompanySession.Id;
                    w.RequestAmount = amount;
                    w.RequestDate = DateTime.UtcNow;
                    w.Status = 0;
                    dbConn.WithdrawReqs.Add(w);
                    dbConn.SaveChanges();
                    //}
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static bool updatereqwithdrawals(int id)
        {
            WithdrawalModel res = new WithdrawalModel();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var req = dbConn.WithdrawReqs.FirstOrDefault(x => x.Id == id);
                    req.Status = 2;
                    if (dbConn.SaveChanges() > 0)
                    { return true; }
                    else { return false; }
                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static List<WithdrawModel> Getwithdrawal()
        {
            List<WithdrawModel> result = new List<WithdrawModel>();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var events = dbConn.Events.Include("EventTickets").Where(x => x.Company_Id == ManageSession.CompanySession.Id);
                    var withdraw = dbConn.Withdraws.Where(x => x.CompanyId == ManageSession.CompanySession.Id);
                    foreach (var i in events)
                    {
                        var withdr = withdraw.FirstOrDefault(x => x.EventId == i.Id);
                        if (withdr != null)
                        {
                            decimal? WithdrawAmount = withdr.TotalAmount - (withdr.TotalAmount * dbConn.PaymentSetups.FirstOrDefault().Adminfee / 100);
                            result.Add(new WithdrawModel { Id = i.Id, EventName = i.Event_name, TransactionDate = withdr.PaymentDate, Status = withdr.Status, AmountCreadited = WithdrawAmount, TransactionID = withdr.TransactionId });
                        }
                        else
                        {
                            decimal? totalAmount = 0;

                            foreach (var j in i.EventTickets)
                            {
                                var tic = dbConn.TickeUserMaps.Include(y => y.Payment).Where(x => x.TicketId == j.Id && (x.Status == 2 || x.Status == 1) && x.Qty > 0).ToList();
                                var amt = tic.Select(x => x.Payment).Sum(y => y.Amount);
                                var qty = tic.Sum(y => y.Qty);
                                totalAmount = totalAmount + amt;
                            }
                            if (totalAmount > 0)
                            {
                                decimal? WithdrawAmount = totalAmount - (totalAmount * dbConn.PaymentSetups.FirstOrDefault().Adminfee / 100);
                                result.Add(new WithdrawModel { Id = i.Id, EventName = i.Event_name, Status = 0, AmountCreadited = WithdrawAmount });
                                totalAmount = 0;
                            }
                        }

                    }

                    return result;

                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        //public static List<WithdrawModel> Getwithdrawaldetails(int eventId)
        //{
        //    List<WithdrawModel> result = new List<WithdrawModel>();
        //    try
        //    {
        //        using (EventmanagerEntities dbConn = new EventmanagerEntities())
        //        {
        //            var etickets = dbConn.EventTickets.Where(x => x.Event_Id == eventId);                    
        //            foreach (var i in etickets)
        //            {

        //                        var tic = dbConn.TickeUserMaps.Where(x => x.TicketId == i.Id && (x.Status == 2 || x.Status == 1) && x.Qty > 0).ToList();
        //                        var amt = tic.Sum(y => y.ActualAmount);
        //                        var qty = tic.Sum(y => y.Qty);
        //                var totalAmount = amt * qty;

        //                    if (amt > 0)
        //                    {
        //                        decimal? WithdrawAmount = totalAmount - (totalAmount * 1 / 100);
        //                        result.Add(new WithdrawModel {TotalAmount = amt, Commition = totalAmount/100,qty= qty,Totalqty = i.Quantity,TicketType = dbConn.TicketTypes.FirstOrDefault(x=>x.id==i.Ticket_Type).TicketTypes, AmountCreadited = WithdrawAmount });
        //                        totalAmount = 0;
        //                    }
        //                }

        //            }
        //            return result;                
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}
        public static List<ObjectMasterModel> GetGroupName(int CompnayId)
        {

            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    //cmpny
                    var objectmaster = (from r in dbConn.ObjectGroups
                                        where r.CompanyId == CompnayId
                                        select new ObjectMasterModel
                                        {
                                            Id = r.Id,
                                            Name = r.Groupname
                                        }).ToList();

                    return objectmaster;

                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public static List<UserManageModel> GetUserbyAccount(int CompnayId)
        {

            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    var companis = dbConn.Companies.Include(a => a.ObjectGroup).Where(a => a.Parent_Id == CompnayId).ToList();
                    if (companis != null && companis.Count > 0)
                    {
                        //cmpny
                        var objectmaster = (from r in companis
                                            where r.Parent_Id == CompnayId
                                            select new UserManageModel
                                            {
                                                UserId = r.Id,
                                                Name = r.UserName,
                                                Email = r.Business_Email_address,
                                                AccountType = r.ObjectGroup != null ? r.ObjectGroup.Groupname : "",
                                            }).ToList();

                        return objectmaster;
                    }


                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public static List<Dropdownlist> GetAllGroups(int CompnayId)
        {

            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    //cmpny
                    var objectmaster = (from r in dbConn.ObjectGroups
                                        where r.CompanyId == CompnayId
                                        select new Dropdownlist
                                        {
                                            Id = r.Id,
                                            Text = r.Groupname
                                        }).ToList();

                    return objectmaster;

                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public static ApiResponse AddAccount(AcccountTypeViewModel Model)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    if (!string.IsNullOrEmpty(Model.AccountName) && Model.Objects != null && Model.Objects.Count() > 0)
                    {
                        ObjectGroup gp = new ObjectGroup();
                        gp.Groupname = Model.AccountName;
                        gp.CompanyId = ManageSession.CompanySession.Id;
                        gp.InsertDate = DateTime.UtcNow;
                        gp.LastChanged = DateTime.UtcNow;
                        List<GroupObjectMap> maps = new List<GroupObjectMap>();
                        foreach (var item in Model.Objects)
                        {
                            GroupObjectMap map = new GroupObjectMap();
                            map.ObjectGroup = gp;
                            map.ObjectMasterId = item.Id;
                            string right = "";
                            if (item.Watch)
                            {
                                right = "1";
                            }
                            else
                            {
                                right = "0";
                            }
                            if (item.EC)
                            {
                                right = right + "1";
                            }
                            else
                            {
                                right = right + "0";
                            }
                            if (item.Delete)
                            {
                                right = right + "1";
                            }
                            else
                            {
                                right = right + "0";
                            }
                            map.Rights = right;
                            maps.Add(map);
                        }
                        Resp.Code = (int)ApiResponseCode.ok;
                        dbConn.GroupObjectMaps.AddRange(maps);
                        dbConn.SaveChanges();
                        Resp.Msg = "Account Created successfully";
                        return Resp;
                    }
                    else
                    {
                        Resp.Msg = "please enter account name!";
                    }

                }
            }
            catch (Exception ex)
            {
                Resp.Msg = ex.Message;
            }
            return Resp;
        }

        public static void AddDefaultAccount(int UserId)
        {
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    string[] adddefault = { "Event Manager", "Marketing and Promotion Manager", "Sales Manager" };
                    foreach (var item in adddefault)
                    {
                        var checkifexist = dbConn.ObjectGroups.Where(a => a.CompanyId == UserId && a.Groupname == item).FirstOrDefault();
                        if (checkifexist == null)
                        {
                            ObjectGroup gp = new ObjectGroup();
                            gp.CompanyId = UserId;
                            gp.Groupname = item;
                            gp.InsertDate = DateTime.UtcNow;
                            gp.LastChanged = DateTime.UtcNow;
                            List<GroupObjectMap> maps = new List<GroupObjectMap>();
                            var objects = dbConn.ObjectMasters.Where(a => a.ModuleType == "cmpny").ToList();
                            foreach (var obj in objects)
                            {
                                GroupObjectMap map = new GroupObjectMap();
                                map.ObjectGroup = gp;
                                map.ObjectMaster = obj;
                                map.Rights = "111";
                                maps.Add(map);
                            }
                            dbConn.GroupObjectMaps.AddRange(maps);
                            dbConn.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static async Task<ApiResponse> Addusers(string txtname, string txtemail, int ddlaccount)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var existinguser = dbConn.Companies.Where(a => a.Business_Email_address == txtemail).FirstOrDefault();
                    if (existinguser == null)
                    {
                        var compy = dbConn.Companies.Where(a => a.Id == ManageSession.CompanySession.Id).FirstOrDefault();
                        Company cmp = new Company();
                        cmp.UserName = txtname;
                        cmp.Business_Email_address = txtemail;
                        cmp.Parent_Id = ManageSession.CompanySession.Id;
                        cmp.Name_of_business = ManageSession.CompanySession.CompName;
                        cmp.Business_contact_number = compy.Business_contact_number;
                        cmp.Country_Id = compy.Country_Id;
                        cmp.GroupId = ddlaccount;
                        Random generator = new Random();
                        string password = generator.Next(0, 999999).ToString("D6");
                        cmp.Password = password;
                        cmp.Status = (int)userstatus.active;
                        dbConn.Companies.Add(cmp);
                        Resp.Msg = "User Created Successfully!";
                        dbConn.SaveChanges();

                        string body = string.Format("Hi {0},<br><br>We received a request to Create Account " +
                            "Your account associated with this email address.<br><br>Your temporary password is:<b>{1}</b><br><br>Please open the app and sign in to your account with this temporary password. You will then be prompted to create a new secure password." +
                            "<br><br>Sincerely,<br>{2}", txtname, password, ManageSession.CompanySession.CompName);
                        await EmailSending.SendEmail(txtemail, body, ManageSession.CompanySession.CompName, "Password",1);

                    }
                    else
                    {
                        Resp.Msg = "Email is in use!";
                    }

                }
            }
            catch (Exception ex)
            {
                Resp.Msg = ex.Message;
            }
            return Resp;
        }

        public static UserManageModel GetSubUser(int CompnayId)
        {

            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    //cmpny
                    var objectmaster = (from r in dbConn.Companies.Include(a => a.ObjectGroup)
                                        where r.Id == CompnayId
                                        select new UserManageModel
                                        {
                                            UserId = r.Id,
                                            Name = r.UserName,
                                            Email = r.Business_Email_address,
                                            AccountTypeId = r.ObjectGroup.Id,
                                            AccountType = r.ObjectGroup.Groupname
                                        }).FirstOrDefault();

                    return objectmaster;

                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public static ApiResponse UpdateSubUser(int CompnayId, string email, int GpId, string name)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    //cmpny

                    var compy = dbConn.Companies.Where(a => a.Id == CompnayId).FirstOrDefault();
                    if (compy != null)
                    {
                        compy.UserName = name;
                        compy.GroupId = GpId;
                        compy.Business_Email_address = email;
                        dbConn.SaveChanges();
                        resp.Code = (int)ApiResponseCode.fail;
                        resp.Msg = "Successfully updated!";
                    }
                    else
                    {
                        resp.Msg = "something went wrong rry again later!";
                    }

                    return resp;

                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }

        public static ApiResponse DeleteSubUser(int CompnayId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    var compy = dbConn.Companies.Where(a => a.Id == CompnayId).FirstOrDefault();
                    if (compy != null)
                    {
                        int i = 0;
                        dbConn.Companies.Remove(compy);
                        i = dbConn.SaveChanges();
                        if (i > 0)
                        {
                            resp.Code = (int)ApiResponseCode.ok;
                            resp.Msg = "User has been deleted successfully!.";
                        }
                        else
                        {
                            resp.Code = (int)ApiResponseCode.fail;
                            resp.Msg = "Something went wrong! while deleting the user.";
                        }

                    }
                    else
                    {
                        resp.Code = (int)ApiResponseCode.fail;
                        resp.Msg = "Something went wrong! Try again later.";
                    }
                }
            }
            catch (Exception ex)
            {
                resp.Code = (int)ApiResponseCode.fail;
                resp.Msg = ex.ToString(); ;
            }
            return resp;

        }

        public static List<ObjectMasterModel> GetGroupObjectMap(int GpId)
        {

            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    //cmpny
                    var gpobjmap = dbConn.GroupObjectMaps.Include(a => a.ObjectMaster).Include(a => a.ObjectGroup).Where(a => a.GroupMasterId == GpId).ToList();
                    var objectmaster = (from r in gpobjmap
                                        select new ObjectMasterModel
                                        {
                                            Id = r.Id,
                                            Groupname = r.ObjectGroup.Groupname,
                                            Name = r.ObjectMaster.ObjectName,
                                            Watch = GetRight(r.Rights, 0),
                                            EC = GetRight(r.Rights, 1),
                                            Delete = GetRight(r.Rights, 2)
                                        }).ToList();

                    return objectmaster;

                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public static bool GetRight(string rights, int j)
        {
            try
            {
                if (!string.IsNullOrEmpty(rights))
                {
                    if (rights.Substring(j, 1) == "1")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static ApiResponse UpdateGpMappingAccount(AcccountTypeViewModel Model)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    if (!string.IsNullOrEmpty(Model.AccountName) && Model.Objects != null && Model.Objects.Count() > 0)
                    {
                        ObjectGroup gp = dbConn.ObjectGroups.Where(a => a.Id == Model.GpId).FirstOrDefault();
                        if (gp != null)
                        {
                            gp.Groupname = Model.AccountName;
                            gp.LastChanged = DateTime.UtcNow;
                        }
                        foreach (var item in Model.Objects)
                        {
                            GroupObjectMap map = dbConn.GroupObjectMaps.Where(a => a.Id == item.Id).FirstOrDefault();
                            //map.ObjectGroup = gp;
                            //map.ObjectMasterId = item.Id;
                            string right = "";
                            if (item.Watch)
                            {
                                right = "1";
                            }
                            else
                            {
                                right = "0";
                            }
                            if (item.EC)
                            {
                                right = right + "1";
                            }
                            else
                            {
                                right = right + "0";
                            }
                            if (item.Delete)
                            {
                                right = right + "1";
                            }
                            else
                            {
                                right = right + "0";
                            }
                            map.Rights = right;
                            dbConn.SaveChanges();
                        }
                        Resp.Code = (int)ApiResponseCode.ok;
                        Resp.Msg = "Account Created successfully";
                        return Resp;
                    }
                    else
                    {
                        Resp.Msg = "please enter account name!";
                    }

                }
            }
            catch (Exception ex)
            {
                Resp.Msg = ex.Message;
            }
            return Resp;
        }
        #endregion

        #region EventPromation
        public static List<Salesoverview> GetSalesOverView(int CompanyId, int type = 0, string startdate = null, string enddate = null)
        {
            List<Salesoverview> result = new List<Salesoverview>();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    //decimal? adminfee = dbConn.PaymentSetups.FirstOrDefault().Adminfee;
                    var events = dbConn.Events.Include(a => a.EventTickets).Where(a => a.Company_Id == CompanyId).ToList();
                    if (type == 0)
                    {//last week
                        foreach (var e in events.Where(e => e.EndDate >= DateTime.UtcNow.AddDays(-7)))
                        {
                            decimal? TransactionCost = 0;
                            decimal? Revenue = 0;
                            int? quantity = 0;
                            foreach (var i in e.EventTickets.Where(x => x.Price > 0))
                            {
                                try
                                {
                                    var ticusermaps = dbConn.TickeUserMaps.Include(y => y.Payment).Where(y => y.TicketId == i.Id && (y.Status == 1)).ToList();
                                    var ticusermap = ticusermaps.Where(y => y.CreateDate >= DateTime.UtcNow.AddDays(-7) && y.Amount > 0);
                                    decimal? adminfee = ticusermaps.FirstOrDefault().Payment.adminfee;
                                    var wallet = from s in ticusermap
                                                 join w in dbConn.Wallets on s.PaymemtId equals w.PaymentId
                                                 select new { adminfee = w.AdminFee };

                                    var qty = ticusermap != null ? ticusermap.Sum(x => x.Qty) : 0;
                                    //var afee = ticusermap.
                                    if (qty > 0)
                                    {
                                        quantity = quantity + qty;
                                        var pt = ticusermap.Select(x => x.Payment);
                                        var pts = pt.Select(m => new { m.Amount, m.Id, m.adminfee, m.MinimumFee }).Distinct();
                                        var pay = pt != null ? pts.Sum(x => x.Amount) : 0;
                                        var cost = (pts.FirstOrDefault().Amount * pts.FirstOrDefault().adminfee) / 100;
                                        var min = pts.FirstOrDefault(x => x.MinimumFee != null) != null ? pts.FirstOrDefault(x => x.MinimumFee != null).MinimumFee : 0;
                                        if (min > 0)
                                        {
                                            if (cost < min)
                                            {
                                                TransactionCost = TransactionCost + wallet.Sum(x => x.adminfee);
                                            }
                                            else
                                            {
                                                TransactionCost = TransactionCost + (pay * adminfee / 100);
                                            }
                                        }
                                        else
                                        {
                                            TransactionCost = TransactionCost + (pay * adminfee / 100);
                                        }
                                        Revenue = Revenue + (pay);
                                    }
                                }
                                catch { }
                            }
                            var payout = Revenue - TransactionCost;

                            if (quantity > 0 && Revenue > 0)
                            {
                                result.Add(new Salesoverview
                                {
                                    Id = e.Id,
                                    EventName = e.Event_name,
                                    TotalTicket = e.EventTickets != null ? e.EventTickets.Where(y => y.Price > 0).Select(a => a.Quantity).Sum() : 0,
                                    BookTotal = quantity,// e.EventTickets != null ? e.EventTickets.Select(a => a.AvailableQuantity).Sum() : 0,
                                    StartDate = e.StartDate,
                                    EventRevenue = Revenue,
                                    TransactionCost = TransactionCost,
                                    Payout = payout
                                });
                            }

                        }

                    }
                    else if (type == 1)
                    {

                        foreach (var e in events.Where(e => e.EndDate >= DateTime.UtcNow.AddDays(-30)))
                        {
                            decimal? TransactionCost = 0;
                            decimal? Revenue = 0;
                            int? quantity = 0;
                            foreach (var i in e.EventTickets.Where(x => x.Price > 0))
                            {
                                try
                                {
                                    var ticusermaps = dbConn.TickeUserMaps.Include(y => y.Payment).Where(y => y.TicketId == i.Id && (y.Status == 1)).ToList();
                                    decimal? adminfee = ticusermaps.FirstOrDefault().Payment.adminfee;
                                    var ticusermap = ticusermaps.Where(y => y.CreateDate >= DateTime.UtcNow.AddDays(-30));
                                    var wallet = from s in ticusermap
                                                 join w in dbConn.Wallets on s.PaymemtId equals w.PaymentId
                                                 select new { adminfee = w.AdminFee };

                                    var qty = ticusermap != null ? ticusermap.Sum(x => x.Qty) : 0;
                                    //var afee = ticusermap.
                                    if (qty > 0)
                                    {
                                        quantity = quantity + qty;
                                        var pt = ticusermap.Select(x => x.Payment);
                                        var pts = pt.Select(m => new { m.Amount, m.Id, m.adminfee, m.MinimumFee }).Distinct();
                                        var pay = pt != null ? pts.Sum(x => x.Amount) : 0;
                                        var cost = (pts.FirstOrDefault().Amount * pts.FirstOrDefault().adminfee) / 100;
                                        var min = pts.FirstOrDefault(x => x.MinimumFee != null) != null ? pts.FirstOrDefault(x => x.MinimumFee != null).MinimumFee : 0;
                                        if (min > 0)
                                        {
                                            if (cost < min)
                                            {
                                                TransactionCost = TransactionCost + wallet.Sum(x => x.adminfee);
                                            }
                                            else
                                            {
                                                TransactionCost = TransactionCost + (pay * adminfee / 100);
                                            }
                                        }
                                        else
                                        {
                                            TransactionCost = TransactionCost + (pay * adminfee / 100);
                                        }
                                        Revenue = Revenue + (pay);
                                    }
                                }
                                catch { }
                            }
                            var payout = Revenue - TransactionCost;

                            if (quantity > 0 && Revenue > 0)
                            {
                                result.Add(new Salesoverview
                                {
                                    Id = e.Id,
                                    EventName = e.Event_name,
                                    TotalTicket = e.EventTickets != null ? e.EventTickets.Where(y => y.Price > 0).Select(a => a.Quantity).Sum() : 0,
                                    BookTotal = quantity,// e.EventTickets != null ? e.EventTickets.Select(a => a.AvailableQuantity).Sum() : 0,
                                    StartDate = e.StartDate,
                                    EventRevenue = Revenue,
                                    TransactionCost = TransactionCost,
                                    Payout = payout
                                });
                            }

                        }
                    }
                    else if (type == 2)
                    {
                        foreach (var e in events.Where(e => e.EndDate >= DateTime.UtcNow.AddYears(-1)))
                        {
                            decimal? TransactionCost = 0;
                            decimal? Revenue = 0;
                            int? quantity = 0;
                            foreach (var i in e.EventTickets.Where(x => x.Price > 0))
                            {
                                try
                                {
                                    var ticusermaps = dbConn.TickeUserMaps.Include(y => y.Payment).Where(y => y.TicketId == i.Id && (y.Status == 1)).ToList();
                                    var ticusermap = ticusermaps.Where(y => y.CreateDate >= DateTime.UtcNow.AddYears(-1));
                                    decimal? adminfee = ticusermaps.FirstOrDefault().Payment.adminfee;
                                    var wallet = from s in ticusermap
                                                 join w in dbConn.Wallets on s.PaymemtId equals w.PaymentId
                                                 select new { adminfee = w.AdminFee };

                                    var qty = ticusermap != null ? ticusermap.Sum(x => x.Qty) : 0;
                                    //var afee = ticusermap.
                                    if (qty > 0)
                                    {
                                        quantity = quantity + qty;
                                        var pt = ticusermap.Select(x => x.Payment);
                                        var pts = pt.Select(m => new { m.Amount, m.Id, m.adminfee, m.MinimumFee }).Distinct();
                                        var pay = pt != null ? pts.Sum(x => x.Amount) : 0;
                                        var cost = (pts.FirstOrDefault().Amount * pts.FirstOrDefault().adminfee) / 100;
                                        var min = pts.FirstOrDefault(x => x.MinimumFee != null) != null ? pts.FirstOrDefault(x => x.MinimumFee != null).MinimumFee : 0;
                                        if (min > 0)
                                        {
                                            if (cost < min)
                                            {
                                                TransactionCost = TransactionCost + wallet.Sum(x => x.adminfee);
                                            }
                                            else
                                            {
                                                TransactionCost = TransactionCost + (pay * adminfee / 100);
                                            }
                                        }
                                        else
                                        {
                                            TransactionCost = TransactionCost + (pay * adminfee / 100);
                                        }
                                        Revenue = Revenue + (pay);
                                    }
                                }
                                catch { }
                            }
                            var payout = Revenue - TransactionCost;

                            if (quantity > 0 && Revenue > 0)
                            {
                                result.Add(new Salesoverview
                                {
                                    Id = e.Id,
                                    EventName = e.Event_name,
                                    TotalTicket = e.EventTickets != null ? e.EventTickets.Where(y => y.Price > 0).Select(a => a.Quantity).Sum() : 0,
                                    BookTotal = quantity,// e.EventTickets != null ? e.EventTickets.Select(a => a.AvailableQuantity).Sum() : 0,
                                    StartDate = e.StartDate,
                                    EventRevenue = Revenue,
                                    TransactionCost = TransactionCost,
                                    Payout = payout
                                });
                            }


                        }
                    }
                    else if (type == 3 && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(startdate))
                    {
                        DateTime to = Convert.ToDateTime(startdate);
                        DateTime end = Convert.ToDateTime(enddate).AddDays(1);


                        var aghh = events.Where(e => e.EndDate >= to && e.EndDate <= end).ToList();
                        foreach (var e in events.Where(e => e.EndDate >= to))
                        {
                            decimal? TransactionCost = 0;
                            decimal? Revenue = 0;
                            int? quantity = 0;
                            foreach (var i in e.EventTickets.Where(x => x.Price > 0))
                            {
                                //var ticusermap = dbConn.TickeUserMaps.Include(y => y.Payment).Where(y => y.TicketId == i.Id && (y.Status == 1 || y.Status == 2) && y.CreateDate >= to && y.CreateDate <= end);
                                var ticusermaps = dbConn.TickeUserMaps.Include(y => y.Payment).Where(y => y.TicketId == i.Id && (y.Status == 1)).ToList();
                                if (ticusermaps.Count > 0)
                                {
                                    try
                                    {
                                        var ticusermap = ticusermaps.Where(y => y.CreateDate >= to && y.CreateDate <= end);
                                        decimal? adminfees = ticusermaps.FirstOrDefault().Payment.adminfee;
                                        decimal? adminfee = ticusermaps.FirstOrDefault().Payment.adminfee;
                                        var wallet = from s in ticusermap
                                                     join w in dbConn.Wallets on s.PaymemtId equals w.PaymentId
                                                     select new { adminfee = w.AdminFee };

                                        var qty = ticusermap != null ? ticusermap.Sum(x => x.Qty) : 0;
                                        //var afee = ticusermap.
                                        if (qty > 0)
                                        {
                                            quantity = quantity + qty;
                                            var pt = ticusermap.Select(x => x.Payment);
                                            var pts = pt.Select(m => new { m.Amount, m.Id, m.adminfee, m.MinimumFee }).Distinct();
                                            var pay = pt != null ? pts.Sum(x => x.Amount) : 0;
                                            var cost = (pts.FirstOrDefault().Amount * pts.FirstOrDefault().adminfee) / 100;
                                            var min = pts.FirstOrDefault(x => x.MinimumFee != null) != null ? pts.FirstOrDefault(x => x.MinimumFee != null).MinimumFee : 0;
                                            if (min > 0)
                                            {
                                                if (cost < min)
                                                {
                                                    TransactionCost = TransactionCost + wallet.Sum(x => x.adminfee);
                                                }
                                                else
                                                {
                                                    TransactionCost = TransactionCost + (pay * adminfee / 100);
                                                }
                                            }
                                            else
                                            {
                                                TransactionCost = TransactionCost + (pay * adminfee / 100);
                                            }
                                            Revenue = Revenue + (pay);
                                        }
                                    }
                                    catch { }
                                }
                            }
                            var payout = Revenue - TransactionCost;

                            if (quantity > 0 && Revenue > 0)
                            {
                                result.Add(new Salesoverview
                                {
                                    Id = e.Id,
                                    EventName = e.Event_name,
                                    TotalTicket = e.EventTickets != null ? e.EventTickets.Where(y => y.Price > 0).Select(a => a.Quantity).Sum() : 0,
                                    BookTotal = quantity,// e.EventTickets != null ? e.EventTickets.Select(a => a.AvailableQuantity).Sum() : 0,
                                    StartDate = e.StartDate,
                                    EventRevenue = Revenue,
                                    TransactionCost = TransactionCost,
                                    Payout = payout
                                });
                            }
                        }
                        //DateTime to = Convert.ToDateTime(startdate);
                        //DateTime end = Convert.ToDateTime(enddate);
                        //result = (from e in events
                        //          where e.StartDate >= to && e.StartDate <= end
                        //          select new Salesoverview
                        //          {
                        //              Id = e.Id,
                        //              EventName = e.Event_name,
                        //              TotalTicket = e.EventTickets != null ? e.EventTickets.Select(a => a.Quantity).Sum() : 0,
                        //              BookTotal = e.EventTickets != null ? e.EventTickets.Select(a => a.AvailableQuantity).Sum() : 0,
                        //              StartDate = e.StartDate,
                        //              EventRevenue = 0,
                        //              TransactionCost = 0,
                        //              Payout = 0
                        //          }).ToList();
                        //customer date filter
                    }
                    else
                    {
                        //default case
                        foreach (var e in events.Where(e => e.EndDate >= DateTime.UtcNow.AddDays(-7)))
                        {
                            decimal? TransactionCost = 0;
                            decimal? Revenue = 0;
                            int? quantity = 0;
                            foreach (var i in e.EventTickets.Where(x => x.Price > 0))
                            {
                                try
                                {
                                    var ticusermaps = dbConn.TickeUserMaps.Include(y => y.Payment).Where(y => y.TicketId == i.Id && (y.Status == 1)).ToList();
                                    var ticusermap = ticusermaps.Where(y => y.CreateDate >= DateTime.UtcNow.AddDays(-7));
                                    decimal? adminfee = ticusermaps.FirstOrDefault().Payment.adminfee;
                                    var wallet = from s in ticusermap
                                                 join w in dbConn.Wallets on s.PaymemtId equals w.PaymentId
                                                 select new { adminfee = w.AdminFee };

                                    var qty = ticusermap != null ? ticusermap.Sum(x => x.Qty) : 0;
                                    //var afee = ticusermap.
                                    if (qty > 0)
                                    {
                                        quantity = quantity + qty;
                                        var pt = ticusermap.Select(x => x.Payment);
                                        var pts = pt.Select(m => new { m.Amount, m.Id, m.adminfee, m.MinimumFee }).Distinct();
                                        var pay = pt != null ? pts.Sum(x => x.Amount) : 0;
                                        var cost = (pts.FirstOrDefault().Amount * pts.FirstOrDefault().adminfee) / 100;
                                        var min = pts.FirstOrDefault(x => x.MinimumFee != null) != null ? pts.FirstOrDefault(x => x.MinimumFee != null).MinimumFee : 0;
                                        if (min > 0)
                                        {
                                            if (cost < min)
                                            {
                                                TransactionCost = TransactionCost + wallet.Sum(x => x.adminfee);
                                            }
                                            else
                                            {
                                                TransactionCost = TransactionCost + (pay * adminfee / 100);
                                            }
                                        }
                                        else
                                        {
                                            TransactionCost = TransactionCost + (pay * adminfee / 100);
                                        }
                                        Revenue = Revenue + (pay);
                                    }
                                }
                                catch { }
                            }
                            var payout = Revenue - TransactionCost;

                            if (quantity > 0 && Revenue > 0)
                            {
                                result.Add(new Salesoverview
                                {
                                    Id = e.Id,
                                    EventName = e.Event_name,
                                    TotalTicket = e.EventTickets != null ? e.EventTickets.Where(y => y.Price > 0).Select(a => a.Quantity).Sum() : 0,
                                    BookTotal = quantity,// e.EventTickets != null ? e.EventTickets.Select(a => a.AvailableQuantity).Sum() : 0,
                                    StartDate = e.StartDate,
                                    EventRevenue = Revenue,
                                    TransactionCost = TransactionCost,
                                    Payout = payout
                                });
                            }
                        }
                        //result = (from e in events
                        //          where e.StartDate >= DateTime.UtcNow.AddDays(-7) && e.StartDate <= DateTime.UtcNow
                        //          select new Salesoverview
                        //          {
                        //              Id = e.Id,
                        //              EventName = e.Event_name,
                        //              TotalTicket = e.EventTickets != null ? e.EventTickets.Select(a => a.Quantity).Sum() : 0,
                        //              BookTotal = e.EventTickets != null ? e.EventTickets.Select(a => a.AvailableQuantity).Sum() : 0,
                        //              StartDate = e.StartDate,
                        //              EventRevenue = 0,
                        //              TransactionCost = 0,
                        //              Payout = 0
                        //          }).ToList();

                    }
                }
            }
            catch (Exception ex)
            { }
            return result;
        }

        public static ApiResponse UpdateSubscription(int EventId, int SubscriptionType, int payId)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                var result = dbConn.Events.Where(a => a.Id == EventId).FirstOrDefault();
                if (result != null)
                {
                    result.SubscriptionType = SubscriptionType;
                    result.PaymetIdSubscription = payId;
                    dbConn.SaveChanges();
                    Resp.Code = (int)ApiResponseCode.ok;
                    Resp.Msg = "Subscription created successfully!";
                    return Resp;
                }
                else
                {
                    Resp.Msg = ApplicationStrings.Somethingwrmg;
                }
            }
            return Resp;
        }
        public static SubscriptionTypes getsubsctipcost()
        {
            SubscriptionTypes sub = new SubscriptionTypes();
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                var result = dbConn.PaymentSetups.FirstOrDefault();
                if (result != null)
                {
                    sub.Platinum = Convert.ToInt32(result.Platinum);
                    sub.Gold = Convert.ToInt32(result.Gold);
                    sub.Silver = Convert.ToInt32(result.Silver);
                    sub.Bronze = Convert.ToInt32(result.Bronze);
                }

            }
            return sub;
        }
        public static ApiResponse AddOffers(OffersModel OffersModel, int paymentId = 0)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    Offer off = new Offer();
                    if (OffersModel != null && OffersModel.Id > 0)
                    {
                        off = dbConn.Offers.Where(a => a.Id == OffersModel.Id).FirstOrDefault();
                        if (OffersModel.EventId > 0)
                        {
                            off.EventId = OffersModel.EventId;
                        }

                        off.StartDate = OffersModel.startdate;
                        off.EndDate = OffersModel.Enddate;
                        off.Value = OffersModel.valus;
                        off.OfferType = OffersModel.OfferType;
                        off.CoupenCode = OffersModel.CoupenCode;
                        off.Noofcoupons = OffersModel.NoofCoupans;
                        off.PaymentID = paymentId;
                        off.OfferPageCategory = OffersModel.OfferPageCategory;
                        off.OfferForwebSite = (int)Offerforwebsite.Stream233;
                        off.TicketType = OffersModel.TicketType;
                        dbConn.SaveChanges();
                        Resp.Code = (int)ApiResponseCode.ok;
                        Resp.Msg = "Offer Added Successfully!";



                    }
                    else
                    {
                        if (OffersModel.EventId > 0)
                        {
                            off.EventId = OffersModel.EventId;
                        }

                        off.StartDate = OffersModel.startdate;
                        off.EndDate = OffersModel.Enddate;
                        off.Value = OffersModel.valus;
                        off.OfferType = OffersModel.OfferType;
                        off.IsDeleted = false;
                        off.CompanyId = ManageSession.CompanySession.Id;
                        //off.CoupenCode = RandomString(6);
                        off.CoupenCode = OffersModel.CoupenCode;
                        off.Noofcoupons = OffersModel.NoofCoupans;
                        off.PaymentID = paymentId;
                        off.OfferPageCategory = OffersModel.OfferPageCategory;
                        off.OfferForwebSite = (int)Offerforwebsite.Stream233;
                        off.TicketType = OffersModel.TicketType;
                        dbConn.Offers.Add(off);
                        dbConn.SaveChanges();
                        Resp.Code = (int)ApiResponseCode.ok;
                        Resp.Msg = "Offer Added Successfully!";



                    }
                }
            }
            catch (Exception ex)
            {
                Resp.Msg = ex.Message;
            }
            return Resp;
        }

        public static List<ExistingOffers> GetOffers()
        {
            List<ExistingOffers> Resp = new List<ExistingOffers>();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    Resp = (from r in dbConn.Offers
                            join p in dbConn.Payments.Where(c => c.Status == 1) on r.PaymentID equals p.Id
                            into cn
                            from coupon in cn.DefaultIfEmpty()
                            where r.CompanyId == ManageSession.CompanySession.Id && r.IsDeleted != true && r.OfferForwebSite == (int)Offerforwebsite.Stream233
                            select new ExistingOffers
                            {
                                Id = r.Id,
                                EventName = dbConn.Events.FirstOrDefault(m => m.Id == r.EventId).Event_name,
                                Offertype = r.OfferType,
                                coupencode = r.CoupenCode,
                                Noofcoupons = r.Noofcoupons,
                                startdate = r.StartDate,
                                enddate = r.EndDate,
                                discount = r.Value,
                                OfferPageCategory = r.OfferPageCategory,
                                TicketType = r.TicketType,
                                NoOfUsedCoupon = dbConn.TickeUserMaps.Where(x => x.OfferCode == r.Id && x.Status == 1).Count()
                            }).ToList();

                }
            }
            catch (Exception ex)
            {
            }
            return Resp;
        }
        public static OffersModel GetOffersById(int OfferId)
        {
            OffersModel Resp = new OffersModel();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    Resp = (from r in dbConn.Offers
                            where r.Id == OfferId && r.OfferForwebSite == (int)Offerforwebsite.Stream233
                            select new OffersModel
                            {
                                Id = r.Id,
                                OfferType = r.OfferType,
                                startdate = r.StartDate,
                                Enddate = r.EndDate,
                                valus = r.Value,
                                EventId = r.EventId,
                                CoupenCode = r.CoupenCode
                            }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
            }
            return Resp;
        }
        public static List<CouponModel> GetCoupons(int OfferId)
        {
            List<CouponModel> Resp = new List<CouponModel>();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var offers = dbConn.Coupons.Where(x => x.OfferId == OfferId);
                    if (offers != null && offers.Count() > 0)
                    {

                    }
                    else
                    {
                        var offer = dbConn.Offers.FirstOrDefault(x => x.Id == OfferId);
                        List<Coupon> respp = new List<Coupon>();
                        for (int i = 1; i <= offer.Noofcoupons; i++)
                        {
                            respp.Add(new Coupon { OfferId = OfferId, CoupanCode = (offer.CoupenCode + i.ToString()), OfferForwebSite = (int)Offerforwebsite.Stream233 });
                        }
                        dbConn.Coupons.AddRange(respp);
                        dbConn.SaveChanges();
                    }
                    Resp = (from r in dbConn.Coupons
                            where r.OfferId == OfferId
                            select new CouponModel { Id = r.Id, OfferId = r.OfferId, CoupanCode = r.CoupanCode, Mobile = r.Mobile }).ToList();
                    return Resp;
                }
            }
            catch (Exception ex)
            {
            }
            return Resp;
        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static ApiResponse DeleteOffers(int EventId)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                var result = dbConn.Offers.Where(a => a.Id == EventId).FirstOrDefault();
                if (result != null)
                {
                    result.IsDeleted = true;
                    dbConn.SaveChanges();
                    Resp.Code = (int)ApiResponseCode.ok;
                    Resp.Msg = "deleted successfully!";
                    return Resp;
                }
                else
                {
                    Resp.Msg = ApplicationStrings.Somethingwrmg;
                }
            }
            return Resp;
        }
        public static ApiResponse SendCoupan(string CoupanCode, string mobile, string Phone_CountryCode)
        {
            string sender = "Stream233";
            var PhoneCode = "";
            string PhoneReciever = "";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://www.ticketsandinvites.com/");
            //client.BaseAddress = new Uri("http://localhost:62111/");
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                var result = dbConn.Coupons.Where(a => a.CoupanCode == CoupanCode).FirstOrDefault();

                if (result != null)
                {
                    string Mobile = mobile;

                    if (Mobile != null)
                    {
                        PhoneCode = Phone_CountryCode;
                        Regex regexM = new Regex(@"^\d{10}$");
                        Match matchM = regexM.Match(Mobile);
                        if (matchM.Success)
                        {
                            if (Mobile.Length == 10 && PhoneCode == "233")
                            {
                                PhoneReciever = "00" + PhoneCode + Mobile.Substring(1);
                            }
                            else if (Mobile.Length == 10 && PhoneCode == "245")
                            {
                                PhoneReciever = "00" + PhoneCode + Mobile.Substring(1);
                            }
                            else
                            {
                                if (PhoneCode != "")
                                {
                                    PhoneReciever = "00" + PhoneCode + Mobile;
                                }
                            }

                            var Text = " Coupon code is: " + "" + CoupanCode + " " + "for Stream233" + ".";
                            if (PhoneReciever != null)
                            {
                                HttpResponseMessage CMSmsresponse = client.GetAsync("api/TicketAPI/SendCMSms?PhoneSender= " + sender + "&PhoneReciever=" + PhoneReciever + "&Text=" + Text + "").Result;
                            }
                        }
                    }
                    string email = mobile;
                    if (email != null)
                    {
                        Regex regexE = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                        Match matchE = regexE.Match(email);
                        if (matchE.Success)
                        {

                            string body = string.Format(CoupanCode + " is your Coupon code on Stream233.");

                            int mailer = (int)Ticketforwebsite.Stream233;
                            string MailfromName = "Stream233";
                            string Subject = "OTP";
                            int eventid = 0;
                            HttpResponseMessage CMSmsresponse = client.GetAsync("api/TicketAPI/SendLoginRegistrationEmail?email= " + email + "&body=" + body + "&MailfromName=" + MailfromName + "&Subject=" + Subject + "&mailer=" + mailer + "&eventid=" + eventid + "").Result;

                        }
                    }

                    result.Mobile = mobile;
                    dbConn.SaveChanges();
                    Resp.Code = (int)ApiResponseCode.ok;
                    Resp.Msg = "Coupon send successfully!";
                    return Resp;
                }
                else
                {
                    Resp.Msg = ApplicationStrings.Somethingwrmg;
                }
            }
            return Resp;
        }
        public static DashboardOrgModel GetDashboardDataOrg()
        {
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {

                    dbConn.Configuration.LazyLoadingEnabled = false;
                    var events = dbConn.Events.Include("EventTickets").Where(x => x.Company_Id == ManageSession.CompanySession.CompanyId).ToList();
                    var users = dbConn.Users;
                    DashboardOrgModel dash = new DashboardOrgModel();
                    List<TopsaleEvent> tp = new List<TopsaleEvent>();
                    List<RecentTicket> rt = new List<RecentTicket>();
                    List<TransactionHistory> ct = new List<TransactionHistory>();
                    List<TopsaleEvent> dashdata = new List<TopsaleEvent>();
                    var conts = (from r in dbConn.TickeUserMaps
                                 join p in dbConn.Payments on r.PaymemtId equals p.Id
                                 join w in dbConn.Wallets on p.Id equals w.PaymentId
                                 join u in dbConn.Users on p.UserId equals u.Id
                                 join t in dbConn.EventTickets on r.TicketId equals t.Id
                                 join e in dbConn.Events on t.Event_Id equals e.Id
                                 where w.CompanyId == ManageSession.CompanySession.CompanyId
                                 select new RecentTicket { Id = r.Id, UserName = u.FirstName, EventDate = e.StartDate.ToString(), EventName = e.Event_name, TotalAmt = w.TotalAmount.ToString(), Bookedticket = r.Qty.ToString(), PayId = p.Id }
                                 ).ToList();

                    var cresult = conts.GroupBy(item => item.Id)
                .Select(grouping => grouping.FirstOrDefault())
                .ToList();
                    var result = cresult.GroupBy(item => item.PayId)
                 .Select(grouping => new RecentTicket
                 {
                     Bookedticket = grouping.Count().ToString(),
                     EventDate = grouping.FirstOrDefault().EventDate,
                     EventName = grouping.FirstOrDefault().EventName,
                     TotalAmt = grouping.FirstOrDefault().TotalAmt,
                     UserName = grouping.FirstOrDefault().UserName
                 })// grouping.FirstOrDefault() )                 
                 .ToList();

                    //var cont = dbConn.Events.Include("EventTickets").Where(s => s.Company_Id == ManageSession.CompanySession.Id && s.Status == (int)EventStatus.Active).ToList();
                    //foreach (var i in cont)
                    //{
                    //    foreach (var j in i.EventTickets)
                    //    {
                    //        //var conts = dbConn.TickeUserMaps.Include(y => y.Payment).Where(s => s.TicketId == j.Id && s.ActualAmount > 0).ToList();
                    //        ////int userid=0;

                    //        //foreach (var k in conts.OrderByDescending(x => x.Id))
                    //        //{
                    //        //    var USERm = users.FirstOrDefault(x => x.Id == k.UserId);
                    //        //    //userid = USERm.Id;
                    //        //    rt.Add(new RecentTicket { UserName = (USERm.FirstName + " " + USERm.LastName), EventName = i.Event_name, EventDate = Convert.ToDateTime(i.StartDate).ToString("dd/MM/yyyy"), Bookedticket = k.Qty.ToString(), TotalAmt = (k.Qty * k.Amount).ToString() });
                    //        //}

                    //        var conts = from r in dbConn.TickeUserMaps join p in dbConn.TickeUserMaps on r.PaymemtId equals p.Id join w in dbConn.Wallets on p.Id equals w.PaymentId
                    //                    where w.CompanyId == ManageSession.CompanySession.Id





                    //        var conts = dbConn.TickeUserMaps.Include(y => y.Payment).Where(s => s.TicketId == j.Id && s.ActualAmount > 0 && s.Status == 1).ToList();
                    //        //int userid=0;
                    //        int bookedqty = 0;
                    //        foreach (var k in conts.OrderByDescending(x => x.Id))
                    //        {
                    //            var USERm = users.FirstOrDefault(x => x.Id == k.UserId);
                    //            //userid = USERm.Id;
                    //            bookedqty = Convert.ToInt32(k.Qty) + bookedqty;
                    //            rt.Add(new RecentTicket { UserName = (USERm.FirstName + " " + USERm.LastName), EventName = i.Event_name, EventDate = Convert.ToDateTime(i.StartDate).ToString("dd/MM/yyyy"), Bookedticket = k.Qty.ToString(), TotalAmt = k.Amount.ToString() });
                    //        }




                    //        var walletamt = dbConn.Wallets.Where(x => x.CompanyId == ManageSession.CompanySession.Id).ToList();
                    //        var totals = Convert.ToDouble(walletamt.Sum(x => x.TotalAmount));
                    //        dashdata.Add(new TopsaleEvent { EventName = i.Event_name, topticketSale = totals, Total = bookedqty });
                    //        //double total = i.EventTickets.Select(t => t.Quantity ?? 0).Sum();
                    //        //var totalsold = dbConn.TickeUserMaps.Where(x => x.TicketId == j.Id).Sum(x => x.ActualAmount);
                    //        //var totalqty = dbConn.TickeUserMaps.Where(x=> x.TicketId == j.Id).Sum(x => x.Qty);
                    //        //tp.Add(new TopsaleEvent { EventName = i.Event_name, topticketSale = Convert.ToDouble(totalsold), Total = Convert.ToInt32(totalqty) });
                    //    }
                    //}
                    TopsaleEvent tps = new TopsaleEvent();
                    var topev = result.GroupBy(x => x.EventName)
                    .Select(x => new
                    {
                        EventName = x.Key,
                        total = x.Sum(y => Convert.ToDouble(y.TotalAmt))
                    }).ToList();
                    var topeve = result.GroupBy(x => x.EventName)
                    .Select(x => new
                    {
                        EventName = x.Key,
                        totalqty = x.Sum(y => Convert.ToInt32(y.Bookedticket))
                    }).ToList();
                    tps.totalevent = events.Count();
                    tps.EventName = topev.OrderByDescending(x => x.total).FirstOrDefault().EventName.ToString();
                    tps.topticketSale = topev.OrderByDescending(x => x.total).FirstOrDefault().total;

                    tps.EventNametotal = topeve.OrderByDescending(x => x.totalqty).FirstOrDefault().EventName.ToString();
                    tps.Total = topeve.OrderByDescending(x => x.totalqty).FirstOrDefault().totalqty;
                    // tp.OrderByDescending(x => x.Total).FirstOrDefault().Total.ToString();
                    dash.Recenttickets = result.Take(10).ToList();
                    dash.Topsaleevents = tps;

                    return dash;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }
        #endregion


        #region scannUsercreate
        public static ApiResponse AddScannusers(string txtname, string txtemail, string txtPassword)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var existinguser = dbConn.ScanCompanyUsers.Where(a => a.Email == txtemail && a.Status == (int)ScannUserStatus.Active).FirstOrDefault();
                    if (existinguser == null)
                    {
                        //   var compy = dbConn.ScanCompanyUsers.Where(a => a.Id == ManageSession.CompanySession.Id).FirstOrDefault();
                        ScanCompanyUser cmp = new ScanCompanyUser();
                        cmp.Email = txtemail;
                        cmp.Name = txtname;
                        cmp.Password = txtPassword;
                        cmp.CompanyId = ManageSession.CompanySession.Id;
                        cmp.Status = (int)ScannUserStatus.Active;
                        cmp.Createdat = DateTime.Now;
                        cmp.Updateat = DateTime.Now;
                        dbConn.ScanCompanyUsers.Add(cmp);
                        Resp.Msg = "User Created Successfully!";
                        dbConn.SaveChanges();

                        //string body = string.Format("Hi {0},<br><br>We received a request to Create Account " +
                        //    "Your account associated with this email address.<br><br>Your temporary password is:<b>{1}</b><br><br>Please open the app and sign in to your account with this temporary password. You will then be prompted to create a new secure password." +
                        //    "<br><br>Sincerely,<br>{2}", txtname, txtPassword, ManageSession.CompanySession.CompName);
                        //EmailSending.SendEmail(txtemail, body, ManageSession.CompanySession.CompName, "Password");

                    }
                    else
                    {
                        Resp.Msg = "Error while uploading";
                    }

                }
            }
            catch (Exception ex)
            {
                Resp.Msg = ex.Message;
            }
            return Resp;
        }

        public static List<ScannUserModel> GetScanUserbyAccount(int CompnayId)
        {

            try
            {
                List<ScannUserModel> result = new List<ScannUserModel>();
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    var scanusers = dbConn.ScanCompanyUsers.Where(a => a.CompanyId == CompnayId && a.Status == (int)(int)ScannUserStatus.Active).ToList();
                    if (scanusers != null && scanusers.Count > 0)
                    {
                        //cmpny
                        var objectmaster = (from r in scanusers
                                            where r.CompanyId == CompnayId
                                            select new ScannUserModel
                                            {
                                                Id = r.Id,
                                                Name = r.Name,
                                                Email = r.Email
                                            }).ToList();

                        return objectmaster;
                    }


                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        public static ScannUserModel GetScanUser(int UserId)
        {
            ScannUserModel result = new ScannUserModel();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {

                    //userData
                    result = (from r in dbConn.ScanCompanyUsers
                              where r.Id == UserId
                              select new ScannUserModel
                              {
                                  Id = r.Id,
                                  Name = r.Name,
                                  Email = r.Email
                              }).FirstOrDefault();

                    return result;

                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }


        public static List<EventDataModel> GetAllEventbyCompany(int UserId)
        {
            List<EventDataModel> result = new List<EventDataModel>();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    if (UserId != 0)
                    {
                        int CompanyId = dbConn.ScanCompanyUsers.Where(a => a.Id == UserId && a.Status == (int)ScannUserStatus.Active).FirstOrDefault().CompanyId ?? 0;
                        //cmpny
                        DateTime date = new DateTime();
                        date = DateTime.UtcNow;
                        if (CompanyId != 0)
                        {
                            var EventRelCount = dbConn.ScannerEventRels.Where(x => x.ScanerId == UserId && x.status == (int)ScannUserStatus.Active).ToList();
                            if (EventRelCount.Count() == 0)
                            {

                                result = (from e in dbConn.Events
                                          where e.Company_Id == CompanyId && e.Status == 2 && e.EndDate >= date.Date
                                          select (new EventDataModel
                                          {
                                              Id = e.Id,
                                              EventName = e.Event_name,
                                              permission = false,
                                              CreateDate = e.Created_at ?? DateTime.Now,
                                          })).Distinct().OrderBy(x => x.CreateDate).ToList();
                                return result;
                            }
                            else
                            {
                                var res = (from e in dbConn.Events
                                           where e.Company_Id == CompanyId && e.Status == 2 && e.EndDate >= date.Date
                                           select (new EventDataModel
                                           {
                                               Id = e.Id,
                                               EventName = e.Event_name,
                                               permission = false,
                                               CreateDate = e.Created_at ?? DateTime.Now,
                                           })).Distinct().ToList();

                                foreach (var item in res)
                                {
                                    var data = EventRelCount.Find(x => x.EventId == item.Id);
                                    if (data != null)
                                    {
                                        item.permission = true;

                                    }
                                }

                                result = res;

                                return result;

                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }


        public static List<EventDataModel> GetSelectEventbyuser(int UserId)
        {
            List<EventDataModel> result = new List<EventDataModel>();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    if (UserId != 0)
                    {
                        int CompanyId = dbConn.ScanCompanyUsers.Where(a => a.Id == UserId && a.Status == (int)ScannUserStatus.Active).FirstOrDefault().CompanyId ?? 0;
                        //cmpny
                        if (CompanyId != 0)
                        {
                            var EventRelCount = dbConn.ScannerEventRels.Where(x => x.ScanerId == UserId && x.status == (int)ScannUserStatus.Active).Select(x => x.EventId).ToList();
                            if (EventRelCount.Count() > 0)
                            {
                                var res = (from e in dbConn.Events
                                           where e.Company_Id == CompanyId && e.Status == 2 && EventRelCount.Contains(e.Id)
                                           select (new EventDataModel
                                           {
                                               Id = e.Id,
                                               EventName = e.Event_name,
                                               permission = true,
                                               CreateDate = e.Created_at ?? DateTime.Now,
                                           })).Distinct().OrderBy(x => x.CreateDate).ToList();

                                result = res;

                                return result;
                            }
                            return result;
                        }
                    }


                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public static ApiResponse SaveUserEventPermission(int Id, string[] SelectedId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    List<ScannerEventRel> data = new List<ScannerEventRel>();

                    // delete userdata
                    var deletedataexit = dbConn.ScannerEventRels.Where(a => a.ScanerId == Id).ToList();
                    if (deletedataexit != null && deletedataexit.Count > 0)
                    {
                        foreach (var item in deletedataexit)
                        {
                            dbConn.ScannerEventRels.Remove(item);
                        }
                        dbConn.SaveChanges();
                    }

                    foreach (var item in SelectedId)
                    {
                        if (item != "")
                        {
                            int EventId = Convert.ToInt32(item);

                            var scanData = dbConn.ScannerEventRels.Where(a => a.EventId == EventId && a.ScanerId == Id).FirstOrDefault();

                            if (scanData == null)
                            {
                                ScannerEventRel setdata = new ScannerEventRel();
                                setdata.EventId = EventId;
                                setdata.ScanerId = Convert.ToInt32(Id);
                                setdata.status = (int)ScannUserStatus.Active;
                                setdata.Createdat = DateTime.Now;
                                setdata.Updateat = DateTime.Now;
                                dbConn.ScannerEventRels.Add(setdata);
                                // data.Add(setdata);
                            }
                        }
                    }

                    if (data != null)
                    {
                        dbConn.SaveChanges();
                        resp.Code = (int)ApiResponseCode.ok;
                        resp.Msg = "Successfully updated!";


                    }
                    else
                    {
                        resp.Msg = "Successfully updated!";
                    }



                    return resp;

                }
            }
            catch (Exception ex)
            {
                resp.Msg = "Error while save data!";
            }
            return resp;
        }


        public static ApiResponse EditUserEventPermission(int Id, string[] SelectedId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    List<ScannerEventRel> data = new List<ScannerEventRel>();

                    if (SelectedId.Count() > 0)
                    {
                        foreach (var item in SelectedId)
                        {
                            int EventId = Convert.ToInt32(item);
                            var scanData = dbConn.ScannerEventRels.Where(a => a.EventId == EventId && a.ScanerId == Id).FirstOrDefault();
                            if (scanData != null)
                            {
                                scanData.status = (int)ScannUserStatus.Delete;
                                dbConn.SaveChanges();
                                // data.Add(setdata);
                            }
                        }
                        resp.Code = (int)ApiResponseCode.ok;
                        resp.Msg = "Successfully updated!";


                    }
                    else
                    {
                        resp.Msg = "Error while save data!";
                    }

                    return resp;

                }
            }
            catch (Exception ex)
            {
                resp.Msg = "Error while save data!";
            }
            return resp;
        }

        public static ApiResponse DeleteuserScan(int Id)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {


                    var scanData = dbConn.ScanCompanyUsers.Where(a => a.Id == Id).FirstOrDefault();

                    if (scanData != null)
                    {
                        scanData.Status = (int)ScannUserStatus.Active;

                        dbConn.SaveChanges();
                        resp.Code = (int)ApiResponseCode.ok;
                        resp.Msg = "Successfully updated!";


                    }
                    else
                    {
                        resp.Msg = "Error while save data!";
                    }

                    return resp;

                }
            }
            catch (Exception ex)
            {
                resp.Msg = "Error while save data!";
            }
            return resp;
        }
        #endregion

        #region TicketstatusView

        public static List<Dropdownlist> GetEventSearchEvent(string prefix)
        {

            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        List<Dropdownlist> result = (from r in dbConn.Events
                                                     where r.Event_name.ToLower().Contains(prefix.ToLower()) && r.Company_Id == ManageSession.CompanySession.Id && r.Status == (int)EventStatus.Pending
                                                     select new Dropdownlist
                                                     {
                                                         Id = r.Id,
                                                         Text = r.Event_name
                                                     }).ToList();
                        return result;
                    }
                    else
                    {
                        List<Dropdownlist> result = (from r in dbConn.Events
                                                     where r.Company_Id == ManageSession.CompanySession.Id && r.Status == (int)EventStatus.Active // && r.EndDate >= DateTime.UtcNow
                                                     select new Dropdownlist
                                                     {
                                                         Id = r.Id,
                                                         Text = r.Event_name
                                                     }).ToList();
                        return result;
                    }



                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        public static List<EventticketModel> GetEventStatusTickets(string Event_Id, int CompnayId)
        {
            List<EventticketModel> eventsticket = new List<EventticketModel>();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    //get user from invitation list

                    if (!string.IsNullOrEmpty(Event_Id))
                    {
                        int EventId = Convert.ToInt32(Event_Id);

                        int id = Convert.ToInt32(Event_Id);
                        eventsticket = (from ev in dbConn.Events
                                        join tic in dbConn.EventTickets on ev.Id equals tic.Event_Id
                                        join e in dbConn.TickeUserMaps on tic.Id equals e.TicketId
                                        join u in dbConn.Users on e.UserId equals u.Id
                                        where ev.Id == EventId && tic.TicketStatus == (int)TicketStatus.Active && e.Status == 1
                                        select new EventticketModel
                                        {
                                            Id = tic.Id,
                                            TicketName = tic.TicketName,
                                            Eventname = ev.Event_name,
                                            TicketbuyerEmail = e.Email,
                                            CheckInStatus = e.IsCheckIn ?? false,
                                            Startdate = ev.StartDate ?? DateTime.Now,
                                            CheckInDate = e.UpdateDate,
                                            Venuename = ev.Venue,
                                            Name = u.FirstName + " " + u.LastName

                                        }).ToList();


                        return eventsticket;


                    }

                }
            }
            catch (Exception ex)
            {

            }
            return eventsticket;
        }

        #endregion
    }
}