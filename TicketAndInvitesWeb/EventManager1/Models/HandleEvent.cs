using EventManager1.Areas.Organizer.Models;
using EventManager1.DBCon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Plivo.XML;
using System.Runtime.CompilerServices;
using System.Web.Hosting;
using System.IO;

namespace EventManager1.Models
{

    public class HandleEvent
    {
        EventmanagerEntities db;
        public ApiResponse EventRegister(Event_ ce)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            db = new EventmanagerEntities();
            try
            {
                Event events = new Event();

                if (ce.Id > 0)
                {
                    events = db.Events.FirstOrDefault(m => m.Id == ce.Id);
                    events.Status = events.Status;
                    events.Updated_at = DateTime.UtcNow;
                }
                events.Event_name = ce.EventName;
                events.Eventtype = ce.Eventtype;
                events.eventCatg = ce.EventCategory;
                events.streamType = 2;
                if (ce.BusinessOwner_Id > 0)
                {
                    events.Company_Id = ce.BusinessOwner_Id;
                }
                else
                {
                    events.User_Id = ce.User_Id;
                }
                events.Venue = ce.Venue;
                if (events.PublishDate != null)
                {
                    events.PublishDate = Convert.ToDateTime(ce.PublishDate);
                }
                else { events.PublishDate = DateTime.UtcNow; }
                if (ce.Id > 0)
                {
                    events.PublishDate = DateTime.UtcNow;
                    events.LiveURL = ce.LiveURL;
                }
                events.StartDate = Convert.ToDateTime(ce.StartDate + " " + ce.StartTime);
                events.EndDate = Convert.ToDateTime(ce.EndDate + " " + ce.EndTime);
                events.longitude = Convert.ToDecimal(ce.address_.FirstOrDefault().longitude);
                events.latitude = Convert.ToDecimal(ce.address_.FirstOrDefault().latitude);
                events.CreatedOnWebsite = (int)Ticketforwebsite.Stream233; // 1 for tickets and invites 2 for roverman
                events.Description = ce.Description;
                if (ce.Id == 0)
                {
                    events.Status = (int)EventStatus.Active;
                    events.Created_at = DateTime.UtcNow;
                    //events.LiveURL = CommonDbLogic.RandomString(6)+DateTime.Now.Second;
                    events.LiveURL = ce.LiveURL;
                    Address addr = new Address();
                    addr.AddressLine = ce.address_[0].AddressLine;
                    addr.AddressType = 1;
                    addr.City = ce.address_[0].City;
                    addr.Country_Id = ce.address_[0].Country;
                    addr.ZipCode = ce.address_[0].ZipCode;
                    addr.longitude = ce.address_.FirstOrDefault().longitude;
                    addr.latitude = ce.address_.FirstOrDefault().latitude;
                    events.Address = addr;

                    foreach (var i in ce.Multimedia)
                    {
                        MultimediaContent b = new MultimediaContent();
                        b.Mul_Type = i.Mul_Type;
                        b.URL = i.URL;
                        b.Created_at = DateTime.Now;
                        b.Mul_MainPic = i.Mul_MainPic;
                        b.videotype = i.videotype;
                        if (b.URL != null && b.Mul_Type == 2)
                        {
                            b.videoId = getdynamic_number();
                        }
                        events.MultimediaContents.Add(b);
                    }
                    foreach (var o in ce.Rsvp)
                    {
                        events.RSVPs.Add(new RSVP { Namer = o.Namer, Phone = o.Phone });
                    }
                    db.Events.Add(events);
                }
                else
                {

                    events.Address.AddressLine = ce.address_[0].AddressLine;
                    events.Address.AddressType = 1;
                    events.Address.City = ce.address_[0].City;
                    events.Address.Country_Id = ce.address_[0].Country;
                    events.Address.ZipCode = ce.address_[0].ZipCode;
                    events.Address.longitude = ce.address_.FirstOrDefault().longitude;
                    events.Address.latitude = ce.address_.FirstOrDefault().latitude;

                    for (int i = 0; i < 5; i++)
                    {
                        if (i < ce.Multimedia.Count())
                        {
                            var obj = events.MultimediaContents.Skip(i).FirstOrDefault();
                            if (obj != null)
                            {
                                obj.URL = ce.Multimedia[i].URL;
                                obj.Updated_at = DateTime.Now;
                                obj.Mul_Type = ce.Multimedia[i].Mul_Type;
                                obj.Mul_MainPic = ce.Multimedia[i].Mul_MainPic;
                                //if (obj.Mul_Type == 2)
                                //{
                                //    obj.videoId = getdynamic_number();
                                //}
                                obj.videotype = ce.Multimedia[i].videotype;
                            }
                            else
                            {
                                // events.MultimediaContents.Add(new MultimediaContent { URL = ce.Multimedia[i].URL, Mul_Type = ce.Multimedia[i].Mul_Type, Updated_at = DateTime.Now });
                                EventmanagerEntities dbs = new EventmanagerEntities();
                                MultimediaContent b = new MultimediaContent();
                                b.Mul_Type = ce.Multimedia[i].Mul_Type;
                                b.URL = ce.Multimedia[i].URL;
                                b.Mul_MainPic = ce.Multimedia[i].Mul_MainPic;
                                b.videotype = ce.Multimedia[i].videotype;
                                b.Created_at = DateTime.Now;
                                b.Event_Id = events.Id;
                                //b.videoId = getdynamic_number();
                                dbs.MultimediaContents.Add(b);
                                dbs.SaveChanges();
                            }
                        }
                        else
                        {
                            var obj = events.MultimediaContents.Skip(i).FirstOrDefault();
                            if (obj != null)
                            {
                                EventmanagerEntities dbs = new EventmanagerEntities();
                                var mul = dbs.MultimediaContents.FirstOrDefault(x => x.Id == obj.Id);
                                dbs.MultimediaContents.Remove(mul);
                                dbs.SaveChanges();
                            }
                        }

                    }
                    for (int i = 0; i < ce.Rsvp.Count(); i++)
                    {
                        //var rsvpcount = events.RSVPs;
                        //if (rsvpcount > ce.Rsvp.Count()) { }
                        var obj = events.RSVPs.Skip(i).FirstOrDefault();
                        if (obj != null)
                        {
                            obj.Namer = ce.Rsvp[i].Namer;
                            obj.Phone = ce.Rsvp[i].Phone;
                        }
                        else
                        {
                            //events.RSVPs.Add(new RSVP { Namer = ce.Rsvp[i].Namer, Phone = ce.Rsvp[i].Phone });
                            EventmanagerEntities dbs = new EventmanagerEntities();
                            RSVP rsvp = new RSVP();
                            rsvp.Namer = ce.Rsvp[i].Namer;
                            rsvp.Phone = ce.Rsvp[i].Phone;
                            rsvp.Event_Id = events.Id;
                            dbs.RSVPs.Add(rsvp);
                            dbs.SaveChanges();
                        }


                    }
                }
                if (db.SaveChanges() > 0)
                {
                    Resp.Code = (int)ApiResponseCode.ok;
                }
                else { Resp.Msg = "Oops! Something went wrong."; }

            }
            catch (Exception ex) { Resp.Msg = ex.Message; }
            return Resp;
        }
        public string getdynamic_number()
        {
            db = new EventmanagerEntities();
            string num = "";
            try
            {
                var numlist = db.MultimediaContents.Where(x => x.videoId != null).ToList();
                List<string> str = numlist.Select(x => x.videoId).ToList();
                if (str != null && str.Count() > 0)
                {
                    do
                    {
                        num = RandomString(10);
                    } while (str.Contains(num));
                }
                else { num = RandomString(10); }
            }
            catch (Exception)
            {

            }
            return num;

        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public ApiResponse CreateTicket(createTicket ce)
        {
            db = new EventmanagerEntities();
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {

                foreach (var i in ce.Ticket_)
                {
                    EventTicket b = new EventTicket();
                    if (i.Id > 0)
                    {
                        b = db.EventTickets.FirstOrDefault(x => x.Id == i.Id);
                        var uqty = i.Quantity - b.Quantity;
                        if (uqty > 0)
                        {
                            b.AvailableQuantity = i.Quantity + uqty;
                        }

                    }
                    else
                    {
                        b.AvailableQuantity = i.Quantity;
                    }

                    if (i.TicketTypeText != null)
                    {
                        i.TicketType = CreateTicketTypes(i.TicketTypeText);
                    }
                    //var mul = db.MultimediaContents.FirstOrDefault(x => x.Event_Id == ce.EventId && x.videotype == 1);
                    //mul.validdays = i.validdays;
                    b.Event_Id = ce.EventId;
                    b.TicketName = i.TicketName;
                    b.Ticket_Type = i.TicketType;
                    b.Price = i.Price;
                    b.Quantity = i.Quantity;
                    b.ValidDays = i.validdays;
                    b.Created_at = DateTime.Now;
                    b.Seat = i.Seat;
                    b.tableNo = i.table;
                    b.ColorArea = i.ColorArea;
                    b.TicketStatus = (int)TicketStatus.Active;
                    b.Section_Details = i.ColorArea;
                    // b.TicketName = i.TicketName;
                    b.GateNo = i.GateNo;
                    b.IsEnable = i.IsEnable;
                    if (i.Id == 0)
                    {
                        db.EventTickets.Add(b);
                    }            //db.SaveChanges();
                                 //var events = db.Events.Where(a => a.Id == ce.EventId).FirstOrDefault();
                                 //events.Status = (int)EventStatus.Pending;
                    if (db.SaveChanges() > 0)
                    {
                        Resp.Code = (int)ApiResponseCode.ok;
                        Resp.Msg = b.Id.ToString();
                    }
                    else { Resp.Msg = "Oops! Something went wrong."; }
                }

            }
            catch (Exception ex) { Resp.Msg = ex.Message; }
            return Resp;
        }
        public ApiResponse DeleteTicket(int id)
        {
            db = new EventmanagerEntities();
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (id > 0)
                {
                    EventTicket b = new EventTicket();
                    var eticket = new EventTicket { Id = id };
                    db.EventTickets.Attach(eticket);
                    db.EventTickets.Remove(eticket);
                }

                if (db.SaveChanges() > 0)
                {
                    Resp.Code = (int)ApiResponseCode.ok;
                }
                else { Resp.Msg = "Oops! Something went wrong."; }

            }
            catch (Exception ex) { Resp.Msg = ex.Message; }
            return Resp;
        }
        public ApiResponse UpdateTicket(int id, int qty)
        {
            db = new EventmanagerEntities();
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (id > 0)
                {
                    var tic = db.EventTickets.FirstOrDefault(x => x.Id == id);
                    var tickets = qty - tic.Quantity;
                    if (tickets > 0)
                    {
                        tic.Quantity = qty; tic.AvailableQuantity = tic.AvailableQuantity + tickets;
                    }

                    if (db.SaveChanges() > 0)
                    {
                        Resp.Code = (int)ApiResponseCode.ok;
                    }
                }
                else { Resp.Msg = "Oops! Something went wrong."; }

            }
            catch (Exception ex) { Resp.Msg = ex.Message; }
            return Resp;
        }
        public ApiResponse UpdateTicketstate(int id, bool isenable)
        {
            db = new EventmanagerEntities();
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (id > 0)
                {
                    var tic = db.EventTickets.FirstOrDefault(x => x.Id == id);
                    tic.IsEnable = isenable;
                    if (db.SaveChanges() > 0)
                    {

                    }
                    Resp.Code = (int)ApiResponseCode.ok;
                }
                else { Resp.Msg = "Oops! Something went wrong."; }

            }
            catch (Exception ex) { Resp.Msg = ex.Message; }
            return Resp;
        }
        public ApiResponse CreateTicketType(string ticket)
        {
            db = new EventmanagerEntities();
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                DBCon.TicketType b = new DBCon.TicketType();
                b.TicketTypes = ticket;
                db.TicketTypes.Add(b);

                if (db.SaveChanges() > 0)
                {
                    Resp.Code = (int)ApiResponseCode.ok;
                }
                else { Resp.Msg = "Oops! Something went wrong."; }

            }
            catch (Exception ex) { Resp.Msg = ex.Message; }
            return Resp;
        }
        public int CreateTicketTypes(string ticket)
        {
            db = new EventmanagerEntities();
            int tickettype;
            try
            {
                DBCon.TicketType b = new DBCon.TicketType();
                b.TicketTypes = ticket;
                db.TicketTypes.Add(b);

                if (db.SaveChanges() > 0)
                {
                    tickettype = b.id;
                }
                else { tickettype = 0; }

            }
            catch (Exception ex) { tickettype = 0; }
            return tickettype;
        }

        public List<Event_> GetEvents(int id, int user = 0)
        {
            db = new EventmanagerEntities();
            List<Event_> ct = new List<Event_>();
            List<Event> cont = new List<Event>();
            if (user > 0)
            {
                cont = db.Events.Include("EventTickets").Include("MultimediaContents").Include("RSVPs").Where(s => s.User_Id == id && s.Status != 0 && s.CreatedOnWebsite == (int)Ticketforwebsite.Stream233).ToList();
            }
            else { cont = db.Events.Include("EventTickets").Include("MultimediaContents").Include("RSVPs").Where(s => s.Company_Id == id && s.Status != 0 && s.CreatedOnWebsite == (int)Ticketforwebsite.Stream233).ToList(); }
            var ticmap = db.TickeUserMaps;
            foreach (var i in cont)
            {
                //  var mul = db.MultimediaContents.Where(s => s.Event_Id == i.Id).ToList().Take(1);
                List<Multimedia_Content> mlist = new List<Multimedia_Content>();
                List<EventTicket_> eticket = new List<EventTicket_>();
                List<RSVPModel> rsvp = new List<RSVPModel>();
                foreach (var u in i.MultimediaContents) { mlist.Add(new Multimedia_Content { URL = u.URL, Mul_MainPic = Convert.ToBoolean(u.Mul_MainPic) }); }
                foreach (var u in i.EventTickets)
                {
                    if (u.TicketStatus == (int)TicketStatus.Active)
                        eticket.Add(new EventTicket_ { TicketName = u.TicketName, Quantity = Convert.ToInt32(u.Quantity), Seat = ticmap != null && ticmap.Count() > 0 ? ticmap.Where(x => x.TicketId == u.Id && (x.Status == 1 || x.Status == 2)).Sum(y => y.Qty).ToString() : "0" });
                }
                foreach (var u in i.RSVPs) { rsvp.Add(new RSVPModel { Namer = u.Namer, Phone = u.Phone }); }

                ct.Add(new Event_
                {
                    Id = i.Id,
                    EventName = i.Event_name,
                    Eventtype = Convert.ToInt32(i.Eventtype),
                    Venue = i.Venue,
                    Description = i.Description,
                    BusinessOwner_Id = Convert.ToInt32(i.Company_Id),
                    StartDate = i.StartDate.ToString(),
                    EndDate = i.EndDate.ToString(),
                    Tickets = eticket,
                    Multimedia = mlist,
                    Total_Invitaion = db.Invitations.Where(x => x.Event_Id == i.Id).Count(),
                    Rsvp = rsvp,
                    status = i.Status.ToString() //Enum.GetName(typeof(EventStatus), Convert.ToInt32(i.Status))
                });

            };
            return ct;
        }

        public List<EventTicket_> Gettickets(int id)
        {
            db = new EventmanagerEntities();
            List<EventTicket_> ct = new List<EventTicket_>();
            var cont = db.EventTickets.Where(s => s.Event_Id == id).ToList();
            foreach (var i in cont)
            {
                //  var mul = db.MultimediaContents.Where(s => s.Event_Id == i.Id).ToList().Take(1); 
                try
                {
                    if (i.TicketStatus == (int)TicketStatus.Active)
                    {
                        ct.Add(new EventTicket_
                        {
                            Id = i.Id,
                            TicketName = i.TicketName,
                            Seat = i.Seat,
                            ColorArea = i.ColorArea,
                            GateNo = i.GateNo != null ? i.GateNo.ToString() : "",
                            Quantity = Convert.ToInt32(i.Quantity),
                            table = i.tableNo,
                            Price = Convert.ToDecimal(i.Price),
                            TicketType = Convert.ToInt32(i.Ticket_Type)
                        });
                    }
                }
                catch { }
            };
            return ct;
        }

        public List<EventTicket_> GetPurchasedTicket(int id, int PaymentId)
        {
            db = new EventmanagerEntities();
            List<EventTicket_> ct = new List<EventTicket_>();
            //var cont = db.Events.Include("EventTickets").Where(s => s.Event_Id == id).ToList();
            var tick = db.TickeUserMaps.Include("Payment").Where(x => x.UserId == id && x.Qty > 0 && x.IsTicketSendToUser == false && x.PaymemtId == PaymentId && x.Payment.Status == (int)PaymentStatus.PaymentSuccess).OrderByDescending(a => a.CreateDate).ToList();

            foreach (var i in tick)
            {
                var tic = db.EventTickets.Where(x => x.Id == i.TicketId).FirstOrDefault();
                ct.Add(new EventTicket_
                {
                    Quantity = Convert.ToInt32(i.Qty),
                    Barcode = i.BarCodeNumber,
                    ColorArea = tic.ColorArea,
                    GateNo = tic.GateNo,
                    TicketName = tic.TicketName,
                    TicketId = i.TicketId,
                    UserId = (int)i.UserId,
                    TicketmapId = i.Id,
                    ticketdate = (DateTime)tic.StartDate

                });
            }

            return ct;
        }

        public List<Event_> GetUserTickets(int id, int comId)
        {
            db = new EventmanagerEntities();
            List<Event_> ct = new List<Event_>();
            //var cont = db.Events.Include("EventTickets").Where(s => s.Event_Id == id).ToList();
            var tick = db.TickeUserMaps.Include("Payment").Where(x => x.UserId == id && x.Payment.Status == (int)PaymentStatus.PaymentSuccess && x.Qty > 0 && x.Payment.CompanyId == comId).OrderByDescending(a => a.CreateDate).ToList();
            var tickettype = db.TicketTypes;
            foreach (var i in tick)
            {
                List<Multimedia_Content> mlist = new List<Multimedia_Content>();
                List<EventTicket_> tickett = new List<EventTicket_>();
                try
                {
                    var ticket = db.EventTickets.Where(x => x.Id == i.TicketId && x.Event.Company_Id == comId).ToList();
                    foreach (var tic in ticket)
                    {
                        var tickets = tic.Event_Id;
                        var e = db.Events.Include("MultimediaContents").FirstOrDefault(x => x.Id == tickets && x.Status > 0);
                        string url;
                        if (e != null)
                        {
                            var urls = e.MultimediaContents.FirstOrDefault(x => x.Mul_MainPic == true);

                            if (urls == null)
                            {
                                url = e.MultimediaContents.FirstOrDefault().URL;
                            }
                            else
                            {
                                url = urls.URL;
                            }
                            string BarcodePath = HostingEnvironment.MapPath("~/Content/BarCode/" + i.BarCodeNumber + ".jpg");
                            if (File.Exists(BarcodePath))
                            {
                            }
                            else
                            {
                                if (i.BarCodeNumber == null)
                                {
                                    i.BarCodeNumber = common.GetUniqueBarCode(i.TicketId);
                                }
                                common.GenerateBarcode(i.BarCodeNumber);
                            }
                            //mlist.Add(new Multimedia_Content { URL = url, Event_Id = Convert.ToInt32(urls.Event_Id), videoId = e.MultimediaContents.FirstOrDefault(x => x.videotype == 1).videoId });
                            mlist.Add(new Multimedia_Content { URL = url, Event_Id = Convert.ToInt32(e.Id), videoId = e.MultimediaContents.FirstOrDefault(x => x.videotype == 1).videoId });
                            tickett.Add(new EventTicket_
                            {
                                Quantity = Convert.ToInt32(i.Qty),
                                Barcode = i.BarCodeNumber,
                                ColorArea = tic.ColorArea,
                                GateNo = tic.GateNo,
                                TicketName = tic.TicketName,
                                TicketId = tic.Id,
                                UserId = (int)i.UserId,
                                TicketmapId = i.Id,
                                ticketdate = (DateTime)tic.StartDate
                            }); //tickettype.FirstOrDefault(y=>y.id == tic.Ticket_Type).TicketTypes
                            ct.Add(new Event_
                            {
                                Id = i.Id,
                                EventName = e.Event_name,
                                Eventtype = Convert.ToInt32(e.Eventtype),
                                Multimedia = mlist,
                                EventId = e.Id,
                                UserName = i.Name,
                                streamType = e.streamType,
                                StartDate = Convert.ToDateTime(e.StartDate).ToString(),
                                Venue = e.Venue,
                                BusinessOwner_Id = comId,
                                EndDate = Convert.ToDateTime(e.EndDate).ToString(),
                                status = Convert.ToInt32(e.Status).ToString(),
                                Tickets = tickett,
                                CityName = db.Addresses.FirstOrDefault(x => x.Id == e.Address_Id).AddressLine,
                                paymentStatus = i.Payment.Status,
                                purchaseDate = Convert.ToDateTime(i.Payment.PaymentDate),
                                evid = EnryptString(e.Id.ToString()),
                                TicketDate = Convert.ToDateTime(tic.StartDate).ToString(),

                            });
                        }
                    }
                }
                catch (Exception ex) { }
            }
            return ct;
            //foreach (var i in cont)
            //{
            //    //  var mul = db.MultimediaContents.Where(s => s.Event_Id == i.Id).ToList().Take(1);                
            //    ct.Add(new EventTicket_
            //    {
            //        Id = i.Id,
            //        TicketName = i.TicketName,
            //        Seat = i.Seat,
            //        ColorArea = i.ColorArea,
            //        GateNo = i.GateNo.ToString(),
            //        Quantity = Convert.ToInt32(i.Quantity),
            //        table = i.tableNo,
            //        Price = Convert.ToDecimal(i.Price),
            //        TicketType = Convert.ToInt32(i.Ticket_Type)
            //    });
            //};


        }

        public List<Event_> GetUserTickets(int id)
        {
            db = new EventmanagerEntities();
            List<Event_> ct = new List<Event_>();
            //var cont = db.Events.Include("EventTickets").Where(s => s.Event_Id == id).ToList();
            var tick = db.TickeUserMaps.Include("Payment").Where(x => x.UserId == id && x.Payment.Status == (int)PaymentStatus.PaymentSuccess && x.Qty > 0).OrderByDescending(a => a.CreateDate).ToList();
            var tickettype = db.TicketTypes;
            foreach (var i in tick)
            {
                List<Multimedia_Content> mlist = new List<Multimedia_Content>();
                List<EventTicket_> tickett = new List<EventTicket_>();
                try
                {
                    var tic = db.EventTickets.Where(x => x.Id == i.TicketId).FirstOrDefault();
                    var tickets = tic.Event_Id;
                    var e = db.Events.Include("MultimediaContents").FirstOrDefault(x => x.Id == tickets && x.Status > 0);
                    string url;
                    if (e != null)
                    {
                        var urls = e.MultimediaContents.FirstOrDefault(x => x.Mul_MainPic == true);

                        if (urls == null)
                        {
                            url = e.MultimediaContents.FirstOrDefault().URL;
                        }
                        else
                        {
                            url = urls.URL;
                        }
                        string BarcodePath = HostingEnvironment.MapPath("~/Content/BarCode/" + i.BarCodeNumber + ".jpg");
                        if (File.Exists(BarcodePath))
                        {
                        }
                        else
                        {
                            if (i.BarCodeNumber == null)
                            {
                                i.BarCodeNumber = common.GetUniqueBarCode(i.TicketId);
                            }
                            common.GenerateBarcode(i.BarCodeNumber);
                        }
                        //mlist.Add(new Multimedia_Content { URL = url, Event_Id = Convert.ToInt32(urls.Event_Id), videoId = e.MultimediaContents.FirstOrDefault(x => x.videotype == 1).videoId });
                        mlist.Add(new Multimedia_Content { URL = url, Event_Id = Convert.ToInt32(e.Id), videoId = e.MultimediaContents.FirstOrDefault(x => x.videotype == 1).videoId });
                        tickett.Add(new EventTicket_
                        {
                            Quantity = Convert.ToInt32(i.Qty),
                            Barcode = i.BarCodeNumber,
                            ColorArea = tic.ColorArea,
                            GateNo = tic.GateNo,
                            TicketName = tic.TicketName,
                            TicketId = tic.Id,
                            UserId = (int)i.UserId,
                            TicketmapId = i.Id
                        }); //tickettype.FirstOrDefault(y=>y.id == tic.Ticket_Type).TicketTypes
                        ct.Add(new Event_
                        {
                            Id = i.Id,
                            EventName = e.Event_name,
                            Eventtype = Convert.ToInt32(e.Eventtype),
                            Multimedia = mlist,
                            EventId = e.Id,
                            UserName = i.Name,
                            streamType = e.streamType,
                            StartDate = Convert.ToDateTime(e.StartDate).ToString(),
                            Venue = e.Venue,
                            BusinessOwner_Id = i.Id,
                            EndDate = Convert.ToDateTime(e.EndDate).ToString(),
                            status = Convert.ToInt32(e.Status).ToString(),
                            Tickets = tickett,
                            CityName = db.Addresses.FirstOrDefault(x => x.Id == e.Address_Id).AddressLine,
                            paymentStatus = i.Payment.Status,
                            purchaseDate = Convert.ToDateTime(i.Payment.PaymentDate),
                            evid = EnryptString(e.Id.ToString()),
                        });
                    }
                }
                catch (Exception ex) { }
            }
            return ct;
        }

        public List<Event_> GetUserTicketsNew(int id)
        {
            db = new EventmanagerEntities();
            List<Event_> ct = new List<Event_>();
            //var cont = db.Events.Include("EventTickets").Where(s => s.Event_Id == id).ToList();
            var tick = db.TickeUserMaps.Where(x => x.UserId == id && (x.Status == 1 || x.Status == 2 || x.Status == 0) && x.Qty > 0).OrderByDescending(a => a.CreateDate).ToList();
            var tickettype = db.TicketTypes;
            foreach (var i in tick)
            {
                List<Multimedia_Content> mlist = new List<Multimedia_Content>();
                List<EventTicket_> tickett = new List<EventTicket_>();
                try
                {
                    var tic = db.EventTickets.Where(x => x.Id == i.TicketId).FirstOrDefault();
                    var tickets = tic.Event_Id;
                    var e = db.Events.Include("MultimediaContents").FirstOrDefault(x => x.Id == tickets && x.Status > 0 && x.Eventtype != 2);
                    string url;
                    if (e != null)
                    {
                        var urls = e.MultimediaContents.FirstOrDefault(x => x.Mul_MainPic == true);

                        if (urls == null)
                        {
                            url = e.MultimediaContents.FirstOrDefault().URL;
                        }
                        else
                        {
                            url = urls.URL;
                        }
                        string BarcodePath = HostingEnvironment.MapPath("~/Content/BarCode/" + i.BarCodeNumber + ".jpg");
                        if (File.Exists(BarcodePath))
                        {
                        }
                        else
                        {
                            if (i.BarCodeNumber == null)
                            {
                                i.BarCodeNumber = common.GetUniqueBarCode(i.TicketId);
                            }
                            common.GenerateBarcode(i.BarCodeNumber);
                        }
                        //mlist.Add(new Multimedia_Content { URL = url, Event_Id = Convert.ToInt32(urls.Event_Id), videoId = e.MultimediaContents.FirstOrDefault(x => x.videotype == 1).videoId });
                        mlist.Add(new Multimedia_Content { URL = url, Event_Id = Convert.ToInt32(e.Id), videoId = e.MultimediaContents.FirstOrDefault(x => x.videotype == 1).videoId });
                        tickett.Add(new EventTicket_
                        {
                            Quantity = Convert.ToInt32(i.Qty),
                            Barcode = i.BarCodeNumber,
                            ColorArea = tic.ColorArea,
                            GateNo = tic.GateNo,
                            TicketName = tic.TicketName
                        }); //tickettype.FirstOrDefault(y=>y.id == tic.Ticket_Type).TicketTypes
                        ct.Add(new Event_
                        {
                            Id = i.Id,
                            EventName = e.Event_name,
                            Eventtype = Convert.ToInt32(e.Eventtype),
                            Multimedia = mlist,
                            EventId = e.Id,
                            UserName = i.Name,
                            streamType = e.streamType,
                            StartDate = Convert.ToDateTime(e.StartDate).ToString(),
                            Venue = e.Venue,
                            BusinessOwner_Id = i.Id,
                            EndDate = Convert.ToDateTime(e.EndDate).ToString(),
                            status = Convert.ToInt32(e.Status).ToString(),
                            Tickets = tickett,
                            CityName = db.Addresses.FirstOrDefault(x => x.Id == e.Address_Id).AddressLine,
                            paymentStatus = i.Payment.Status,
                            purchaseDate = Convert.ToDateTime(i.Payment.PaymentDate),
                            evid = EnryptString(e.Id.ToString()),
                        });
                    }
                }
                catch (Exception ex) { }
            }
            return ct;
            //foreach (var i in cont)
            //{
            //    //  var mul = db.MultimediaContents.Where(s => s.Event_Id == i.Id).ToList().Take(1);                
            //    ct.Add(new EventTicket_
            //    {
            //        Id = i.Id,
            //        TicketName = i.TicketName,
            //        Seat = i.Seat,
            //        ColorArea = i.ColorArea,
            //        GateNo = i.GateNo.ToString(),
            //        Quantity = Convert.ToInt32(i.Quantity),
            //        table = i.tableNo,
            //        Price = Convert.ToDecimal(i.Price),
            //        TicketType = Convert.ToInt32(i.Ticket_Type)
            //    });
            //};

        }
        public string EnryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }
        public string Getimageurl(int id)
        {
            db = new EventmanagerEntities();
            string url = db.MultimediaContents.FirstOrDefault(x => x.Event_Id == id).URL;
            return url;
        }
        public string GetInvitaionfee(string page)
        {
            db = new EventmanagerEntities();
            string fee;
            if (page == "Invitation")
            {
                fee = db.PaymentSetups.FirstOrDefault().InvitationOrg.ToString();
            }
            else if (page == "BroadCast")
            {
                fee = db.PaymentSetups.FirstOrDefault().Broadcast.ToString();
            }
            else
            {
                fee = db.PaymentSetups.FirstOrDefault().InvitationUser.ToString();
            }
            return fee;
        }
        public List<TransactionHistory> GettransHistory(int type, string startdate, string enddate)
        {
            db = new EventmanagerEntities();

            List<TransactionHistory> ct = new List<TransactionHistory>();
            var cont = db.Events.Include("EventTickets").Where(s => s.Company_Id == ManageSession.CompanySession.CompanyId).ToList();

            //var adminfee = db.PaymentSetups.FirstOrDefault().Adminfee;
            int days = -7;
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
            var sdate = DateTime.Now.AddDays(days);
            var payments = db.TickeUserMaps.Include(y => y.Payment).Where(x => x.Amount > 0 && x.CreateDate >= sdate && x.Payment.Status == 1).ToList();
            foreach (var i in cont)
            {
                var tickets = i.EventTickets.Where(x => x.Ticket_Type != 2);

                foreach (var j in tickets)
                {
                    List<TickeUserMap> tic = new List<TickeUserMap>();
                    if (type == 3) { tic = payments.Where(y => y.TicketId == j.Id && (y.Status == 1) && y.Qty > 0 && y.CreateDate >= Convert.ToDateTime(startdate) && y.CreateDate <= Convert.ToDateTime(enddate)).ToList(); }
                    else
                    {
                        tic = payments.Where(y => y.TicketId == j.Id && y.Payment.Status == 1 && y.Qty > 0 && y.CreateDate >= sdate).ToList();
                    }
                    if (tic.Count() > 0)
                    {
                        //double total = i.EventTickets.Select(t => t.Quantity ?? 0).Sum();
                        double total = i.EventTickets.Where(y => y.Price > 0).Select(t => t.Quantity ?? 0).Sum();
                        foreach (var k in tic)
                        {
                            var adminfee = k.Payment.adminfee;// db.Payments.FirstOrDefault(x => x.Id == k.PaymemtId).adminfee;
                            if (adminfee == null)
                            {
                                adminfee = db.PaymentSetups.FirstOrDefault().Adminfee;
                            }
                            var totalearn = k.Qty * k.Amount;
                            var transcost = (totalearn * adminfee) / 100;
                            var mincharge = k.Payment.MinimumFee;

                            if (k.OfferCode > 0)
                            {
                                var discount = db.Offers.FirstOrDefault(x => x.Id == k.OfferCode);
                                if (discount.OfferType == 1)
                                {
                                    totalearn = Math.Round(Convert.ToDecimal(totalearn - (totalearn * discount.Value / 100)), 2);
                                }
                                else
                                {
                                    totalearn = totalearn - discount.Value;
                                }
                            }

                            if (mincharge != null)
                            {
                                if (transcost < mincharge)
                                {
                                    var comition = mincharge;
                                    var earned = totalearn - comition;
                                    ct.Add(new TransactionHistory { Date = Convert.ToDateTime(k.CreateDate).ToString("dd/MM/yyyy "), Total = total.ToString(), EventDate = Convert.ToDateTime(i.StartDate).ToString("dd/MM/yyyy hh:mmtt"), Eventname = i.Event_name, Booked = k.Qty.ToString(), GrossTotal = totalearn.ToString(), Transactioncosts = comition.ToString(), TotalEarned = earned.ToString(), TicketName = j.TicketName, PaymentCurrency = (int)j.PaymentCurrency });
                                }
                                else
                                {
                                    var comition = Math.Round(Convert.ToDecimal(totalearn * adminfee / 100), 2);
                                    var earned = totalearn - comition;
                                    ct.Add(new TransactionHistory { Date = Convert.ToDateTime(k.CreateDate).ToString("dd/MM/yyyy "), Total = total.ToString(), EventDate = Convert.ToDateTime(i.StartDate).ToString("dd/MM/yyyy hh:mmtt"), Eventname = i.Event_name, Booked = k.Qty.ToString(), GrossTotal = totalearn.ToString(), Transactioncosts = comition.ToString(), TotalEarned = earned.ToString(), TicketName = j.TicketName, PaymentCurrency = (int)j.PaymentCurrency });
                                }
                            }
                            else
                            {
                                var comition = Math.Round(Convert.ToDecimal(totalearn * adminfee / 100), 2);
                                var earned = totalearn - comition;
                                ct.Add(new TransactionHistory { Date = Convert.ToDateTime(k.CreateDate).ToString("dd/MM/yyyy "), Total = total.ToString(), EventDate = Convert.ToDateTime(i.StartDate).ToString("dd/MM/yyyy hh:mmtt"), Eventname = i.Event_name, Booked = k.Qty.ToString(), GrossTotal = totalearn.ToString(), Transactioncosts = comition.ToString(), TotalEarned = earned.ToString(), TicketName = j.TicketName, PaymentCurrency = (int)j.PaymentCurrency });
                            }

                            //var comition = Math.Round(Convert.ToDecimal(totalearn * adminfee / 100), 2);
                            //var earned = totalearn - comition;
                            //    ct.Add(new TransactionHistory { Date = Convert.ToDateTime(k.CreateDate).ToString("dd/MM/yyyy "), Total = total.ToString(), EventDate = Convert.ToDateTime(i.StartDate).ToString("dd/MM/yyyy hh:mmtt"), Eventname = i.Event_name, Booked = k.Qty.ToString(), GrossTotal = totalearn.ToString(), Transactioncosts = comition.ToString(), TotalEarned = earned.ToString(), TicketName = j.TicketName });
                        }
                    }
                }
            }

            //foreach (var i in cont)
            //{
            //    //  var mul = db.MultimediaContents.Where(s => s.Event_Id == i.Id).ToList().Take(1);                
            //    ct.Add(new EventTicket_
            //    {
            //        Id = i.Id,
            //        TicketName = i.TicketName,
            //        Seat = i.Seat,
            //        ColorArea = i.ColorArea,
            //        GateNo = i.GateNo.ToString(),
            //        Quantity = Convert.ToInt32(i.Quantity),
            //        table = i.tableNo,
            //        Price = Convert.ToDecimal(i.Price),
            //        TicketType = Convert.ToInt32(i.Ticket_Type)
            //    });
            //};
            return ct;
        }
        public static List<ExistingOffers> checkOffer(string coupan, int eventID = 0)
        {
            List<ExistingOffers> Resp = new List<ExistingOffers>();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var isoffer = dbConn.Offers.FirstOrDefault(x => x.CoupenCode == coupan && x.OfferPageCategory == 1);
                    if (isoffer != null)
                    {

                        Resp = (from r in dbConn.Offers
                                where r.CoupenCode == coupan && r.IsDeleted != true && r.EventId == eventID
                                select new ExistingOffers
                                {
                                    Id = r.Id,
                                    EventName = dbConn.Events.FirstOrDefault(m => m.Id == r.EventId).Event_name,
                                    Offertype = r.OfferType,
                                    coupencode = r.CoupenCode,
                                    startdate = r.StartDate,
                                    enddate = r.EndDate,
                                    discount = r.Value,
                                    TicketType = r.TicketType
                                }).ToList();
                    }
                    else
                    {
                        var email = "";
                        var user = dbConn.Coupons.Where(c => c.CoupanCode == coupan).FirstOrDefault().Mobile;
                        if (user == ManageSession.UserSession.EmailId)
                        {
                            email = ManageSession.UserSession.EmailId;
                        }
                        else if (user == ManageSession.UserSession.PhoneNo)
                        {
                            email = ManageSession.UserSession.PhoneNo;
                        }
                        //var userrr = user.FirstOrDefault(x => x.email == ManageSession.UserSession.PhoneNo || (email!= null && x.email.ToLower() == email.ToLower()));
                        if (email != null)
                        {
                            Resp = (from r in dbConn.Offers
                                    join c in dbConn.Coupons on r.Id equals c.OfferId
                                    where c.CoupanCode == coupan && r.IsDeleted != true && r.EventId == eventID && c.Mobile == email
                                    select new ExistingOffers
                                    {
                                        Id = r.Id,
                                        EventName = dbConn.Events.FirstOrDefault(m => m.Id == r.EventId).Event_name,
                                        Offertype = r.OfferType,
                                        coupencode = r.CoupenCode,
                                        startdate = r.StartDate,
                                        enddate = r.EndDate,
                                        discount = r.Value,
                                        TicketType = r.TicketType
                                    }).ToList();
                            //}
                        }
                    }
                    return Resp;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public List<AWSStreamPage> GetAWSChannelDetailForAdmin()
        {
            db = new EventmanagerEntities();
            List<AWSStreamPage> ct = new List<AWSStreamPage>();

            try
            {
                var List = db.AWSStreamingPages.Where(a => a.ChannelForWebSite == (int)Ticketforwebsite.Stream233).ToList();
                foreach (var item in List)
                {
                    ct.Add(new AWSStreamPage { Id = item.Id, SiteName = item.SiteName, SiteLogo = item.SiteLogo, ServerUrl = item.ServerUrl, StreamKey = item.StreamKey, ChannelId = item.ChannelId, IsStreaming = item.IsStreaming, StreamStartDate = item.StreamStartDate, StreamStopDate = item.StreamStopDate });
                }
            }
            catch (Exception ex)
            {

            }
            return ct;
        }
        public static int getEvent(int ticID)
        {
            int evid = 0;
            using (var db = new EventmanagerEntities())
            {

                try
                {
                    var ev = db.EventTickets.Where(x => x.Id == ticID).FirstOrDefault();
                    if (ev != null)
                    {
                        evid = Convert.ToInt32(ev.Event_Id);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return evid;

        }
        public static companyDetail getcompany(int ID)
        {
            companyDetail result = new companyDetail();
            using (var db = new EventmanagerEntities())
            {

                try
                {
                    var ev = db.Events.Where(x => x.Id == ID).FirstOrDefault();
                    if (ev != null)
                    {
                        var cop = db.Companies.FirstOrDefault(x => x.Id == ev.Company_Id);
                        result.Id = cop.Id; result.Logo = cop.Logo; result.Name = cop.Name_of_business; result.website = cop.website;
                    }
                }
                catch (Exception ex) { }
            }
            return result;

        }
    }
    public class Reason
    {
        public string code { get; set; }
        public string message { get; set; }
    }
    public class StatusResponse
    {
        public string amount { get; set; }
        public string status { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string type { get; set; }
        public string ExtId { get; set; }
        public string financialTransactionId { get; set; }
        public string Page { get; set; }
    }
    public class TransctionStatus
    {
        public string amount { get; set; }
        public string currency { get; set; }
        public int financialTransactionId { get; set; }
        public int externalId { get; set; }
        public Payer payer { get; set; }
        public string status { get; set; }
        public Reason reason { get; set; }
    }

    public class CallbackStatus
    {
        public string financialTransactionId { get; set; }
        public string externalId { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public Payer payer { get; set; }
        public string payeeNote { get; set; }
        public string status { get; set; }
    }

    public class Transction_Status
    {
        public string externalId { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public Payer payer { get; set; }
        public string payerMessage { get; set; }
        public string payeeNote { get; set; }
        public string status { get; set; }
        public string reason { get; set; }
        public string financialTransactionId { get; set; }
    }
    public class TransactionHistory
    {
        public string Date { get; set; }
        public string Eventname { get; set; }
        public string EventDate { get; set; }
        public string Booked { get; set; }
        public string Total { get; set; }
        public string GrossTotal { get; set; }
        public string Transactioncosts { get; set; }
        public string TotalEarned { get; set; }
        public string TicketName { get; set; }
        public int PaymentCurrency { get; set; }
    }
    public class Event_
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        [Required(ErrorMessage = "Event Name is required.")]
        public string EventName { get; set; }
        public int Eventtype { get; set; }
        public int? streamType { get; set; }
        public string EventKey { get; set; }
        public string LiveURL { get; set; }
        public string evid { get; set; }
        public string EventURLWebrtc { get; set; }
        public int EventCategory { get; set; }
        public string PublishDate { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        [Required(ErrorMessage = "Venue Name is required.")]
        public string Venue { get; set; }
        [Required(ErrorMessage = "City Name is required.")]
        public string CityName { get; set; }
        public string Description { get; set; }
        public int BusinessOwner_Id { get; set; }
        public int User_Id { get; set; }
        public string UserName { get; set; }
        public string status { get; set; }
        public int Total_Invitaion { get; set; }
        public bool IsTicket { get; set; }
        public Nullable<int> paymentStatus { get; set; }
        public DateTime purchaseDate { get; set; }
        public IList<address> address_ { get; set; }
        public IList<Multimedia_Content> Multimedia { get; set; }
        public IList<RSVPModel> Rsvp { get; set; }
        public IList<EventTicket_> Tickets { get; set; }
        public string TicketDate { get; set; }

    }
    public class createTicket
    {
        public int EventId { get; set; }
        public IList<EventTicket_> Ticket_ { get; set; }
    }
    public class EventTicket_
    {
        public int Id { get; set; }
        public int TicketType { get; set; }
        public int TmapID { get; set; }
        public string TicketTypeText { get; set; }
        public string TicketName { get; set; }
        public string Seat { get; set; }
        public string table { get; set; }
        public string ColorArea { get; set; }
        public string GateNo { get; set; }
        public decimal Price { get; set; }
        public int Fee { get; set; }
        public int Quantity { get; set; }
        public string Barcode { get; set; }
        public bool IsEnable { get; set; }
        public int validdays { get; set; }
        public int PaymentType { get; set; }
        public int PaymentCurrency { get; set; }
        public int TicketId { get; set; }
        public int TicketmapId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public DateTime ticketdate { get; set; }
    }
    public partial class Multimedia_Content
    {
        public string URL { get; set; }
        public int Event_Id { get; set; }
        public int Mul_Type { get; set; }
        public bool Mul_MainPic { get; set; }
        public int videotype { get; set; }
        public string videoId { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
    public partial class RSVPModel
    {
        public string Namer { get; set; }
        public string Phone { get; set; }
    }
    public class expressPendingModel
    {
        public int result { get; set; }
        public string result_text { get; set; }
        public string order_id { get; set; }
        public string token { get; set; }
        public string currency { get; set; }
        public string amount { get; set; }
        public string auth_code { get; set; }
        public string transaction_id { get; set; }
        public string date_processed { get; set; }

    }
    public class AWSStreamPage
    {
        public int Id { get; set; }
        public string SiteName { get; set; }
        public string SiteLogo { get; set; }
        public string ServerUrl { get; set; }
        public string StreamKey { get; set; }
        public Nullable<int> ChannelId { get; set; }
        public Nullable<bool> IsStreaming { get; set; }
        public Nullable<System.DateTime> StreamStartDate { get; set; }
        public Nullable<System.DateTime> StreamStopDate { get; set; }
    }
}