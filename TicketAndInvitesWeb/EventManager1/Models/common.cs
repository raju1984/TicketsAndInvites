using EventManager1.DBCon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Hosting;
using ZXing;
using System.Threading.Tasks;
using System.Net;

namespace EventManager1.Models
{
    public class common
    {
        EventmanagerEntities db;
        public List<Country> GetCountry()
        {
            db = new EventmanagerEntities();
            List<Country> ct = new List<Country>();
            var cont = db.Countries.ToList();
            foreach (var i in cont) { ct.Add(new Country { Id = i.Id, Name = i.Name }); };
            return ct;
        }
        public List<Dropdownlist> GetEventType()
        {

            List<Dropdownlist> ct = new List<Dropdownlist>();
            ct.Add(new Dropdownlist { Id = (int)EventTypeEnum.Normal, Text = "Physical Event" });
            ct.Add(new Dropdownlist { Id = (int)EventTypeEnum.E_event, Text = "Hosted Event" });
            ct.Add(new Dropdownlist { Id = (int)EventTypeEnum.Live, Text = "Live event" });
            return ct;
        }
        public static string GetEventStatus(int? EventStats)
        {
            if (EventStats == (int)EventStatus.Pending)
            {
                return "Pending";
            }
            else if (EventStats == (int)EventStatus.Active)
            {
                return "Active";
            }
            else if (EventStats == (int)EventStatus.Deleted)
            {
                return "Deleted";
            }
            else if (EventStats == (int)EventStatus.Closed)
            {
                return "Closed";
            }
            else
            {
                return "Incomplete";
            }
        }
        public List<TicketTypemodel> GetticketTypes()
        {
            db = new EventmanagerEntities();
            List<TicketTypemodel> ct = new List<TicketTypemodel>();
            var cont = db.TicketTypes.ToList();
            foreach (var i in cont) { ct.Add(new TicketTypemodel { Id = i.id, TicketTypes = i.TicketTypes }); };
            return ct;
        }
        public Event_ GetEvents(int id)
        {
            db = new EventmanagerEntities();
            var ev1 = db.Events.FirstOrDefault(i => i.Id == id);
            dynamic eve;
            if (ev1.Eventtype == 0)
            {
                eve = db.Events.Include("Address").Include("MultimediaContents").Include("RSVPs").FirstOrDefault(i => i.Id == id);
            }
            else { eve = db.Events.Include("MultimediaContents").Include("RSVPs").FirstOrDefault(i => i.Id == id); }
            Event_ ev = new Event_();
            ev.Id = eve.Id;
            ev.BusinessOwner_Id = Convert.ToInt32(eve.Company_Id);
            ev.User_Id = Convert.ToInt32(eve.User_Id);
            ev.EventName = eve.Event_name;
            ev.LiveURL = eve.LiveURL;
            ev.EventCategory = eve.eventCatg != null ? Convert.ToInt32(eve.eventCatg) : 1;
            ev.Eventtype = eve.Eventtype != null ? Convert.ToInt32(eve.Eventtype) : 0;
            ev.Venue = eve.Venue;
            string sdate = eve.StartDate.ToString();
            DateTime sdt = Convert.ToDateTime(sdate);
            string endate = eve.EndDate.ToString();
            DateTime endt = Convert.ToDateTime(endate);
            DateTime publisdt = Convert.ToDateTime(eve.PublishDate);

            ev.CityName = eve.Address.City;
            ev.PublishDate = publisdt.ToString("yyyy-MM-dd");
            ev.StartDate = sdt.ToString("yyyy-MM-dd");
            ev.StartTime = sdt.ToString("HH:mm");
            ev.EndDate = endt.ToString("yyyy-MM-dd");
            ev.EndTime = endt.ToString("HH:mm");
            ev.Description = eve.Description;
            // ev.address_ = eve.Address;
            List<RSVPModel> rsvp = new List<RSVPModel>();
            foreach (var i in eve.RSVPs)
            {
                rsvp.Add(new RSVPModel { Namer = i.Namer, Phone = i.Phone });
            }
            List<address> addr = new List<address>();
            addr.Add(new address { AddressLine = eve.Address.AddressLine, Country = Convert.ToInt32(eve.Address.Country_Id) });
            ev.address_ = addr;
            List<Multimedia_Content> mul = new List<Multimedia_Content>();
            foreach (var i in eve.MultimediaContents)
            {
                mul.Add(new Multimedia_Content { URL = i.URL, Mul_Type = Convert.ToInt32(i.Mul_Type), Mul_MainPic = Convert.ToBoolean(i.Mul_MainPic), videotype = i.videotype != null ? Convert.ToInt32(i.videotype) : 0 });
            }


            ev.Multimedia = mul;
            ev.Rsvp = rsvp;

            return ev;
        }
        public static string AlphanumbericNumber()
        {
            Random generator = new Random();
            return generator.Next(0, 999999).ToString("D6");
        }
        public static bool GenerateBarcode(string BarCodeNumber)
        {
            
            string BarcodePath = HostingEnvironment.MapPath("~/Content/BarCode/" + BarCodeNumber + ".jpg");
            Log4Net.Error("Checking BarCodeNumber :" + BarCodeNumber);
            if (!System.IO.File.Exists(BarcodePath))
            {
                Log4Net.Error(BarCodeNumber + "does not exist in the system.");
                Log4Net.Error("Generating Barcode number : "+ BarCodeNumber);
                try
                {
                    var barcodeWriter = new BarcodeWriter();
                    // set the barcode format
                    barcodeWriter.Format = BarcodeFormat.QR_CODE;

                    // write text and generate a 2-D barcode as a bitmap
                    barcodeWriter.Write(BarCodeNumber).Save(BarcodePath);
                    return true;
                }
                catch (Exception ex)
                {
                    Log4Net.Error("Generate Barcode excpetion: " + ex.Message + "\n " + ex.StackTrace);
                }
            }

            return false;


        }

