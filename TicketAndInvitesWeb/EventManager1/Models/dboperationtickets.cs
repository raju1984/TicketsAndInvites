using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using EventManager1.DBCon;
namespace EventManager1.Models
{
    public class dboperationtickets
    {
        EventmanagerEntities db;
        public Event_ GetUserTickets(int id)
        {
            db = new EventmanagerEntities();
            Event_ ct = new Event_();
            //var cont = db.Events.Include("EventTickets").Where(s => s.Event_Id == id).ToList();
            var tick = db.TickeUserMaps.Where(x => x.Id == id && (x.Status == 1 || x.Status == 2)).FirstOrDefault();
            var tickettype = db.TicketTypes;

            List<Multimedia_Content> mlist = new List<Multimedia_Content>();
            List<EventTicket_> tickett = new List<EventTicket_>();
            try
            {
                var tic = db.EventTickets.Where(x => x.Id == tick.TicketId).FirstOrDefault();
                var tickets = tic.Event_Id;
                DateTime date = new DateTime();
                date = DateTime.UtcNow;
                var e = db.Events.Include("MultimediaContents").FirstOrDefault(x => x.Id == tickets && x.Status > 0);
                if (e != null)
                {
                    var urls = e.MultimediaContents.FirstOrDefault(x => x.Mul_MainPic == true);
                    string url;
                    if (urls == null)
                    {
                        url = e.MultimediaContents.FirstOrDefault().URL;
                    }
                    else { url = urls.URL; }
                    mlist.Add(new Multimedia_Content { URL = url });
                    string BarcodePath = HostingEnvironment.MapPath("~/Content/BarCode/" + tick.BarCodeNumber + ".jpg");
                    if (File.Exists(BarcodePath))
                    {
                    }
                    else
                    {
                        if (tick.BarCodeNumber == null)
                        {
                            tick.BarCodeNumber = common.GetUniqueBarCode(tick.TicketId);
                        }
                        common.GenerateBarcode(tick.BarCodeNumber);
                    }

                    tickett.Add(new EventTicket_ { Quantity = Convert.ToInt32(tick.Qty), Barcode = tick.BarCodeNumber, ColorArea = tic.ColorArea, GateNo = tic.GateNo, TicketName = tic.TicketName }); //tickettype.FirstOrDefault(y=>y.id == tic.Ticket_Type).TicketTypes
                    ct = (new Event_ { Id = tick.Id, UserName=tick.Name,IsTicket= tick.InvitationId==null?true:false, EventName = e.Event_name, Multimedia = mlist, StartDate = Convert.ToDateTime(e.StartDate).ToString(), Venue = e.Venue, BusinessOwner_Id = tick.Id, EndDate = Convert.ToDateTime(e.EndDate).ToString(), status = Convert.ToInt32(e.Status).ToString(), Tickets = tickett, CityName = db.Addresses.FirstOrDefault(x => x.Id == e.Address_Id).AddressLine });
                }
                else
                {

                }
            }
            catch (Exception ex) { }

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


        public List<TicketTypemodel> GetticketTypes()
        {
            db = new EventmanagerEntities();
            List<TicketTypemodel> ct = new List<TicketTypemodel>();
            var cont = db.TicketTypes.ToList();
            foreach (var i in cont) { ct.Add(new TicketTypemodel { Id = i.id, TicketTypes = i.TicketTypes }); };
            return ct;
        }
    }
}