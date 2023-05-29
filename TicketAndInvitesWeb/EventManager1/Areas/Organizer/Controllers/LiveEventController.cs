using EventManager1.Areas.Organizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventManager1.Areas.Organizer.Controllers
{
    [MvcApplication.OrganizerSessionExpire]
    public class LiveEventController : Controller
    {
        // GET: Organizer/LiveEvent
        public ActionResult Index()
        {
            HandleLiveEvent hdb = new HandleLiveEvent();
            var res = hdb.GetLiveEvents();
            return View(res);
        }
        public ActionResult UpdateLiveKey(int id)
        {
            HandleLiveEvent hdb = new HandleLiveEvent();
            var urls = hdb.updateEventID(id);            
            //return Redirect(Liveurl);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Updatestreamtype(string eventID, string type)
        {
            HandleLiveEvent hdb = new HandleLiveEvent();
            var res = hdb.updateStreamtype(Convert.ToInt32( eventID), Convert.ToInt32( type));
            return Json(1);
        }
        [HttpPost]
        public ActionResult StartStreaming(string eventID)
        {
            HandleLiveEvent hdb = new HandleLiveEvent();
            var res = hdb.StartStream(Convert.ToInt32(eventID));
            return Json(1);
        }
        [HttpPost]
        public ActionResult StopStreaming(string eventID)
        {
            HandleLiveEvent hdb = new HandleLiveEvent();
            var res = hdb.StopStream(Convert.ToInt32(eventID));
            return Json(1);
        }
    }
}