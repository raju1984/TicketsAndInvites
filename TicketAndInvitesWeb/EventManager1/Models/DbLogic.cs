using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventManager1.DBCon;
using System.Data.Entity;
using System.Globalization;

namespace EventManager1.Models
{
    public class DbLogic
    {
        public static List<IndexView> GetIndexData(int skip, int take, int type = 0)
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                List<IndexView> result = new List<IndexView>();
                SubscriptionTypes subs = new SubscriptionTypes();
                dbConn.Configuration.LazyLoadingEnabled = false;
                var edate = DateTime.UtcNow;
                try
                {
                    List<Event> events = new List<Event>();
                    if (type == 1)
                    {
                        //get only subscribe event
                        events = dbConn.Events.Include(a => a.Address)
                      .Include(a => a.MultimediaContents).Include(a => a.EventTickets)
                      .Where(a => a.Company_Id != null && a.SubscriptionType != null && a.SubscriptionType > 0 && a.EventTickets.Where(x => x.IsEnable == true).Count() > 0 && a.Status == (int)EventStatus.Active && a.EndDate >= edate && (a.CreatedOnWebsite == (int)Ticketforwebsite.Stream233 || a.CreatedOnWebsite == (int)Ticketforwebsite.Scribeproduction)).ToList();
                    }
                    else
                    {
                        //get free events
                        events = dbConn.Events.Include(a => a.Address)
                      .Include(a => a.MultimediaContents).Include(a => a.EventTickets)
                      .Where(a => a.Company_Id != null && a.EventTickets.Where(x => x.IsEnable == true).Count() > 0 && a.Status == (int)EventStatus.Active && a.EndDate >= edate && a.SubscriptionType == null && (a.CreatedOnWebsite == (int)Ticketforwebsite.Stream233 || a.CreatedOnWebsite == (int)Ticketforwebsite.Scribeproduction)).ToList();
                    }

                    //var events = dbConn.Events
                    //  .Where(a => a.Company_Id != null && a.IsActive == true && a.Address_Id != null).ToList();
                    if (take == 10)
                    {
                        events = events.Take(10).ToList();
                    }
                    var sub = dbConn.AdminSetups.FirstOrDefault();
                    subs.Platinum = Convert.ToInt32(sub.PlatinumRow);
                    subs.Gold = Convert.ToInt32(sub.GoldRow);
                    subs.Silver = Convert.ToInt32(sub.SilverRow);
                    subs.Bronze = Convert.ToInt32(sub.BronzeRow);
                    result = (from r in events
                              select new IndexView
                              {
                                  Id = r.Id,
                                  EventName = r.Event_name,
                                  EventStartDate = r.StartDate,
                                  EventEndDate = r.EndDate,
                                  EventCatg = r.eventCatg != null ? r.eventCatg : 1,
                                  EventType = r.Eventtype,
                                  IsPopupar = r.IsPopularEvent,
                                  Location = r.Address.City != null ? r.Address.City : r.Address.Country.Name,
                                  Currency = "Gh₵",
                                  SubCriptionType = r.SubscriptionType,
                                  Images = (from img in r.MultimediaContents
                                            select new Images
                                            {
                                                MulType = img.Mul_Type,
                                                mainpic = img.Mul_MainPic,
                                                Url = img.URL
                                            }).ToList(),
                                  Price = (from p in r.EventTickets
                                           select new TicketPrice
                                           {
                                               Price = p.Price
                                           }).ToList(),
                                  subscripton = subs
                              }).ToList();



                }
                catch (Exception ex)
                {

                }

