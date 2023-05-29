using EventManager1.Areas.Organizer.Models;
using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static EventManager1.MvcApplication;

namespace EventManager1.Areas.User.Controllers
{
    [UserSessionExpire]
    public class ManageEventController : Controller
    {
        HandleEvent hdb = new HandleEvent();
        // GET: User/ManageEvent
        public ActionResult MyEvent(string date = null, string EventType = null, string Eventname = null, string EventId = null)
        {
            try
            {
                List<Event_> ct = hdb.GetEvents(ManageSession.UserSession.Id,1);
                if (ct != null && ct.Count() > 0)
                {
                    if (!string.IsNullOrEmpty(date))
                    {
                        ct = ct.Where(a => Convert.ToDateTime(a.StartDate).Date == Convert.ToDateTime(date).Date).ToList();
                    }
                    
                    if (!string.IsNullOrEmpty(EventId))
                    {
                        ct = ct.Where(a => a.Id == Convert.ToInt32(EventId)).ToList();
                    }
                    else if (!string.IsNullOrEmpty(Eventname))
                    {
                        ct = ct.Where(a => a.EventName.ToLower().Contains(Eventname.ToLower())).ToList();
                    }
                    else if (!string.IsNullOrEmpty(EventType))
                    {
                        if (EventType == "2")
                        {
                            ct = ct.Where(a => Convert.ToDateTime(a.EndDate).Date >= DateTime.UtcNow.Date && Convert.ToDateTime(a.StartDate).Date <= DateTime.UtcNow.Date).ToList();
                            //live
                        }
                        else if (EventType == "3")
                        {
                            ct = ct.Where(a => Convert.ToDateTime(a.StartDate).Date < DateTime.UtcNow.Date).ToList();
                            //past
                        }
                        else if (EventType == "4")
                        {
                            ct = ct.Where(a => Convert.ToDateTime(a.StartDate).Date >= DateTime.UtcNow.Date).ToList();
                            //upcoming
                        }
                        //1=all
                    }
                }

                ViewData["events"] = ct;
                // ViewBag.events = ct;
                ViewBag.compName = Session["compName"];
            }
            catch (Exception ex) { }
            return View();
        }
        public ActionResult AllTicket(int EventId)
        {
            try
            {
                List<TicketsModelView> result = OrganizerDbOperation.GetTickets(EventId);
                return PartialView("_Tickets", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_Tickets");
        }
        [HttpPost]
        public JsonResult GetEventName(string prefix)
        {
            try
            {
                return Json(OrganizerDbOperation.GetEventName(prefix), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Overview(string EventId = null)
        {
            OverModels result = new OverModels();
            result.Tickets = OrganizerDbOperation.GetEventOverviewTickets(EventId, ManageSession.CompanySession.Id);
            result.Events = OrganizerDbOperation.GetEventName("");
            return View(result);
        }
        public ActionResult GetInviation(int EventId)
        {
            try
            {
                List<InvitationDetail> result = OrganizerDbOperation.GetInvition(EventId);
                return PartialView("_Invitation", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_Invitation");
        }
    }
}