        public static async Task<bool> SendTicketToMail(string name, string emailaddress, string TickeName, string request, string address, string rootpath, int tumapId, int ticketId, DateTime ticketdate)
        {
            //string body = string.Format("Hi {0},<br>I would love to invite you for our exiting event."
            //                      + "Please find the details below and let me know if you will be able to come, so that I will hold the seat for you.", name);
            try
            {
                Log4Net.Error("SendTicketToMail :Funcation Start");
                EventmanagerEntities db = new EventmanagerEntities();
                var ev = db.Events.Include("EventTickets").Include(a => a.RSVPs).Include("MultimediaContents").Where(c => c.EventTickets.Any(i => i.Id == ticketId)).FirstOrDefault();
                var ev_tickets = db.EventTickets.Where(e => e.Id == ticketId).FirstOrDefault();
                var addr = db.Addresses.FirstOrDefault(x => x.Id == ev.Address_Id);
                var ttickmap = db.TickeUserMaps.Where(x => x.Id == tumapId).FirstOrDefault();
                if (ttickmap != null && ttickmap.Id > 0)
                {
                    if (string.IsNullOrEmpty(ttickmap.BarCodeNumber))
                    {
                        var ticket = TickeName.Split('.');
                        ttickmap.BarCodeNumber = ticket[0];
                        db.SaveChanges();
                    }
                }

                var tickmap = db.TickeUserMaps.Where(x => x.Id == tumapId).FirstOrDefault();
                var barcodeNumber = tickmap.BarCodeNumber;
                TickeName = barcodeNumber + ".jpg";
                string BarcodePath = HostingEnvironment.MapPath("~/Content/BarCode/" + TickeName);
                if(!System.IO.File.Exists(BarcodePath))
                {
                    common.GenerateBarcode(barcodeNumber);
                }
                ModelInviation model = new ModelInviation();
                //model.downloadUrl = address +"/"+ "Promotion/Downloadticket?id=" + ticketId+"&user="+ManageSession.UserSession.Id;
                model.NameTosend = name;
                model.eventType = ev.Eventtype;
                model.EventName = ev.Event_name;
                model.EventDate = ticketdate;
                model.TicketName = ev_tickets.TicketName;
                var CreatedOnWebsite = tickmap.Ticketforwebsite == null ? ev.CreatedOnWebsite : tickmap.Ticketforwebsite;
                if (CreatedOnWebsite == null)
                {
                    CreatedOnWebsite = (int)Ticketforwebsite.veetickets;
                }
                model.Websitefor = CreatedOnWebsite;
                if (ev.Company != null)
                {
                    model.CompanyName = ev.Company != null ? ev.Company.Name_of_business : "Event Pass";
                }
                else if (ev.User != null)
                {
                    model.CompanyName = ev.User != null ? ev.User.FirstName : "Event Pass";
                }
                var no_of_Days = db.EventTickets.Where(x => x.Id == ticketId).FirstOrDefault().ValidDays;
                int? eventID = db.EventTickets.Where(x => x.Id == ticketId).FirstOrDefault().Event_Id;
                if (no_of_Days != null)
                {
                    model.ValidDays = no_of_Days;
                }
                else
                {
                    model.ValidDays = 1;
                }
                string url;
                var img = ev.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1 && x.Mul_MainPic == true);
                if (img != null)
                {
                    url = img.URL; //img.URL.Substring(5);
                }
                else
                {
                    img = ev.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1);//.Substring(5);  
                    if (img != null)
                    {
                        url = img.URL;
                    }
                    else
                    {
                        url = address + "/" + "images/placeholder-image.jpg";
                    }
                }
                model.Image = url;
                model.Address = addr.AddressLine + ", " + addr.City;
                model.Venuename = ev.Venue;
                model.RsvpName = ev.RSVPs != null && ev.RSVPs.Count() > 0 ? ev.RSVPs.FirstOrDefault().Namer : "";
                model.Contact = ev.RSVPs != null && ev.RSVPs.Count() > 0 ? ev.RSVPs.FirstOrDefault().Phone : "";
                model.Gate = ev.EventTickets.FirstOrDefault(x => x.Id == ticketId).GateNo;
                model.qty = db.TickeUserMaps.FirstOrDefault(x => x.Id == tumapId).Qty;
                model.EventId = ev.Id;
                //var request = HttpContext.Current.Request;
                //var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
                model.TickeUrl = address + "/Content/BarCode/" + TickeName;
                // model.TickeUrl = "https://vsprocessorpro.com/Content/BarCode/" + TickeName;

