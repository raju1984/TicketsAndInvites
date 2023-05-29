using EventManager1.Areas.User.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventManager1.Models;
namespace EventManager1.Areas.User.Controllers
{
    [MvcApplication.UserSessionExpire]
    public class EventsController : Controller
    {
        // GET: User/Events

        public ActionResult Index(string v)
        {
            //Create a Cookie with a suitable Key.
            //HttpCookie videoInfo = new HttpCookie("video");
            var res = HandleEvents.GetEVideo(v);
            HandleLiveEvent hdb = new HandleLiveEvent();
           
            if (Request.Cookies["CheckVideoSession"] == null || Request.Cookies["VideoId"] == null)
            {
                Random r = new Random();
                int VideoSession = r.Next(10000, 999999);
                Response.Cookies["CheckVideoSession"].Value = VideoSession.ToString();
                Response.Cookies["VideoId"].Value = res.EventId.ToString();

                hdb.UserLoggedForLiveStreaming(ManageSession.UserSession.Id, res.EventId, VideoSession);
            }
            else if (Request.Cookies["CheckVideoSession"].Value == "" || Request.Cookies["VideoId"].Value == "")
            {
                Random r = new Random();
                int VideoSession = r.Next(10000, 999999);
                Response.Cookies["CheckVideoSession"].Value = VideoSession.ToString(); Response.Cookies["VideoId"].Value = res.EventId.ToString();
                hdb.UserLoggedForLiveStreaming(ManageSession.UserSession.Id, res.EventId, VideoSession);
            }

            TempData["v"] = res.videoUrl;
                ViewBag.KeySecret = res.videoUrl;
                if (res.EventId < 140)
                {
                    return View(res);
                }
                else
                {
                    return View("Video", res);
                }

            


            //var Video = hdb.GetUserStatusForVideoStreaming(ManageSession.UserSession.Id);

            //if (Convert.ToInt32(Session["CheckVideoSession"]) == Video.VideoSession)
            //{

            //}
            //else
            //{
            //    return RedirectToAction("Logout", "User", new { area = "User" });
            //}


        }


        public ActionResult PlayVideo(string Secret)
        {
            var res = HandleEvents.GetEVideo(Secret);
            if (res != null)
            {
                string clean = res.videoUrl.ToString();
                string result = "~" + clean.Substring(clean.IndexOf("VideoEvent") - 1);
                string fn = Server.MapPath(result);
                var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes(fn));
                return new FileStreamResult(memoryStream, MimeMapping.GetMimeMapping(System.IO.Path.GetFileName(fn)));
            }
            else return null;

        }

        [Authorize]
        public ActionResult GetMedia()
        {
            if (TempData["v"] != null)
            {
                string path = TempData["v"].ToString();
                string fn = Server.MapPath(path);
                var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes(fn));
                return new FileStreamResult(memoryStream, MimeMapping.GetMimeMapping(System.IO.Path.GetFileName(fn)));
            }
            return null;
        }
        public ActionResult Eevent()
        {
            return View();
        }
        public ActionResult LiveEvents()
        {
            HandleLiveEvent hdb = new HandleLiveEvent();
            var urls = hdb.GetLiveURL();
            //TempData["urls"] = urls;
            // ViewBag.Liveurl = "/Stream#" + urls;
            //return Redirect(Liveurl);
            return View(urls);
        }

        public ActionResult LiveStream(int id)
        {
            HandleLiveEvent hdb = new HandleLiveEvent();
            var urls = hdb.GetLiveURL();
            ViewBag.Liveurl = urls.FirstOrDefault(x => x.id == id).EventKey + ".flv";
            //return Redirect(Liveurl);
            return View(urls);
        }
        [HttpPost]
        public JsonResult UserLoggedForVideo()
        {
            
            bool msg;
            int VideoId = Convert.ToInt32(Request.Cookies["VideoId"].Value);
            HandleLiveEvent hdb = new HandleLiveEvent();
            var Video = hdb.GetUserStatusForVideoStreaming(ManageSession.UserSession.Id, VideoId);
          
            if (Convert.ToInt32(Request.Cookies["CheckVideoSession"].Value) == Video.VideoSession)
            {
                msg = true;
            }
            else
            {
                msg = false;
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

    }
}