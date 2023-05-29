using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventManager1.Controllers
{
    public class GrabTicketController : Controller
    {
        // GET: GrabTicket
        public ActionResult Index(int tid = 0)
        {
            int eventID = 0;
            TicketModelPopup result = new TicketModelPopup();
            LoginphoneModel ru = new LoginphoneModel();
            try
            {
                if (tid > 0)
                {

                    //if (returnUrl != null) { TempData["returnUrl"] = returnUrl; }
                    ViewBag.Ticket = tid;
                    ru.Country = CommonDbLogic.GetCountry();
                    foreach (var item in ru.Country)
                    {
                        if (item.Text == "Ghana")
                        { ru.CountryId = item.Id; }
                    }
                    eventID = HandleEvent.getEvent(tid);
                    if (eventID > 0)
                    {
                        var EventType = DbLogic.GetEventType(eventID);
                        if(EventType==(int)EventTypeEnum.Live)
                        {
                            ViewBag.Live = true;
                        }

                        ViewBag.evId = eventID;
                        HandleEvent hdb = new HandleEvent();
                        HandleUser db = new HandleUser();
                        result = DbLogic.GetTicket(eventID);
                        if (result != null && result.Tickest != null && result.Tickest.Count() > 0)
                        {
                            var tic = result.Tickest.ToList();
                            result.Tickest = tic;
                            result.Company = HandleEvent.getcompany(eventID);
                            ManageSession.CompanySession = new CompanySession();
                            ManageSession.CompanySession.CompName = result.Company.Name;
                            ManageSession.CompanySession.CompanyId = result.Company.Id;
                            ManageSession.CompanySession.FirstName = result.Company.Logo;
                            ManageSession.CompanySession.website = result.Company.website;
                        }
                        else
                        {
                            ViewData["error"] = "this event don't have ticket or ticket is disabled please check";
                            return View("Error");
                        }
                    }
                    result.loginpopup = ru;
                    if (ManageSession.UserSession != null)
                    {
                        ViewBag.usersession = ManageSession.UserSession.Id;
                      
                        //var tick = GetSummarytick(tid);
                        //if(tick != null)
                        //{
                        //    result.VideoTicket = tick.VideoTicket; //result.LivestreamTiccket = tick.LivestreamTiccket;
                        //    result.Tickest = tick.Tickest;
                        //}
                    }
                }
                else
                {
                    if ((ManageSession.UserSession != null))
                    {
                      return  RedirectToAction("GetAllTickets", "GrabTicket");
                        ViewBag.usersession = ManageSession.UserSession.Id;
                    }
                }
                //common.GenerateBarcode("1");            
                return View(result);
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;
                return View("Error");
            }

        }
        public string EnryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetSummary(int tid = 0)
        {
            int eventID = 0;
            TicketModelPopup result = new TicketModelPopup();
            ViewBag.tid = tid;
            if (tid > 0)
            {
                eventID = HandleEvent.getEvent(tid);
                //if (returnUrl != null) { TempData["returnUrl"] = returnUrl; }
                HandleEvent hdb = new HandleEvent();
                HandleUser db = new HandleUser();
                result = DbLogic.GetTicket(eventID);
                var tic = result.Tickest.Where(x => x.TicketId == tid).ToList();
                result.Tickest = tic;
                ViewBag.evId = eventID;
                var act = hdb.GetUserTickets(ManageSession.UserSession.Id);
                foreach (var item in act)
                {
                    if (item.EventId == eventID)
                    {
                        if (item.Eventtype == 2)
                        {
                            if (item.streamType == 2)
                            {
                                string evid = EnryptString(item.EventId.ToString());
                                // return Redirect("/OnlineStreaming/Index/?ev=" + item.evid);
                                result.LivestreamTiccket = "/OnlineStreaming/Index/?ev=" + item.evid;
                            }
                        }
                        else if (item.Eventtype == 1)
                        {
                            result.VideoTicket = "/User/Events/Index?v=" + item.Multimedia.FirstOrDefault().videoId;
                            //return Redirect("/User/Events/Index?v=" + item.Multimedia.FirstOrDefault().videoId);
                        }

                    }
                }

            }
            //common.GenerateBarcode("1");            
            return PartialView("_SummaryPartial", result);
        }
        public TicketModelPopup GetSummarytick(int tid = 0)
        {
            int eventID = 0;
            TicketModelPopup result = new TicketModelPopup();

            if (tid > 0)
            {
                eventID = HandleEvent.getEvent(tid);
                //if (returnUrl != null) { TempData["returnUrl"] = returnUrl; }
                HandleEvent hdb = new HandleEvent();
                HandleUser db = new HandleUser();
                result = DbLogic.GetTicket(eventID);
                var tic = result.Tickest.ToList();
                result.Tickest = tic;

                var act = hdb.GetUserTickets(ManageSession.UserSession.Id);
                foreach (var item in act)
                {
                    if (item.EventId == eventID)
                    {
                        if (item.Eventtype == 2)
                        {
                            if (item.streamType == 2)
                            {
                                string evid = EnryptString(item.EventId.ToString());
                                // return Redirect("/OnlineStreaming/Index/?ev=" + item.evid);
                                result.LivestreamTiccket = "/OnlineStreaming/Index/?ev=" + item.evid;
                            }
                        }
                        else if (item.Eventtype == 1)
                        {
                            result.VideoTicket = "/User/Events/Index?v=" + item.Multimedia.FirstOrDefault().videoId;
                            //return Redirect("/User/Events/Index?v=" + item.Multimedia.FirstOrDefault().videoId);
                        }

                    }
                }
            }
            //common.GenerateBarcode("1");            
            return result;
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetticketURL(int tid = 0)
        {
            string URL = "";
            if (tid > 0)
            {
                HandleEvent ev = new HandleEvent();
                var tickets = ev.GetUserTickets(ManageSession.UserSession.Id);
                Event_ ticket = new Event_();
                foreach (var i in tickets)
                {
                    if (i.Tickets.FirstOrDefault(y => y.Id == tid) != null)
                    {
                        ticket = i;
                    }
                }
                // var ticket = tickets.FirstOrDefault(x => x.Tickets.FirstOrDefault(y => y.Id == tid).Id == tid);
                if (ticket != null && ticket.EventName != null)
                {
                    if (ticket.Eventtype == 1 && ticket.paymentStatus == 1)
                    {
                        URL = "/User/Events/Index?v=" + ticket.Multimedia.FirstOrDefault().videoId;
                    }
                    else if (ticket.Eventtype == 2 && ticket.paymentStatus == 1)
                    {

                        URL = "/OnlineStreaming/Index/?ev=" + ticket.evid;
                    }
                    else
                    {
                        //URL = "/User/User/Tickets?id=" + ticket.Tickets.FirstOrDefault().TmapID + "&tmap=" + ticket.Tickets.FirstOrDefault().TmapID;
                        //URL = "/user/user/dashboard";
                    }
                }

            }
            //common.GenerateBarcode("1");            
            return Json(URL);
        }

        public ActionResult GetAllTickets()
        {
            common cdb = new common();
            HandleEvent hdb = new HandleEvent();
            List<Event_> ct = new List<Event_>();
            try
            {
                List<TicketTypemodel> tType = cdb.GetticketTypes();
                var act = hdb.GetUserTickets(ManageSession.UserSession.Id);
                ct = act.ToList();
            }
            catch (Exception ex) { RedirectToAction("Index", "GrabTicket", new { area = "" }); }
            return View(ct);
        }
    }
}