                return result;
            }
        }

        public static int GetEventType(int EventId)
        {
            int EventType = 0;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var data = dbConn.Events.Where(x => x.Id == EventId).FirstOrDefault();
                    EventType = data.Eventtype == null ? 0 : data.Eventtype.Value;
                }
            }
            catch { }
            return EventType;
        }
        public static List<IndexView> GetIndexeEvent(int skip, int take, int type = 0)
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                List<IndexView> result = new List<IndexView>();
                //SubscriptionTypes subs = new SubscriptionTypes();
                dbConn.Configuration.LazyLoadingEnabled = false;
                var edate = DateTime.UtcNow;
                try
                {
                    List<Event> events = new List<Event>();
                    //get only subscribe event
                    events = dbConn.Events.Include(a => a.Address)
                      .Include(a => a.MultimediaContents).Include(a => a.EventTickets)
                      .Where(a => a.Company_Id != null && a.Eventtype == 0 && a.EventTickets.Where(x => x.IsEnable == true).Count() > 0 && a.Status == (int)EventStatus.Active && a.Address_Id != null && a.EndDate >= edate).ToList();

                    //var events = dbConn.Events
                    //  .Where(a => a.Company_Id != null && a.IsActive == true && a.Address_Id != null).ToList();
                    if (take == 10)
                    {
                        events = events.Take(10).ToList();
                    }
                    //var sub = dbConn.AdminSetups.FirstOrDefault();
                    //subs.Platinum = Convert.ToInt32(sub.PlatinumRow);
                    //subs.Gold = Convert.ToInt32(sub.GoldRow);
                    //subs.Silver = Convert.ToInt32(sub.SilverRow);
                    //subs.Bronze = Convert.ToInt32(sub.BronzeRow);
                    result = (from r in events
                              where r.EventTickets.Count() > 0
                              select new IndexView
                              {
                                  Id = r.Id,
                                  EventName = r.Event_name,
                                  EventStartDate = r.StartDate,
                                  EventEndDate = r.EndDate,
                                  EventType = r.Eventtype,
                                  EventCatg = r.eventCatg != null ? r.eventCatg : 1,
                                  IsPopupar = r.IsPopularEvent,
                                  Location = r.Address.City != null ? r.Address.City : r.Address.Country.Name,
                                  Currency = "Gh₵",
                                  SubCriptionType = r.SubscriptionType,
                                  Images = (from img in r.MultimediaContents
                                            select new Images
                                            {
                                                MulType = img.Mul_Type,
                                                mainpic = img.Mul_MainPic,
                                                Url = img.URL
                                            }).ToList(),
                                  Price = (from p in r.EventTickets
                                           select new TicketPrice
                                           {
                                               Price = p.Price
                                           }).ToList(),
                                  // subscripton = subs
                              }).ToList();



                }
                catch (Exception ex)
                {

                }

                return result;
            }
        }
        public static ApiResponse submitPaymentSupport(PaymentSupportModel obj)
        {
            ApiResponse res = new ApiResponse();
            res.Code = (int)ApiResponseCode.fail;
            res.Msg = "Something went wrong! Please try again";
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    PaymentSupportt p = new PaymentSupportt();
                    p.email = obj.Email;
                    p.name = obj.Name;
                    p.transactionId = obj.TrxId;
                    p.created_at = DateTime.UtcNow;
                    dbConn.PaymentSupportts.Add(p);
                    if (dbConn.SaveChanges() > 0)
                    {
                        res.Code = (int)ApiResponseCode.ok;
                        res.Msg = "Data submitted successfully";
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return res;
        }
        public static List<IndexView> GetLiveEvent()
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                List<IndexView> result = new List<IndexView>();
                SubscriptionTypes subs = new SubscriptionTypes();
                dbConn.Configuration.LazyLoadingEnabled = false;
                var edate = DateTime.UtcNow;
                try
                {
                    List<Event> events = new List<Event>();

                    //get free events
                    events = dbConn.Events.Include(a => a.MultimediaContents).Include(a => a.EventTickets)
                  .Where(a => a.Company_Id != null && a.Status == (int)EventStatus.Active && a.EndDate >= edate && a.EventTickets.Where(x => x.IsEnable == true).Count() > 0 && a.Eventtype == 2).ToList();

                    //var events = dbConn.Events
                    //  .Where(a => a.Company_Id != null && a.IsActive == true && a.Address_Id != null).ToList();
                    //if (take == 10)
                    //{
                    //    events = events.Take(10).ToList();
                    //}
                    //var sub = dbConn.AdminSetups.FirstOrDefault();
                    //subs.Platinum = Convert.ToInt32(sub.PlatinumRow);
                    //subs.Gold = Convert.ToInt32(sub.GoldRow);
                    //subs.Silver = Convert.ToInt32(sub.SilverRow);
                    //subs.Bronze = Convert.ToInt32(sub.BronzeRow);

                    result = (from r in events
                              where r.EventTickets.Count() > 0
                              select new IndexView
                              {
                                  Id = r.Id,
                                  EventName = r.Event_name,
                                  EventStartDate = r.StartDate,
                                  EventCatg = r.eventCatg,
                                  EventEndDate = r.EndDate,
                                  //IsPopupar = r.IsPopularEvent,
                                  //Location = r.Address.City != null ? r.Address.City : r.Address.Country.Name,
                                  Currency = "Gh₵",
                                  //SubCriptionType = r.SubscriptionType,
                                  Images = (from img in r.MultimediaContents
                                            select new Images
                                            {
                                                MulType = img.Mul_Type,
                                                mainpic = img.Mul_MainPic,
                                                Url = img.URL
                                            }).ToList(),
                                  Price = (from p in r.EventTickets
                                           select new TicketPrice
                                           {
                                               Price = p.Price
                                           }).ToList(),
                                  //subscripton = subs
                              }).ToList();



                }
                catch (Exception ex)
                {

                }

                return result;
            }
        }
        public static List<IndexView> GetVideoEvent()
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                List<IndexView> result = new List<IndexView>();
                SubscriptionTypes subs = new SubscriptionTypes();
                dbConn.Configuration.LazyLoadingEnabled = false;
                var edate = DateTime.UtcNow;
                try
                {
                    List<Event> events = new List<Event>();

                    //get free events
                    events = dbConn.Events.Include(a => a.MultimediaContents).Include(a => a.EventTickets)
                  .Where(a => a.Company_Id != null && a.Status == (int)EventStatus.Active && a.EndDate >= edate && a.EventTickets.Where(x => x.IsEnable == true).Count() > 0 && a.Eventtype == 1).ToList();

                    //var events = dbConn.Events
                    //  .Where(a => a.Company_Id != null && a.IsActive == true && a.Address_Id != null).ToList();
                    //if (take == 10)
                    //{
                    //    events = events.Take(10).ToList();
                    //}
                    //var sub = dbConn.AdminSetups.FirstOrDefault();
                    //subs.Platinum = Convert.ToInt32(sub.PlatinumRow);
                    //subs.Gold = Convert.ToInt32(sub.GoldRow);
                    //subs.Silver = Convert.ToInt32(sub.SilverRow);
                    //subs.Bronze = Convert.ToInt32(sub.BronzeRow);
                    result = (from r in events
                              where r.EventTickets.Count() > 0
                              select new IndexView
                              {
                                  Id = r.Id,
                                  EventName = r.Event_name,
                                  EventStartDate = r.StartDate,
                                  EventCatg = r.eventCatg,
                                  EventEndDate = r.EndDate,
                                  //IsPopupar = r.IsPopularEvent,
                                  //Location = r.Address.City != null ? r.Address.City : r.Address.Country.Name,
                                  Currency = "Gh₵",
                                  //SubCriptionType = r.SubscriptionType,
                                  Images = (from img in r.MultimediaContents
                                            select new Images
                                            {
                                                MulType = img.Mul_Type,
                                                mainpic = img.Mul_MainPic,
                                                Url = img.URL
                                            }).ToList(),
                                  Price = (from p in r.EventTickets
                                           select new TicketPrice
                                           {
                                               Price = p.Price
                                           }).ToList(),
                                  //subscripton = subs
                              }).ToList();



                }
                catch (Exception ex)
                {

                }

                return result;
            }
        }
        public static List<IndexView> GetSearchEventData(string eventname = null, string place = null, string Eventday = null, int eventtype = 0)
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                List<IndexView> result = new List<IndexView>();
                dbConn.Configuration.LazyLoadingEnabled = false;
                try
                {
                    List<Event> events = new List<Event>();
                    if (eventtype > 0)
                    {
                        //get only subscribe event
                        if (eventtype == 5)
                        {
                            events = dbConn.Events.Include(a => a.Address)
                                    .Include(a => a.MultimediaContents).Include(a => a.EventTickets)
                                    .Where(a => a.Company_Id != null && a.IsPopularEvent == true && a.EventTickets.Where(x => x.IsEnable == true).Count() > 0 && a.Status == (int)EventStatus.Active && a.Address_Id != null && a.EndDate >= DateTime.UtcNow && a.CreatedOnWebsite == (int)Ticketforwebsite.Stream233).ToList();
                        }
                        else if (eventtype == 6)
                        {
                            //get all events
                            events = dbConn.Events.Include(a => a.Address)
                                   .Include(a => a.MultimediaContents).Include(a => a.EventTickets)
                                   .Where(a => a.Company_Id != null && a.EventTickets.Where(x => x.IsEnable == true).Count() > 0 && a.Status == (int)EventStatus.Active && a.Address_Id != null && a.EndDate >= DateTime.UtcNow && a.CreatedOnWebsite == (int)Ticketforwebsite.Stream233).ToList();
                        }
                        else
                        {
                            events = dbConn.Events.Include(a => a.Address)
                                                .Include(a => a.MultimediaContents).Include(a => a.EventTickets)
                                                .Where(a => a.Company_Id != null && a.SubscriptionType == eventtype && a.EventTickets.Where(x => x.IsEnable == true).Count() > 0 && a.Status == (int)EventStatus.Active && a.Address_Id != null && a.EndDate >= DateTime.UtcNow).ToList();
                        }

                    }
                    else
                    {
                        //get all events for search condition
                        events = dbConn.Events.Include(a => a.Address)
                      .Include(a => a.MultimediaContents).Include(a => a.EventTickets)
                      .Where(a => a.Company_Id != null && a.Status == (int)EventStatus.Active && a.Address_Id != null && a.EventTickets.Where(x => x.IsEnable == true).Count() > 0 && a.EndDate >= DateTime.UtcNow && a.CreatedOnWebsite == (int)Ticketforwebsite.Stream233).ToList();
                        if (!string.IsNullOrEmpty(eventname))
                        {
                            events = (from e in events
                                      where e.Event_name.ToLower().Contains(eventname.ToLower())
                                      select e).ToList();

                        }
                        if (!string.IsNullOrEmpty(place))
                        {
                            events = (from e in events
                                      where e.Address != null && e.Address.City.Contains(place.ToLower()) || e.Address.AddressLine.ToLower().Contains(place)
                                      select e).ToList();
                        }
                        if (!string.IsNullOrEmpty(Eventday))
                        {
                            string[] days = { "td", "tm", "twek", "nwek" };
                            if (Eventday == days[0])
                            {
                                events = (from e in events
                                          where Convert.ToDateTime(e.StartDate).Date == DateTime.UtcNow
                                          select e).ToList();
                            }
                            else if (Eventday == days[1])
                            {
                                events = (from e in events
                                          where Convert.ToDateTime(e.StartDate).Date == DateTime.UtcNow.AddDays(1)
                                          select e).ToList();
                            }
                            else if (Eventday == days[2])
                            {
                                DateTime startOfWeek = DateTime.Today.AddDays((int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)DateTime.Today.DayOfWeek);
                                events = (from e in events
                                          where Convert.ToDateTime(e.StartDate).Date == startOfWeek.AddDays(5)
                                          select e).ToList();
                            }
                            else if (Eventday == days[3])
                            {
                                DateTime startOfWeek = DateTime.Today.AddDays((int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)DateTime.Today.DayOfWeek);
                                events = (from e in events
                                          where Convert.ToDateTime(e.StartDate).Date == startOfWeek.AddDays(12)
                                          select e).ToList();
                            }

                        }
                    }
                    //var events = dbConn.Events
                    //  .Where(a => a.Company_Id != null && a.IsActive == true && a.Address_Id != null).ToList();
                    result = (from r in events
                              select new IndexView
                              {
                                  Id = r.Id,
                                  EventName = r.Event_name,
                                  EventStartDate = r.StartDate,
                                  EventEndDate = r.EndDate,
                                  IsPopupar = r.IsPopularEvent,
                                  Location = r.Address.City != null ? r.Address.City : r.Address.Country.Name,
                                  Currency = "Gh₵",
                                  SubCriptionType = r.SubscriptionType,
                                  Images = (from img in r.MultimediaContents
                                            select new Images
                                            {
                                                MulType = img.Mul_Type,
                                                mainpic = img.Mul_MainPic,
                                                Url = img.URL
                                            }).ToList(),
                                  Price = (from p in r.EventTickets
                                           select new TicketPrice
                                           {
                                               Price = p.Price
                                           }).ToList()
                              }).ToList();

                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }

        public static IndexView GetEventDetail(int Id)
        {
            IndexView result = new IndexView();
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                dbConn.Configuration.LazyLoadingEnabled = false;
                try
                {
                    var events = dbConn.Events.Include(a => a.Address).Include(a => a.Company).Include(a => a.RSVPs)
                        .Include(a => a.MultimediaContents).Include(a => a.EventTickets).
                        Where(a => a.Id == Id).FirstOrDefault();
                    if (events != null)
                    {
                        result.Id = events.Id;
                        result.EventName = events.Event_name;
                        result.EventType = events.Eventtype;
                        result.EventStartDate = events.StartDate;
                        result.EventEndDate = events.EndDate;
                        result.Description = events.Description;
                        result.Location = events.Address.City != null ? events.Address.City : events.Address.Country.Name;
                        result.Currency = "Gh₵";
                        result.longt = events.longitude;
                        result.lati = events.latitude;
                        result.address = events.Address.AddressLine;

                        result.VenuName = events.Venue;
                        result.RsvpDetails = (from rs in events.RSVPs
                                              select new RsvpDetail
                                              {
                                                  Name = rs.Namer,
                                                  Contact = rs.Phone
                                              }).ToList();
                        result.Images = (from img in events.MultimediaContents
                                         select new Images
                                         {
                                             MulType = img.Mul_Type,
                                             mainpic = Convert.ToBoolean(img.Mul_MainPic),
                                             Url = img.URL
                                         }).ToList();
                        result.Price = (from p in events.EventTickets
                                        where p.Inviation_Id == null
                                        select new TicketPrice
                                        {
                                            TicketType = p.Ticket_Type,
                                            TicketName = p.TicketName,
                                            Price = p.Price,
                                            AvailableQnty = p.AvailableQuantity
                                        }).ToList();
                        if (events.Company != null)
                        {
                            CompanyModel com = new CompanyModel();
                            var address = dbConn.Addresses.Include(y => y.Country).FirstOrDefault(x => x.Id == events.Company.Address_Id);
                            result.CompanyName = events.Company.Name_of_business;
                            com.Address = address.AddressLine;
                            com.City = address.City;
                            com.Country = address.Country.Name;
                            com.Contact = events.Company.Business_contact_number;
                            com.EmailId = events.Company.Business_Email_address;
                            result.company = com;

                        }

                        if (events.Company_Id != null) { result.CompanyId = Convert.ToInt32(events.Company_Id); }
                        else { result.CompanyId = 0; }
                    }


                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }
        //public static TicketModelPopup GetTicket(int Id)
        //{
        //    TicketModelPopup result = new TicketModelPopup();
        //    using (EventmanagerEntities dbConn = new EventmanagerEntities())
        //    {
        //        dbConn.Configuration.LazyLoadingEnabled = false;
        //        try
        //        {
        //            var events = dbConn.Events.Include(a => a.EventTickets).Where(a => a.Id == Id && a.EventTickets.Where(x => x.IsEnable == true).Count() > 0).FirstOrDefault();
        //            result.Eventname = events.Event_name; result.CoverImage = dbConn.MultimediaContents.FirstOrDefault(x => x.Event_Id == events.Id && x.Mul_Type == 1) != null ? dbConn.MultimediaContents.FirstOrDefault(x => x.Event_Id == events.Id && x.Mul_Type == 1).URL : "https://www.ticketsandinvites.com/images/placeholder-image.jpg";
        //            result.EventId = events.Id;
        //            result.EventDate = events.StartDate;
        //            if (events.Eventtype == 1 || events.Eventtype == 2 || events.Eventtype == 0)
        //            {
        //                result.Tickest = (from t in events.EventTickets
        //                                  where t.Inviation_Id == null && t.IsEnable == true
        //                                  select new TicketPrice
        //                                  {
        //                                      TicketId = t.Id,
        //                                      TicketName = t.TicketName,
        //                                      AvailableQnty = t.AvailableQuantity > 0 ? (events.Eventtype == 0 ? t.AvailableQuantity : 1) : 0,
        //                                      Price = t.Price
        //                                  }).ToList();
        //                //foreach (var i in result.Tickest)
        //                //{
        //                //    //i.IsPurchased = false;
        //                //    var validd = dbConn.EventTickets.FirstOrDefault(x => x.Id == i.TicketId);
        //                //    if (validd != null)
        //                //    {
        //                //        if (validd.ValidDays != null && validd.ValidDays > 0)
        //                //        {

        //                //            var res = dbConn.TickeUserMaps.Include(y => y.Payment).Include(m => m.EventTicket).FirstOrDefault(x => x.TicketId == i.TicketId && x.UserId == ManageSession.UserSession.Id && x.Payment.Status == (int)PaymentStatus.PaymentSuccess && DbFunctions.AddDays(x.Payment.PaymentDate, validd.ValidDays) >= DateTime.Now);

        //                //            ////var res1 = dbConn.MultimediaContents.FirstOrDefault(x => x.Event_Id == events.Id);
        //                //            ////var expire = Convert.ToDateTime(res.CreateDate).AddDays(Convert.ToInt32(res1.validdays));
        //                //            ////bool isexpire = DateTime.UtcNow.Date <= expire;
        //                //            if (res != null)
        //                //            {
        //                //                i.IsPurchased = true;
        //                //            }

        //                //        }
        //                //        else
        //                //        {
        //                //            var res = dbConn.TickeUserMaps.Include(y => y.Payment).Include(m => m.EventTicket).FirstOrDefault(x => x.TicketId == i.TicketId && x.UserId == ManageSession.UserSession.Id && x.Payment.Status == (int)PaymentStatus.PaymentSuccess);

        //                //            ////var res1 = dbConn.MultimediaContents.FirstOrDefault(x => x.Event_Id == events.Id);
        //                //            ////var expire = Convert.ToDateTime(res.CreateDate).AddDays(Convert.ToInt32(res1.validdays));
        //                //            ////bool isexpire = DateTime.UtcNow.Date <= expire;
        //                //            if (res != null)
        //                //            {
        //                //                i.IsPurchased = true;
        //                //            }
        //                //        }
        //                //    }




        //                //}
        //            }
        //            else
        //            {
        //                result.Tickest = (from t in events.EventTickets
        //                                  where t.Inviation_Id == null && t.IsEnable == true
        //                                  select new TicketPrice
        //                                  {
        //                                      TicketId = t.Id,
        //                                      TicketName = t.TicketName,
        //                                      AvailableQnty = t.AvailableQuantity,
        //                                      Price = t.Price
        //                                  }).ToList();
        //            }

        //            //var cont = (from r in dbConn.Users where r.Id == ManageSession.UserSession.Id select new ContactDetails { Email = r.Email, Mobile = r.PhoneNo, name = r.FirstName }).FirstOrDefault();
        //            //result.ContactDetail = cont;
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //    return result;
        //}
        public static TicketModelPopup GetTicket(int Id)
        {
            TicketModelPopup result = new TicketModelPopup();
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                dbConn.Configuration.LazyLoadingEnabled = false;
                try
                {
                    var events = dbConn.Events.Include(a => a.EventTickets).Where(a => a.Id == Id).FirstOrDefault();
                    result.Eventname = events.Event_name;
                    result.EventId = events.Id;
                    result.EventDate = events.StartDate;
                    result.CreatedOnWebsite = events.CreatedOnWebsite;
                    
                    if (events.Eventtype == 1)
                    {
                        result.Tickest = (from t in events.EventTickets
                                          where t.Inviation_Id == null
                                          select new TicketPrice
                                          {
                                              TicketId = t.Id,
                                              TicketType = t.Ticket_Type,
                                              TicketName = t.TicketName,
                                              AvailableQnty = t.AvailableQuantity > 0 ? 1 : 0,
                                              Price = t.Price,
                                              IsEnable = t.IsEnable,
                                              PaymentType = Convert.ToInt32(t.PaymentCurrency == null ? 1 : t.PaymentCurrency)
                                              
                                          }).OrderBy(x => x.PaymentType).ToList();
                        //foreach (var i in result.Tickest)
                        //{
                        //    //i.IsPurchased = false;
                        //    var validd = dbConn.EventTickets.FirstOrDefault(x => x.Id == i.TicketId);
                        //    if (validd != null)
                        //    {
                        //        if (validd.ValidDays != null && validd.ValidDays > 0)
                        //        {
                        //            var res = dbConn.TickeUserMaps.Include(y => y.Payment).Include(m => m.EventTicket).FirstOrDefault(x => x.TicketId == i.TicketId && x.UserId == ManageSession.UserSession.Id && x.Payment.Status == (int)PaymentStatus.PaymentSuccess && DbFunctions.AddDays(x.Payment.PaymentDate, validd.ValidDays) >= DateTime.Now);
                        //            if (res != null)
                        //            {
                        //                i.IsPurchased = true;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            var res = dbConn.TickeUserMaps.Include(y => y.Payment).Include(m => m.EventTicket).FirstOrDefault(x => x.TicketId == i.TicketId && x.UserId == ManageSession.UserSession.Id && x.Payment.Status == (int)PaymentStatus.PaymentSuccess);

                        //            if (res != null)
                        //            {
                        //                i.IsPurchased = true;
                        //            }
                        //        }

                        //    }

                        //    ////var res1 = dbConn.MultimediaContents.FirstOrDefault(x => x.Event_Id == events.Id);
                        //    ////var expire = Convert.ToDateTime(res.CreateDate).AddDays(Convert.ToInt32(res1.validdays));
                        //    ////bool isexpire = DateTime.UtcNow.Date <= expire;

                        //}
                    }
                    else
                    {
                        result.Tickest = (from t in events.EventTickets
                                          where t.Inviation_Id == null && t.IsEnable == true
                                          select new TicketPrice
                                          {
                                              TicketId = t.Id,
                                              TicketType = t.Ticket_Type,
                                              TicketName = t.TicketName,
                                              AvailableQnty = t.AvailableQuantity,
                                              Price = t.Price,
                                              IsEnable = t.IsEnable,
                                              PaymentType = Convert.ToInt32(t.PaymentCurrency == null ? 1 : t.PaymentCurrency)
                                          }).OrderBy(x => x.PaymentType).ToList();
                    }

                    //var cont = (from r in dbConn.Users where r.Id == ManageSession.UserSession.Id select new ContactDetails { Email = r.Email, Mobile = r.PhoneNo, name = r.FirstName }).FirstOrDefault();
                    //result.ContactDetail = cont;
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }
        public static TicketModelPopup GetTicketNew(int Id, string p, int tid)
        {
            TicketModelPopup result = new TicketModelPopup();
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                dbConn.Configuration.LazyLoadingEnabled = false;
                try
                {
                    var events = dbConn.Events.Include(a => a.EventTickets).Where(a => a.Id == Id).FirstOrDefault();
                    result.Eventname = events.Event_name;
                    result.EventId = events.Id;
                    result.EventDate = events.StartDate;
                    result.CreatedOnWebsite = events.CreatedOnWebsite;
                    if (tid > 0)
                    {
                        if (events.Eventtype == 1)
                        {
                            result.Tickest = (from t in events.EventTickets
                                              where t.Inviation_Id == null && t.Id==tid
                                              select new TicketPrice
                                              {
                                                  TicketId = t.Id,
                                                  TicketType = t.Ticket_Type,
                                                  TicketName = t.TicketName,
                                                  AvailableQnty = t.AvailableQuantity > 0 ? 1 : 0,
                                                  Price = t.Price,
                                                  IsEnable = t.IsEnable,
                                                  PaymentType = Convert.ToInt32(t.PaymentCurrency == null ? 1 : t.PaymentCurrency)
                                              }).OrderBy(x => x.PaymentType).ToList();
                          

                            //}
                        }
                        else
                        {
                            result.Tickest = (from t in events.EventTickets
                                              where t.Inviation_Id == null && t.IsEnable == true && t.Id == tid
                                              select new TicketPrice
                                              {
                                                  TicketId = t.Id,
                                                  TicketType = t.Ticket_Type,
                                                  TicketName = t.TicketName,
                                                  AvailableQnty = t.AvailableQuantity,
                                                  Price = t.Price,
                                                  IsEnable = t.IsEnable,
                                                  PaymentType = Convert.ToInt32(t.PaymentCurrency == null ? 1 : t.PaymentCurrency)
                                              }).OrderBy(x => x.PaymentType).ToList();
                        }
                    }
                    else
                    {
                        if (events.Eventtype == 1)
                        {
                            result.Tickest = (from t in events.EventTickets
                                              where t.Inviation_Id == null
                                              select new TicketPrice
                                              {
                                                  TicketId = t.Id,
                                                  TicketType = t.Ticket_Type,
                                                  TicketName = t.TicketName,
                                                  AvailableQnty = t.AvailableQuantity > 0 ? 1 : 0,
                                                  Price = t.Price,
                                                  IsEnable = t.IsEnable,
                                                  PaymentType = Convert.ToInt32(t.PaymentCurrency == null ? 1 : t.PaymentCurrency)
                                              }).OrderBy(x => x.PaymentType).ToList();
                            //foreach (var i in result.Tickest)
                            //{
                            //    //i.IsPurchased = false;
                            //    var validd = dbConn.EventTickets.FirstOrDefault(x => x.Id == i.TicketId);
                            //    if (validd != null)
                            //    {
                            //        if (validd.ValidDays != null && validd.ValidDays > 0)
                            //        {
                            //            var res = dbConn.TickeUserMaps.Include(y => y.Payment).Include(m => m.EventTicket).FirstOrDefault(x => x.TicketId == i.TicketId && x.UserId == ManageSession.UserSession.Id && x.Payment.Status == (int)PaymentStatus.PaymentSuccess && DbFunctions.AddDays(x.Payment.PaymentDate, validd.ValidDays) >= DateTime.Now);
                            //            if (res != null)
                            //            {
                            //                i.IsPurchased = true;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            var res = dbConn.TickeUserMaps.Include(y => y.Payment).Include(m => m.EventTicket).FirstOrDefault(x => x.TicketId == i.TicketId && x.UserId == ManageSession.UserSession.Id && x.Payment.Status == (int)PaymentStatus.PaymentSuccess);

                            //            if (res != null)
                            //            {
                            //                i.IsPurchased = true;
                            //            }
                            //        }

                            //    }

                            //    ////var res1 = dbConn.MultimediaContents.FirstOrDefault(x => x.Event_Id == events.Id);
                            //    ////var expire = Convert.ToDateTime(res.CreateDate).AddDays(Convert.ToInt32(res1.validdays));
                            //    ////bool isexpire = DateTime.UtcNow.Date <= expire;

                            //}
                        }
                        else
                        {
                            result.Tickest = (from t in events.EventTickets
                                              where t.Inviation_Id == null && t.IsEnable == true
                                              select new TicketPrice
                                              {
                                                  TicketId = t.Id,
                                                  TicketType = t.Ticket_Type,
                                                  TicketName = t.TicketName,
                                                  AvailableQnty = t.AvailableQuantity,
                                                  Price = t.Price,
                                                  IsEnable = t.IsEnable,
                                                  PaymentType = Convert.ToInt32(t.PaymentCurrency == null ? 1 : t.PaymentCurrency)
                                              }).OrderBy(x => x.PaymentType).ToList();
                        }
                    }
                    //var cont = (from r in dbConn.Users where r.Id == ManageSession.UserSession.Id select new ContactDetails { Email = r.Email, Mobile = r.PhoneNo, name = r.FirstName }).FirstOrDefault();
                    //result.ContactDetail = cont;
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }
        public static TicketModelPopup GetTicketSummaryPage(int Id)
        {
            TicketModelPopup result = new TicketModelPopup();
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                dbConn.Configuration.LazyLoadingEnabled = false;
                try
                {

                    var events = dbConn.Events.Include(a => a.EventTickets).Include(a => a.MultimediaContents).Where(a => a.Id == Id).FirstOrDefault();
                    result.Eventname = events.Event_name;
                    result.EventId = events.Id;
                    result.EventDate = events.StartDate;
                    result.CoverImage = events.MultimediaContents.Where(a => a.Mul_Type == (int)MultiMediaType.image).FirstOrDefault().URL;
                    foreach (var item in ManageSession.TicketCartSession.TickeCarts)
                    {
                        item.TicketName = events.EventTickets.Where(a => a.Id == item.TicketId).FirstOrDefault().TicketName;
                        item.Price = events.EventTickets.Where(a => a.Id == item.TicketId).FirstOrDefault().Price;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public static TicketModelPopup GetTicketNew(int Id, int TicketId)
        {
            TicketModelPopup result = new TicketModelPopup();
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                dbConn.Configuration.LazyLoadingEnabled = false;
                try
                {
                    var events = dbConn.Events.Include(a => a.EventTickets).Where(a => a.Id == Id).FirstOrDefault();
                    result.Eventname = events.Event_name;
                    result.EventId = events.Id;
                    result.EventDate = events.StartDate;
                    result.CreatedOnWebsite = events.CreatedOnWebsite;
                    if (events.Eventtype == 1)
                    {
                        result.Tickest = (from t in events.EventTickets
                                          where t.Inviation_Id == null && t.Id == TicketId
                                          select new TicketPrice
                                          {
                                              TicketId = t.Id,
                                              TicketType = t.Ticket_Type,
                                              TicketName = t.TicketName,
                                              AvailableQnty = t.AvailableQuantity > 0 ? 1 : 0,
                                              Price = t.Price,
                                              IsEnable = t.IsEnable,
                                              PaymentType = Convert.ToInt32(t.PaymentCurrency == null ? 1 : t.PaymentCurrency)
                                          }).OrderBy(x => x.PaymentType).ToList();
                        //foreach (var i in result.Tickest)
                        //{
                        //    //i.IsPurchased = false;
                        //    var validd = dbConn.EventTickets.FirstOrDefault(x => x.Id == i.TicketId);
                        //    if (validd != null)
                        //    {
                        //        if (validd.ValidDays != null && validd.ValidDays > 0)
                        //        {
                        //            var res = dbConn.TickeUserMaps.Include(y => y.Payment).Include(m => m.EventTicket).FirstOrDefault(x => x.TicketId == i.TicketId && x.UserId == ManageSession.UserSession.Id && x.Payment.Status == (int)PaymentStatus.PaymentSuccess && DbFunctions.AddDays(x.Payment.PaymentDate, validd.ValidDays) >= DateTime.Now);
                        //            if (res != null)
                        //            {
                        //                i.IsPurchased = true;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            var res = dbConn.TickeUserMaps.Include(y => y.Payment).Include(m => m.EventTicket).FirstOrDefault(x => x.TicketId == i.TicketId && x.UserId == ManageSession.UserSession.Id && x.Payment.Status == (int)PaymentStatus.PaymentSuccess);

                        //            if (res != null)
                        //            {
                        //                i.IsPurchased = true;
                        //            }
                        //        }

                        //    }

                        //    ////var res1 = dbConn.MultimediaContents.FirstOrDefault(x => x.Event_Id == events.Id);
                        //    ////var expire = Convert.ToDateTime(res.CreateDate).AddDays(Convert.ToInt32(res1.validdays));
                        //    ////bool isexpire = DateTime.UtcNow.Date <= expire;

                        //}
                    }
                    else
                    {
                        result.Tickest = (from t in events.EventTickets
                                          where t.Inviation_Id == null && t.IsEnable == true && t.Id == TicketId
                                          select new TicketPrice
                                          {
                                              TicketId = t.Id,
                                              TicketType = t.Ticket_Type,
                                              TicketName = t.TicketName,
                                              AvailableQnty = t.AvailableQuantity,
                                              Price = t.Price,
                                              IsEnable = t.IsEnable,
                                              PaymentType = Convert.ToInt32(t.PaymentCurrency == null ? 1 : t.PaymentCurrency)
                                          }).OrderBy(x => x.PaymentType).ToList();
                    }

                    //var cont = (from r in dbConn.Users where r.Id == ManageSession.UserSession.Id select new ContactDetails { Email = r.Email, Mobile = r.PhoneNo, name = r.FirstName }).FirstOrDefault();
                    //result.ContactDetail = cont;
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public static bool CheckTicketAvailable()
        {
            bool Availability = false;
            ApiResponse response = new ApiResponse();
            try
            {

                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    if (ManageSession.TicketCartSession != null && ManageSession.TicketCartSession.TickeCarts != null)
                    {
                        foreach (var item in ManageSession.TicketCartSession.TickeCarts)
                        {
                            var TotalTicket = dbConn.EventTickets.Where(x => x.Id == item.TicketId).FirstOrDefault();
                            var check = dbConn.TickeUserMaps.Where(x => x.TicketId == item.TicketId && x.Status == (int)PaymentStatus.PaymentSuccess).ToList();


                            if (TotalTicket != null)
                            {
                                var Totaltkt = TotalTicket.Quantity;

                                if (check != null && check.Count > 0)
                                {
                                    var TotalTicketBought = check.Sum(a => a.Qty) + item.Qnty;

                                    if (Totaltkt >= TotalTicketBought)
                                    {
                                        Availability = true;
                                    }
                                    else
                                    {
                                        Availability = false;
                                        return Availability;
                                    }
                                }
                                else
                                {
                                    var TotalTicketBought = item.Qnty;
                                    if (Totaltkt >= TotalTicketBought)
                                    {
                                        Availability = true;
                                    }
                                    else
                                    {
                                        Availability = false;
                                        return Availability;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return Availability;
        }

        public static bool RemoveNotAddedSessions()
        {
            bool result = true;
            try
            {
                foreach (var item in ManageSession.TicketCartSession.TickeCarts)
                {
                    if (item.TicketAdded == false)
                    {
                        ManageSession.TicketCartSession.TickeCarts.Remove(item);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public static User_ GetEmailResponse(string EmailId)
        {
            // ApiResponse response = new ApiResponse();
            User_ model = new User_();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var check = dbConn.Users.Where(x => x.Email.ToLower().Trim() == EmailId.ToLower().Trim() && x.UserType == (int)UserType.users && x.UserStatus == (int)userstatus.active).FirstOrDefault();
                    if (check != null)
                    {
                        model.Id = check.Id;
                        model.PhoneNo = check.PhoneNo;

                    }
                    else
                    {
                        model.Id = 0;
                        model.PhoneNo = "0";

                    }
                }
            }
            catch (Exception ex)
            {
            }
            return model;
        }

        public static UserSession InsertUserIdIntoSession(TicketModelPopup model)
        {
            UserSession list = new UserSession();
            // bool IsResult = false;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    if (model.UserId != null)
                    {
                        var ID = Convert.ToInt64(model.UserId);
                        var res = dbConn.Users.Where(x => x.Id == ID && x.UserType == (int)UserType.users && x.UserStatus == (int)userstatus.active).FirstOrDefault();

                        if (res != null)
                        {
                            res.Token = Guid.NewGuid().ToString();
                            dbConn.SaveChanges();
                            list.Id = res.Id; list.FirstName = res.FirstName; list.LastName = res.LastName; list.EmailId = res.Email; list.country = res.CountryId; list.Status = res.UserStatus; list.PhoneNo = res.PhoneNo; list.PhoneCode = res.Phone_CountryCode != null ? res.Phone_CountryCode : "233"; list.Token = res.Token;
                            return list;
                        }
                        else
                        {
                            User user = new User();
                            user.Email = model.EmailAddress;
                            user.FirstName = model.FirstName;
                            user.LastName = model.LastName;
                            user.CountryId = 233;
                            user.UserStatus = (int)userstatus.active;
                            user.PhoneNo = model.MobileNumber;
                            user.UserType = (int)UserType.users;
                            user.Token = Guid.NewGuid().ToString();

                            dbConn.Users.Add(user);
                            if (dbConn.SaveChanges() > 0)
                            {
                                list.Id = user.Id; user.FirstName = user.FirstName; list.LastName = user.LastName; list.EmailId = user.Email; list.country = user.CountryId; list.Status = user.UserStatus; list.PhoneNo = user.PhoneNo; list.PhoneCode = user.Phone_CountryCode != null ? user.Phone_CountryCode : "233"; list.Token = user.Token;
                                return list;
                            }
                        }
                    }
                    else
                    {
                        var Email = dbConn.Users.Where(x => x.Email.ToLower().Trim() == model.EmailAddress.ToLower().Trim() && x.UserType == (int)UserType.users && x.UserStatus == (int)userstatus.active).FirstOrDefault();
                        if (Email != null)
                        {
                            Email.Token = Guid.NewGuid().ToString();
                            dbConn.SaveChanges();
                            list.Id = Email.Id; list.FirstName = Email.FirstName; list.LastName = Email.LastName; list.EmailId = Email.Email; list.country = Email.CountryId; list.Status = Email.UserStatus; list.PhoneNo = Email.PhoneNo; list.PhoneCode = Email.Phone_CountryCode != null ? Email.Phone_CountryCode : "233"; list.Token = Email.Token;
                            return list;
                        }
                        else
                        {


                            User user = new User();
                            user.Email = model.EmailAddress;
                            user.FirstName = model.FirstName;
                            user.LastName = model.LastName;
                            user.CountryId = 233;
                            user.UserStatus = (int)userstatus.active;
                            user.PhoneNo = model.MobileNumber;
                            user.UserType = (int)UserType.users;
                            user.Token = Guid.NewGuid().ToString();

                            dbConn.Users.Add(user);
                            if (dbConn.SaveChanges() > 0)
                            {
                                list.Id = user.Id; user.FirstName = user.FirstName; list.LastName = user.LastName; list.EmailId = user.Email; list.country = user.CountryId; list.Status = user.UserStatus; list.PhoneNo = user.PhoneNo; list.PhoneCode = user.Phone_CountryCode != null ? user.Phone_CountryCode : "233"; list.Token = user.Token;
                             
                                return list;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
            }

            return list;
        }

        public static bool InsertSessions(int UserID = 0)
        {
            bool Result = false;
            try
            {
                if (UserID > 0)
                {
                    using (EventmanagerEntities dbConn = new EventmanagerEntities())
                    {
                        var login = dbConn.Users.Where(x => x.Id == UserID).FirstOrDefault();
                        ManageSession.UserSession = new UserSession();
                        ManageSession.UserSession.FirstName = login.FirstName;
                        ManageSession.UserSession.LastName = login.LastName;
                        ManageSession.UserSession.EmailId = login.Email;
                        ManageSession.UserSession.Id = login.Id;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Result;
        }

    }
}