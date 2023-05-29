using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventManager1.Controllers
{
    public class StreamController : Controller
    {
        // GET: Stream
        public ActionResult Index(int Id=0)
         {
            if (ManageSession.CompanySession != null || ManageSession.UserSession != null)
            {
                liveevent ev = new liveevent(); HandleLiveEvent hdb = new HandleLiveEvent();
                string url = Request.Url.AbsoluteUri.ToString();
                ViewBag.ID = Id;
                ev = hdb.getwebrtcEvent(Id);
                return View(ev);
            }
            else { return RedirectToAction("Index", "Home"); }
        }
        [HttpGet]
        public ActionResult savestream(string url,int eventId)
        {
            HandleLiveEvent hdb = new HandleLiveEvent();
            var res = hdb.InsertURL(url, eventId);
            return Json(true);
        }
    }
} 