                Log4Net.Error("Ticket URL : " + model.TickeUrl);
                Guid id = Guid.NewGuid();
                //var siteval = ev.CreatedOnWebsite == null ? tickmap.Ticketforwebsite : ev.CreatedOnWebsite;
                if (CreatedOnWebsite != null)
                {
                    if (string.IsNullOrEmpty(emailaddress))
                    {
                        Log4Net.Error("SendEmail Funcation can not be called Email addres is empty");
                        return false;
                    }

                    //model.WebT = 1;
                    //model.WebT = (int)Ticketforwebsite.Ticketandinvite;
                    if (model.eventType == 2)
                    {



                        if (CreatedOnWebsite == (int)Ticketforwebsite.vsprocessorpro)
                        {
                            string evid = EnryptString(model.EventId.ToString());
                            string Email = EnryptString(emailaddress);
                            //string shortURL = MakeTinyUrl("https://vsprocessorpro.com/Account/Streaming?returnUrl=/OnlineStreaming/Index/?ev=" + evid);
                            string shortURL = MakeTinyUrl("https://vsprocessorpro.com/Account/ViewStreaming?key=" + Email + "&evid=" + evid);
                            model.StreamingURL = shortURL;
                        }
                        else
                        {
                            string evid = EnryptString(model.EventId.ToString());
                            string Email = EnryptString(emailaddress);
                            //string shortURL = MakeTinyUrl("https://www.ticketsandinvites.com/Account/Userlogin?returnUrl=/OnlineStreaming/Index/?ev=" + evid);
                            string shortURL = MakeTinyUrl("https://veetickets.com/Account/ViewStreaming?key=" + Email + "&evid=" + evid);
                            model.StreamingURL = shortURL;
                        }
                    }

                    string body = EmailSending.RunCompile(rootpath, "TicketTemplate.cshtml", id.ToString(), model);
                    Log4Net.Error("SendEmail Funcation call" + "Email addres :" + emailaddress + "Body :-" + body);
                    if (model.eventType == 0)
                    {
                        if (CreatedOnWebsite == (int)Ticketforwebsite.vsprocessorpro)
                        {
                            //return EmailSending.SendEmail(emailaddress, body, "Ticketsandinvites", "Tickets", 1, Convert.ToInt32(eventID));
                            return await EmailSending.SendCMEmailTicket(emailaddress, body, "VSPASS", "Tickets", (int)Ticketforwebsite.vsprocessorpro, Convert.ToInt32(eventID));
                        }
                        else if (CreatedOnWebsite == (int)Ticketforwebsite.veetickets)
                        {
                            //return EmailSending.SendEmail(emailaddress, body, "Ticketsandinvites", "Tickets", 1, Convert.ToInt32(eventID));
                            return await EmailSending.SendCMEmailTicket(emailaddress, body, "VEETKTS", "Tickets", (int)Ticketforwebsite.veetickets, Convert.ToInt32(eventID));
                        }
                        else
                        {
                            //return EmailSending.SendEmail(emailaddress, body, "Ticketsandinvites", "Tickets", 1, Convert.ToInt32(eventID));
                            return await EmailSending.SendCMEmailTicket(emailaddress, body, "VSPASS", "Tickets", (int)Ticketforwebsite.vsprocessorpro, Convert.ToInt32(eventID));
                        }

                    }
                    else if (model.eventType == 1)
                    {
                        if (CreatedOnWebsite == (int)Ticketforwebsite.vsprocessorpro)
                        {
                            //return EmailSending.SendEmail(emailaddress, body, "LuxCinemagh", "Videos Pass", 13);
                            return await EmailSending.SendCMEmailTicket(emailaddress, body, "VSPASS", "Videos Pass", (int)Ticketforwebsite.vsprocessorpro);
                        }
                        else
                        {
                            //return EmailSending.SendEmail(emailaddress, body, "Ticketsandinvites", "Videos Pass", 1);
                            return await EmailSending.SendCMEmailTicket(emailaddress, body, "VEETKTS", "Videos Pass", (int)Ticketforwebsite.veetickets);
                        }

                    }
                    else if (model.eventType == 2)
                    {
                        if (CreatedOnWebsite == (int)Ticketforwebsite.vsprocessorpro)
                        {

                            return await EmailSending.SendCMEmailTicket(emailaddress, body, "VSPASS", "Live Stream Pass", (int)Ticketforwebsite.vsprocessorpro);
                        }
                        else
                        {

                            return await EmailSending.SendCMEmailTicket(emailaddress, body, "VEETKTS", "Live Stream Pass", (int)Ticketforwebsite.veetickets);
                        }


                    }
                    else { return true; }

                }
                else
                {
                    if (string.IsNullOrEmpty(emailaddress))
                    {
                        Log4Net.Error("SendEmail Funcation can not be called Email addres is empty");
                        return false;
                    }
                    //model.WebT = 1;
                    if (model.eventType == 2) //To do pending task
                    {
                        string Email = EnryptString(emailaddress);
                        string evid = EnryptString(model.EventId.ToString());
                        // string shortURL = MakeTinyUrl("https://www.ticketsandinvites.com/Account/Userlogin?returnUrl=/OnlineStreaming/Index/?ev=" + evid);
                        string shortURL = MakeTinyUrl("https://veetickets.com/Account/ViewStreaming?key=" + Email + "&evid=" + evid);
                        model.StreamingURL = shortURL;

                    }
                    model.WebT = (int)Ticketforwebsite.veetickets;
                    string body = EmailSending.RunCompile(rootpath, "TicketTemplate.cshtml", id.ToString(), model);
                    Log4Net.Error("SendEmail Funcation call" + "Email addres :" + emailaddress + "Body :-" + body);
                    if (model.eventType == 0)
                    {
                        return await EmailSending.SendCMEmailTicket(emailaddress, body, "VEETKTS", "Tickets", (int)Ticketforwebsite.veetickets, Convert.ToInt32(eventID));
                    }
                    else if (model.eventType == 1)
                    {
                        return await EmailSending.SendCMEmailTicket(emailaddress, body, "VEETKTS", "Videos Pass", (int)Ticketforwebsite.veetickets);
                    }
                    else if (model.eventType == 2)
                    {
                        return await EmailSending.SendCMEmailTicket(emailaddress, body, "VEETKTS", "Live Stream Pass", (int)Ticketforwebsite.veetickets);
                    }
                    else { return true; }
                }


            }
            catch (Exception ex)
            {
                Log4Net.Error("SendTicketToMail Exception:" + ex.StackTrace);
                return false;
            }

        }

        public static string GetUniqueBarCode(int number=0)
        {
            
            Random generator = new Random();
            var Alphanumber = generator.Next(0, 999999).ToString("D6")+ number.ToString();

            bool IsBarCodeUnique = false;
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                while(IsBarCodeUnique==false)
                {
                    var existing = dbConn.TickeUserMaps.Where(x => x.BarCodeNumber == Alphanumber).ToList();
                    if (existing != null && existing.Count() > 0)
                    {
                        Alphanumber = generator.Next(0, 999999).ToString("D6") + number.ToString();
                    }
                    else
                    {
                        IsBarCodeUnique = true; break;
                    }
                }
                
            }
            return Alphanumber;
        }

        //public static async Task<bool> SendTicketToMail(string name, string emailaddress, string TickeName, string request, string address, string rootpath, int tumapId, int ticketId)
        //{
        //    //string body = string.Format("Hi {0},<br>I would love to invite you for our exiting event."
        //    //                      + "Please find the details below and let me know if you will be able to come, so that I will hold the seat for you.", name);
        //    try
        //    {
        //        Log4Net.Error("SendTicketToMail :Funcation Start");
        //        EventmanagerEntities db = new EventmanagerEntities();
        //        var ev = db.Events.Include("EventTickets").Include(a => a.RSVPs).Include("MultimediaContents").Where(c => c.EventTickets.Any(i => i.Id == ticketId)).FirstOrDefault();
        //        var addr = db.Addresses.FirstOrDefault(x => x.Id == ev.Address_Id);
        //        ModelInviation model = new ModelInviation();
        //        //model.downloadUrl = address +"/"+ "Promotion/Downloadticket?id=" + ticketId+"&user="+ManageSession.UserSession.Id;
        //        model.NameTosend = name;
        //        model.eventType = ev.Eventtype;
        //        model.EventName = ev.Event_name;
        //        model.EventDate = ev.StartDate;
        //        string url;
        //        var img = ev.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1 && x.Mul_MainPic == true);
        //        if (img != null)
        //        {
        //            url = img.URL.Substring(5);
        //        }
        //        else
        //        {
        //            img = ev.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1);//.Substring(5);  
        //            if(img != null)
        //            {
        //                url = img.URL.Substring(5);
        //            }
        //            else
        //            {
        //                url = "images/placeholder-image.jpg";
        //            }
        //        }
        //        model.Image = address + "/" + url;
        //        model.Address = addr.AddressLine + ", " + addr.City;
        //        model.Venuename = ev.Venue;
        //        model.RsvpName = ev.RSVPs != null && ev.RSVPs.Count() > 0 ? ev.RSVPs.FirstOrDefault().Namer : "";
        //        model.Contact = ev.RSVPs != null && ev.RSVPs.Count() > 0 ? ev.RSVPs.FirstOrDefault().Phone : "";
        //        model.Gate = ev.EventTickets.FirstOrDefault(x => x.Id == ticketId).GateNo;
        //        model.qty = db.TickeUserMaps.FirstOrDefault(x => x.Id == tumapId).Qty;
        //        //var request = HttpContext.Current.Request;
        //        //var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
        //        model.TickeUrl = address + "/Content/BarCode/" + TickeName;
        //        Guid id = Guid.NewGuid();

        //        string body = EmailSending.RunCompile(rootpath, "TicketTemplate.cshtml", id.ToString(), model);
        //        Log4Net.Error("SendEmail Funcation call" +"Email addres :"+emailaddress +"Body :-"+body);
        //        if(model.eventType==0)
        //        { 
        //        return await EmailSending.SendEmail(emailaddress, body, "Ticketsandinvites", "Tickets",1);
        //        }
        //        else { return true; }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4Net.Error("SendTicketToMail Exception:" + ex.StackTrace);
        //        return false;
        //    }

        //}

        public static string MakeTinyUrl(string url)
        {
            try
            {
                if (url.Length <= 30)
                {
                    return url;
                }
                if (!url.ToLower().StartsWith("http") && !url.ToLower().StartsWith("ftp"))
                {
                    url = "http://" + url;
                }
                var request = WebRequest.Create("http://tinyurl.com/api-create.php?url=" + url);
                var res = request.GetResponse();
                string text;
                using (var reader = new StreamReader(res.GetResponseStream()))
                {
                    text = reader.ReadToEnd();
                }
                return text;
            }
            catch (Exception)
            {
                return url;
            }
        }

        public static string EnryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }
    }

    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class TicketTypemodel
    {
        public int Id { get; set; }
        public string TicketTypes { get; set; }
    }
    public class address
    {
        public int Id { get; set; }
        public int AddressType { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public int Country { get; set; }
        public string ZipCode { get; set; }
        public string FullName { get; set; }
        public string Landmark { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }

    }

}