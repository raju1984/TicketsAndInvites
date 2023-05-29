using EventManager1.DBCon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Hosting;
using System.IO;
using EventManager1.Areas.Organizer.Models;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventManager1.Models
{
    public class ModelInviation
    {
        public int EventTickeId { get; set; }
        public int EventId { get; set; }
        public string message { get; set; }
        public string Image { get; set; }
        public string EventName { get; set; }
        public string NameTosend { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Venuename { get; set; }
        public DateTime? EventDate { get; set; }
        public string RsvpName { get; set; }
        public string Contact { get; set; }
        public string BaseUrl { get; set; }
        public string TickeUrl { get; set; }
        public string StreamingURL { get; set; }
        public string downloadUrl { get; set; }
        public string Gate { get; set; }
        public int? qty { get; set; }
        public int? ValidDays { get; set; }
        public int? eventType { get; set; }
        public int? WebT { get; set; }
        public int? Websitefor { get; set; }
        public string TicketName { get; set; }

        public static async Task<bool> SendInvitation(int EventId, int EventTicketId, string EmailAddress, string Nametosend)
        {

            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                try
                {
                    var events = dbConn.Events.Include(a => a.User).Include(a => a.MultimediaContents).Include(a => a.Company).Include(a => a.RSVPs).Where(a => a.Id == EventId).FirstOrDefault();

                    if (events != null)
                    {
                        ModelInviation Model = new ModelInviation();
                        var request = HttpContext.Current.Request;
                        var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
                        //if (events.CreatedOnWebsite == 2) { address = "https://www.rovermanproductions.com"; }
                        if (events.CreatedOnWebsite == (int)Ticketforwebsite.Roverman)
                        {
                            address = "https://www.rovermanproductions.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.Stream233)
                        {
                            address = "https://www.stream233.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.Nationaltheatre)
                        {
                            address = "https://www.nationaltheatre.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.WorlaFest)
                        {
                            address = "https://www.worlafest.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.EibStream)
                        {
                            address = "http://stream.eibstream.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.Thepolobeachclub)
                        {
                            address = "https://www.thepolobeachclub.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.rapperholic)
                        {
                            address = "https://www.rapperholic.live";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.Charterhouse)
                        {
                            address = "https://www.charterhouse.live";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.Scribeproduction)
                        {
                            address = "https://www.scribeproductions.live";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.Bolaray)
                        {
                            address = "https://www.bolaray.live";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.parknwatch)
                        {
                            address = "https://www.parknwatch.biz";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.LuxCinemagh)
                        {
                            address = "http://www.luxcinemagh.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.vsprocessorpro)
                        {
                            address = "https://vsprocessorpro.com/";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.veetickets)
                        {
                            address = "http://www.veetickets.com/";
                        }
                        else
                        {
                            address = "http://www.veetickets.com/";
                        }
                        var addr = dbConn.Addresses.FirstOrDefault(x => x.Id == events.Address_Id);
                        Model.EventTickeId = EventTicketId;
                        Model.EventName = events.Event_name;
                        Model.NameTosend = Nametosend;
                        if (events.Company != null)
                        {
                            Model.CompanyName = events.Company != null ? events.Company.Name_of_business : "Event Pass"; //dbConn.Users.FirstOrDefault(x => x.Id == events.Company_Id).FirstName;
                        }// ManageSession.UserSession.FirstName; }
                        else if (events.User != null)
                        {
                            Model.CompanyName = events.User != null ? events.User.FirstName : "Event Pass";
                        }
                        string url;
                        var img = events.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1 && x.Mul_MainPic == true);
                        if (img != null)
                        {
                            //url = img.URL.Substring(6);
                            url = img.URL;
                        }
                        else
                        {
                            //url = (events.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1).URL).Substring(6);
                            img = events.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1);
                            if (img != null)
                            {
                                //url = img.URL.Substring(6);
                                url = img.URL;
                            }
                            else
                            {
                                url = "images/placeholder-image.jpg";
                            }
                        }
                        //Model.Image = address + "/" + url;
                        Model.Image = url;
                        //Model.Image = "http://scannpass.adequateshop.com/" + (events.MultimediaContents.Where(a => a.Mul_Type == (int)MultiMediaType.image).FirstOrDefault().URL).Substring(6);
                        Model.EventDate = events.StartDate;
                        Model.Venuename = events.Venue;
                        Model.Address = addr.AddressLine + ", " + addr.City;
                        Model.BaseUrl = address;
                        if (events.Company == null)
                        {
                            Model.RsvpName = events.User != null ? events.User.FirstName : "Event Pass"; //dbConn.Users.FirstOrDefault(x => x.Id == events.User_Id).FirstName;
                            Model.Contact = events.User != null ? events.User.PhoneNo : "Event Pass"; //dbConn.Users.FirstOrDefault(x => x.Id == events.User_Id).PhoneNo;
                        }
                        else
                        {
                            Model.RsvpName = events.RSVPs != null && events.RSVPs.Count() > 0 ? events.RSVPs.FirstOrDefault().Namer : "";
                            Model.Contact = events.RSVPs != null && events.RSVPs.Count() > 0 ? events.RSVPs.FirstOrDefault().Phone : "";
                        }
                        Guid id = Guid.NewGuid();

                        string body = EmailSending.RunCompile(HttpContext.Current.Server.MapPath("/Views/EmailTemplate/"), "Invitation.cshtml", id.ToString(), Model);
                        if (events.CreatedOnWebsite == (int)Ticketforwebsite.vsprocessorpro)
                        {
                            return await EmailSending.SendCMEmail(EmailAddress, body, "VSPASS", "Invitation", (int)Ticketforwebsite.LuxCinemagh);
                        }
                        else
                        {
                            return await EmailSending.SendCMEmail(EmailAddress, body, "Veetickets", "Invitation", (int)Ticketforwebsite.Ticketandinvite);
                        }
                        //return EmailSending.SendEmail(EmailAddress, body, "Ticketsandinvites", "Invitation",1);
                        //return EmailSending.SendCMEmail(EmailAddress, body, "Ticketsandinvites", "Invitation", (int)Ticketforwebsite.Ticketandinvite);
                    }
                }
                catch (Exception ex)
                {
                    Log4Net.Error("SendInvitation Exception :-" + ex.Message + System.Environment.NewLine + ex.StackTrace);
                    return false;
                }
                return false;
            }
        }
        //public static bool SendInvitation(int EventId, int EventTicketId, string EmailAddress, string Nametosend)
        //{

        //    using (EventmanagerEntities dbConn = new EventmanagerEntities())
        //    {
        //        try
        //        {
        //            var events = dbConn.Events.Include(a => a.User).Include(a => a.MultimediaContents).Include(a => a.Company).Include(a => a.RSVPs).Where(a => a.Id == EventId).FirstOrDefault();

        //            if (events != null)
        //            {
        //                ModelInviation Model = new ModelInviation();
        //                var request = HttpContext.Current.Request;
        //                var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
        //                var addr = dbConn.Addresses.FirstOrDefault(x => x.Id == events.Address_Id);
        //                Model.EventTickeId = EventTicketId;
        //                Model.EventName = events.Event_name;
        //                Model.NameTosend = Nametosend;
        //                if (events.Company == null)
        //                {
        //                    Model.CompanyName = events.Company != null ? events.Company.Name_of_business : "Event Pass"; //dbConn.Users.FirstOrDefault(x => x.Id == events.Company_Id).FirstName;
        //                }// ManageSession.UserSession.FirstName; }
        //                else if (events.User != null)
        //                {
        //                    Model.CompanyName = events.User != null ? events.User.FirstName : "Event Pass";
        //                }
        //                string url;
        //                var img = events.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1 && x.Mul_MainPic == true);
        //                if (img != null)
        //                {
        //                    url = img.URL.Substring(6);
        //                }
        //                else
        //                {
        //                    //url = (events.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1).URL).Substring(6);
        //                    img = events.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1);
        //                    if (img != null)
        //                    {
        //                        url = img.URL.Substring(6);
        //                    }
        //                    else
        //                    {
        //                        url = "images/placeholder-image.jpg";
        //                    }
        //                }
        //                Model.Image = address + "/" + url;
        //                //Model.Image = "http://scannpass.adequateshop.com/" + (events.MultimediaContents.Where(a => a.Mul_Type == (int)MultiMediaType.image).FirstOrDefault().URL).Substring(6);
        //                Model.EventDate = events.StartDate;
        //                Model.Venuename = events.Venue;
        //                Model.Address = addr.AddressLine + ", " + addr.City;
        //                Model.BaseUrl = address;
        //                if (events.Company == null)
        //                {
        //                    Model.RsvpName = events.User != null ? events.User.FirstName : "Event Pass"; //dbConn.Users.FirstOrDefault(x => x.Id == events.User_Id).FirstName;
        //                    Model.Contact = events.User != null ? events.User.PhoneNo : "Event Pass"; //dbConn.Users.FirstOrDefault(x => x.Id == events.User_Id).PhoneNo;
        //                }
        //                else
        //                {
        //                    Model.RsvpName = events.RSVPs != null && events.RSVPs.Count() > 0 ? events.RSVPs.FirstOrDefault().Namer : "";
        //                    Model.Contact = events.RSVPs != null && events.RSVPs.Count() > 0 ? events.RSVPs.FirstOrDefault().Phone : "";
        //                }
        //                Guid id = Guid.NewGuid();

        //                string body = EmailSending.RunCompile(HttpContext.Current.Server.MapPath("/Views/EmailTemplate/"), "Invitation.cshtml", id.ToString(), Model);
        //                return EmailSending.SendEmail(EmailAddress, body, "Ticketsandinvites", "Invitation");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log4Net.Error("SendInvitation Exception :-" + ex.Message + System.Environment.NewLine + ex.StackTrace);
        //            return false;
        //        }
        //        return false;
        //    }
        //}
        public static bool UpdateInvitation(int EventTickeId, int Status)
        {
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    int? invitationId = dbConn.EventTickets.Where(a => a.Id == EventTickeId).FirstOrDefault().Inviation_Id;
                    var eventticke = dbConn.Invitations.Where(a => a.Id == invitationId).FirstOrDefault();
                    if (eventticke != null && eventticke.Status == null)
                    {
                        eventticke.Status = Status;
                        dbConn.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex) { return false; }
        }
        public static async Task<bool> SendTicket(int EventTickeId, int Status, string request, string address, string rootpath)
        {
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var requests = HttpContext.Current.Request;
                    var adr = string.Format("{0}://{1}", requests.Url.Scheme, requests.Url.Authority);
                    int invitation_type = 1;
                    address = adr + "/";
                    dbConn.Configuration.LazyLoadingEnabled = false;
                    var eventticke = dbConn.EventTickets
                                           .Include(a => a.Invitation)
                                           .Include(a => a.Invitation.User)
                                           .Include(a => a.Invitation.Company)
                                           .Include(a => a.TickeUserMaps).Where(a => a.Id == EventTickeId).FirstOrDefault();
                    if (eventticke != null)
                    {
                        invitation_type = eventticke.Invitation.SendType ?? 1;
                        if (eventticke.TickeUserMaps != null && eventticke.TickeUserMaps.Count() > 0)
                        {
                            eventticke.TickeUserMaps.FirstOrDefault().IsTicketSendToUser = true;
                            eventticke.TickeUserMaps.FirstOrDefault().Qty = 1;
                            dbConn.SaveChanges();
                            //ticked already created for the user and Onley send mail
                            string BarcodePath = HostingEnvironment.MapPath("~/Content/BarCode/" + eventticke.TickeUserMaps.FirstOrDefault().BarCodeNumber + ".jpg");
                            if (File.Exists(BarcodePath))
                            {
                                if (invitation_type == 1)
                                {
                                    return await common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, eventticke.TickeUserMaps.FirstOrDefault().BarCodeNumber + ".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id, (DateTime)eventticke.StartDate);
                                }
                                else if (invitation_type == 2)
                                {
                                    var name = eventticke.Invitation.UserId != null ? eventticke.Invitation.User.FirstName : eventticke.Invitation.Company.UserName;
                                    var request1 = System.Web.HttpContext.Current.Request;
                                    var address1 = string.Format("{0}://{1}", request1.Url.Scheme, request1.Url.Authority);

                                    var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(eventticke.TickeUserMaps.FirstOrDefault().Id)));
                                    var Link = HttpUtility.UrlDecode(address + "/Ticket?id=" + IdConvert);

                                    CommonSMSModal commonSMSModal = new CommonSMSModal()
                                    {
                                        PhoneReciever = eventticke.Invitation.MobileNumber, //"+918882262496"
                                        Text = name + "  has Sent you an invite using ticketsandinvites " + "" + ":- " + Link,
                                        PhoneSender = ""// eventticke.Invitation.UserId != null ? eventticke.Invitation.User.PhoneNo : eventticke.Invitation.Company.Business_contact_number
                                    };
                                    Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                    threadSMS.Start();
                                    return true;

                                }
                                else if (invitation_type == 3)
                                {
                                    var name = eventticke.Invitation.UserId != null ? eventticke.Invitation.User.FirstName : eventticke.Invitation.Company.UserName;
                                    var request1 = System.Web.HttpContext.Current.Request;
                                    var address1 = string.Format("{0}://{1}", request1.Url.Scheme, request1.Url.Authority);

                                    var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(eventticke.TickeUserMaps.FirstOrDefault().Id)));
                                    var Link = HttpUtility.UrlDecode(address + "/Ticket?id=" + IdConvert);

                                    CommonSMSModal commonSMSModal = new CommonSMSModal()
                                    {
                                        PhoneReciever = eventticke.Invitation.MobileNumber, //"+918882262496"
                                        Text = name + "  has Sent you an invite using ticketsandinvites " + "" + ":- " + Link,
                                        PhoneSender = ""// eventticke.Invitation.UserId != null ? eventticke.Invitation.User.PhoneNo : eventticke.Invitation.Company.Business_contact_number
                                    };
                                    Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                    threadSMS.Start();
                                    return await common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, eventticke.TickeUserMaps.FirstOrDefault().BarCodeNumber + ".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id, (DateTime)eventticke.StartDate);

                                }
                            }
                            else
                            {
                                common.GenerateBarcode(eventticke.TickeUserMaps.FirstOrDefault().BarCodeNumber);
                                if (File.Exists(BarcodePath))
                                {
                                    if (invitation_type == 1)
                                    {
                                        return await common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, eventticke.TickeUserMaps.FirstOrDefault().BarCodeNumber + ".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id, (DateTime)eventticke.StartDate);
                                    }
                                    else if (invitation_type == 2)
                                    {
                                        var name = eventticke.Invitation.UserId != null ? eventticke.Invitation.User.FirstName : eventticke.Invitation.Company.UserName;
                                        var request1 = System.Web.HttpContext.Current.Request;
                                        var address1 = string.Format("{0}://{1}", request1.Url.Scheme, request1.Url.Authority);

                                        var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(eventticke.TickeUserMaps.FirstOrDefault().Id)));
                                        var Link = HttpUtility.UrlDecode(address + "/Ticket?id=" + IdConvert);

                                        CommonSMSModal commonSMSModal = new CommonSMSModal()
                                        {
                                            PhoneReciever = eventticke.Invitation.MobileNumber, //"+918882262496"
                                            Text = name + "  has Sent you an invite using ticketsandinvites. " + "" + ":- " + Link,
                                            PhoneSender = ""// eventticke.Invitation.UserId != null ? eventticke.Invitation.User.PhoneNo : eventticke.Invitation.Company.Business_contact_number
                                        };
                                        Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                        threadSMS.Start();
                                        return true;

                                    }
                                    else if (invitation_type == 3)
                                    {
                                        var name = eventticke.Invitation.UserId != null ? eventticke.Invitation.User.FirstName : eventticke.Invitation.Company.UserName;
                                        var request1 = System.Web.HttpContext.Current.Request;
                                        var address1 = string.Format("{0}://{1}", request1.Url.Scheme, request1.Url.Authority);

                                        var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(eventticke.TickeUserMaps.FirstOrDefault().Id)));
                                        var Link = HttpUtility.UrlDecode(address + "/Ticket?id=" + IdConvert);

                                        CommonSMSModal commonSMSModal = new CommonSMSModal()
                                        {
                                            PhoneReciever = eventticke.Invitation.MobileNumber, //"+918882262496"
                                            Text = name + "  has Sent you an invite using ticketsandinvites. " + "" + ":- " + Link,
                                            PhoneSender = ""// eventticke.Invitation.UserId != null ? eventticke.Invitation.User.PhoneNo : eventticke.Invitation.Company.Business_contact_number
                                        };
                                        Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                        threadSMS.Start();
                                        return await common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, eventticke.TickeUserMaps.FirstOrDefault().BarCodeNumber + ".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id, (DateTime)eventticke.StartDate);

                                    }
                                    //  return common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, eventticke.TickeUserMaps.FirstOrDefault().BarCodeNumber+".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id);
                                }
                            }
                        }
                        else
                        {
                            //if ticket not generated then create ticket
                            TickeUserMap map = new TickeUserMap();
                            map.EventTicket = eventticke;
                            map.InvitationId = eventticke.Inviation_Id;
                            map.CreateDate = DateTime.UtcNow;
                            map.Name = eventticke.Invitation.FirstName;
                            map.Email = eventticke.Invitation.EmailAddress;
                            map.Phone = eventticke.Invitation.MobileNumber;
                            map.Status = Status;
                            map.Qty = 1;
                            map.IsTicketSendToUser = true;
                            map.BarCodeNumber = common.GetUniqueBarCode(eventticke.Id);
                            dbConn.TickeUserMaps.Add(map);

                            if (dbConn.SaveChanges() > 0)
                            {
                                if (common.GenerateBarcode(map.BarCodeNumber) && (Status == 1 || Status == 2))
                                {
                                    string BarcodePath = HostingEnvironment.MapPath("~/Content/BarCode/" + map.BarCodeNumber + ".jpg");
                                    if (File.Exists(BarcodePath))
                                    {
                                        //send email
                                        if (invitation_type == 1)
                                        {
                                            map.IsTicketSendToUser = true;
                                            dbConn.SaveChanges();
                                            return await common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, eventticke.TickeUserMaps.FirstOrDefault().BarCodeNumber + ".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id, (DateTime)eventticke.StartDate);
                                        }
                                        else if (invitation_type == 2)
                                        {
                                            var name = eventticke.Invitation.UserId != null ? eventticke.Invitation.User.FirstName : eventticke.Invitation.Company.UserName;
                                            var request1 = System.Web.HttpContext.Current.Request;
                                            var address1 = string.Format("{0}://{1}", request1.Url.Scheme, request1.Url.Authority);

                                            var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(map.Id)));
                                            var Link = HttpUtility.UrlDecode(address + "/Ticket?id=" + IdConvert);

                                            CommonSMSModal commonSMSModal = new CommonSMSModal()
                                            {
                                                PhoneReciever = eventticke.Invitation.MobileNumber, //"+918882262496"
                                                Text = name + "  has Sent you an invite using ticketsandinvites. " + "" + ":- " + Link,
                                                PhoneSender = ""// eventticke.Invitation.UserId != null ? eventticke.Invitation.User.PhoneNo : eventticke.Invitation.Company.Business_contact_number
                                            };
                                            Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                            threadSMS.Start();
                                            map.IsTicketSendToUser = true;
                                            dbConn.SaveChanges();
                                            return true;

                                        }
                                        else if (invitation_type == 3)
                                        {
                                            var name = eventticke.Invitation.UserId != null ? eventticke.Invitation.User.FirstName : eventticke.Invitation.Company.UserName;
                                            var request1 = System.Web.HttpContext.Current.Request;
                                            var address1 = string.Format("{0}://{1}", request1.Url.Scheme, request1.Url.Authority);

                                            var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(map.Id)));
                                            var Link = HttpUtility.UrlDecode(address + "/Ticket?id=" + IdConvert);

                                            CommonSMSModal commonSMSModal = new CommonSMSModal()
                                            {
                                                PhoneReciever = eventticke.Invitation.MobileNumber, //"+918882262496"
                                                Text = name + "  has Sent you an invite using ticketsandinvites. " + "" + ":- " + Link,
                                                PhoneSender = ""// eventticke.Invitation.UserId != null ? eventticke.Invitation.User.PhoneNo : eventticke.Invitation.Company.Business_contact_number
                                            };
                                            Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                            threadSMS.Start();
                                            map.IsTicketSendToUser = true;
                                            dbConn.SaveChanges();
                                            return await common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, eventticke.TickeUserMaps.FirstOrDefault().BarCodeNumber + ".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id, (DateTime)eventticke.StartDate);

                                        }

                                        //  return common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, map.BarCodeNumber+".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id);
                                    }
                                    else
                                    {
                                        //try one time more
                                        common.GenerateBarcode(map.BarCodeNumber);
                                        if (File.Exists(BarcodePath))
                                        {
                                            if (invitation_type == 1)
                                            {
                                                map.IsTicketSendToUser = true;
                                                dbConn.SaveChanges();
                                                return await common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, map.BarCodeNumber + ".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id, (DateTime)eventticke.StartDate);
                                                // return common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, eventticke.TickeUserMaps.FirstOrDefault().BarCodeNumber + ".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id);
                                            }
                                            else if (invitation_type == 2)
                                            {
                                                var name = eventticke.Invitation.UserId != null ? eventticke.Invitation.User.FirstName : eventticke.Invitation.Company.UserName;
                                                var request1 = System.Web.HttpContext.Current.Request;
                                                var address1 = string.Format("{0}://{1}", request1.Url.Scheme, request1.Url.Authority);

                                                var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(map.Id)));
                                                var Link = HttpUtility.UrlDecode(address + "/Ticket?id=" + IdConvert);

                                                CommonSMSModal commonSMSModal = new CommonSMSModal()
                                                {
                                                    PhoneReciever = eventticke.Invitation.MobileNumber, //"+918882262496"
                                                    Text = name + "  has Sent you an invite using ticketsandinvites. " + "" + ":- " + Link,
                                                    PhoneSender = ""// eventticke.Invitation.UserId != null ? eventticke.Invitation.User.PhoneNo : eventticke.Invitation.Company.Business_contact_number
                                                };
                                                Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                                threadSMS.Start();
                                                map.IsTicketSendToUser = true;
                                                dbConn.SaveChanges();
                                                return true;

                                            }
                                            else if (invitation_type == 3)
                                            {
                                                var name = eventticke.Invitation.UserId != null ? eventticke.Invitation.User.FirstName : eventticke.Invitation.Company.UserName;
                                                var request1 = System.Web.HttpContext.Current.Request;
                                                var address1 = string.Format("{0}://{1}", request1.Url.Scheme, request1.Url.Authority);

                                                var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(map.Id)));
                                                var Link = HttpUtility.UrlDecode(address + "/Ticket?id=" + IdConvert);

                                                CommonSMSModal commonSMSModal = new CommonSMSModal()
                                                {
                                                    PhoneReciever = eventticke.Invitation.MobileNumber, //"+918882262496"
                                                    Text = name + "  has Sent you an invite using ticketsandinvites. " + "" + ":- " + Link,
                                                    PhoneSender = ""// eventticke.Invitation.UserId != null ? eventticke.Invitation.User.PhoneNo : eventticke.Invitation.Company.Business_contact_number
                                                };
                                                Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                                threadSMS.Start();
                                                // return common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, eventticke.TickeUserMaps.FirstOrDefault().BarCodeNumber + ".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id);
                                                map.IsTicketSendToUser = true;
                                                dbConn.SaveChanges();
                                                return await common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, map.BarCodeNumber + ".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id, (DateTime)eventticke.StartDate);
                                            }
                                            //send email
                                            // return common.SendTicketToMail(eventticke.Invitation.FirstName, eventticke.Invitation.EmailAddress, map.BarCodeNumber+".jpg", request, address, rootpath, eventticke.TickeUserMaps.FirstOrDefault().Id, eventticke.Id);
                                        }
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
            return false;
        }
        public static int savebroadcastmessage(int EventId, BroadcastEmails broadcost)
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                try
                {
                    BroadcastMessage brodmessage = new BroadcastMessage();
                    brodmessage.Email = broadcost.Youremail;
                    brodmessage.Message = broadcost.Message;
                    brodmessage.Subject = broadcost.Subject;
                    brodmessage.Event_Id = EventId;
                    dbConn.BroadcastMessages.Add(brodmessage);
                    if (dbConn.SaveChanges() > 0)
                    {
                        return brodmessage.Id;
                    }

                }
                catch { }
            }
            return 0;
        }
        public static bool savebroadcast(int EventId, string email, string mobile, int payId, BroadcastEmails broadcost, int BMID)
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                try
                {

                    Broadcast brod = new Broadcast();
                    brod.EmailAddress = email;
                    brod.MobileNumber = mobile;
                    brod.IsMailSend = false;
                    brod.PaymentId = payId;
                    brod.Created_at = DateTime.UtcNow;
                    brod.CompanyId = ManageSession.CompanySession.Id;
                    brod.Event_Id = EventId;
                    brod.BroadcastMessageId = BMID;
                    dbConn.Broadcasts.Add(brod);
                    dbConn.SaveChanges();
                    //var events = dbConn.Broadcasts.Where(a => a.EmailAddress == email).FirstOrDefault();
                    //if(events == null)
                    //{

                    //    Broadcast brod = new Broadcast();
                    //    brod.EmailAddress = email;
                    //    brod.MobileNumber = mobile;
                    //    brod.IsMailSend = false;
                    //    brod.PaymentId = payId;
                    //    brod.Created_at = DateTime.UtcNow;
                    //    brod.CompanyId = ManageSession.CompanySession.Id;
                    //    brod.Event_Id = EventId;
                    //    brod.BroadcastMessageId = BMID;
                    //    dbConn.Broadcasts.Add(brod);
                    //    dbConn.SaveChanges();
                    //}
                }
                catch { return false; }
            }
            return true;
        }
        public static bool updatebroadcast(int BID)
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                try
                {
                    var events = dbConn.Broadcasts.Where(a => a.Id == BID).FirstOrDefault();
                    if (events != null)
                    {
                        events.IsMailSend = true;
                        dbConn.SaveChanges();
                    }
                }
                catch { return false; }
            }
            return true;
        }
        public static async Task<bool> SendBroadcast(int EventId, string EmailAddress, BroadcastEmails Broadcast)
        {
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                try
                {
                    var events = dbConn.Events.Include(a => a.MultimediaContents).Include(a => a.RSVPs).Where(a => a.Id == EventId).FirstOrDefault();
                    if (events != null)
                    {
                        ModelInviation Model = new ModelInviation();
                        var request = HttpContext.Current.Request;
                        var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
                        int webT = 1;
                        string Title = "Ticketsandinvites";
                        if (events.CreatedOnWebsite == (int)Ticketforwebsite.Roverman)
                        {
                            webT = 2;
                            Title = "RovermanProductions";
                            address = "https://www.rovermanproductions.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.Stream233)
                        {
                            webT = 3;
                            Title = "Stream233";
                            address = "https://www.stream233.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.Nationaltheatre)
                        {
                            webT = 4;
                            Title = "Nationalth";
                            address = "https://www.nationaltheatre.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.WorlaFest)
                        {
                            webT = 5;
                            Title = "WorlaFest";
                            address = "https://www.worlafest.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.EibStream)
                        {
                            webT = 6;
                            Title = "Eibstream";
                            address = "http://stream.eibstream.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.Thepolobeachclub)
                        {
                            webT = 7;
                            Title = "Thepolobeachclub";
                            address = "https://www.thepolobeachclub.com";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.rapperholic)
                        {
                            webT = 8;
                            Title = "RPRHOLIC";
                            address = "https://www.rapperholic.live";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.Charterhouse)
                        {
                            webT = 9;
                            Title = "CHATRHSE";
                            address = "https://www.charterhouse.live";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.Scribeproduction)
                        {
                            webT = 10;
                            Title = "Scribeproduction";
                            address = "https://www.scribeproductions.live";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.Bolaray)
                        {
                            webT = 11;
                            Title = "Bolaray";
                            address = "https://www.bolaray.live";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.parknwatch)
                        {
                            webT = 12;
                            Title = "PARKNWATCH";
                            address = "https://www.parknwatch.biz";
                        }
                        else if (events.CreatedOnWebsite == (int)Ticketforwebsite.LuxCinemagh)
                        {
                            webT = 13;
                            Title = "LuxCinemagh";
                            address = "https://www.luxcinemagh.com";
                        }
                        else
                        {
                            webT = 1;
                            Title = "Ticketsandinvites";
                            address = "https://www.ticketsandinvites.com";
                        }
                        Model.EventId = EventId;
                        Model.message = Broadcast.Message;
                        Model.EventName = events.Event_name;
                        //Model.NameTosend = Nametosend;
                        string url;
                        var img = events.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1 && x.Mul_MainPic == true);
                        if (img != null) { url = img.URL.Substring(5); }
                        else
                        {
                            //url = (events.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1).URL).Substring(5);
                            img = events.MultimediaContents.FirstOrDefault(x => x.Mul_Type == 1);//.Substring(5);  
                            if (img != null)
                            {
                                url = img.URL.Substring(5);
                            }
                            else
                            {
                                url = "images/placeholder-image.jpg";
                            }
                        }
                        Model.Image = address + "/" + url;
                        //Model.Image = "http://scannpass.adequateshop.com/" + (events.MultimediaContents.Where(a => a.Mul_Type == (int)MultiMediaType.image).FirstOrDefault().URL).Substring(6);
                        //events.MultimediaContents.Where(a => a.Mul_Type == (int)MultiMediaType.image).FirstOrDefault().URL;
                        Model.EventDate = events.StartDate;
                        Model.Venuename = events.Venue;
                        Model.BaseUrl = address;
                        Model.RsvpName = events.RSVPs != null && events.RSVPs.Count() > 0 ? events.RSVPs.FirstOrDefault().Namer : "";
                        Model.Contact = events.RSVPs != null && events.RSVPs.Count() > 0 ? events.RSVPs.FirstOrDefault().Phone : "";
                        //Guid id = Guid.NewGuid();

                        string body = EmailSending.RunCompile(HttpContext.Current.Server.MapPath("/Views/EmailTemplate/"), "Broadcast.cshtml", "", Model);
                        return await EmailSending.SendEmail(EmailAddress, body, "Stream233", Broadcast.Subject, webT);
                    }
                    return true;
                }
                catch (Exception ex)
                {

                }
                return false;
            }
        }
    }

}