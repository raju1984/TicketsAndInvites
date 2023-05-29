using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventManager1.Models;

namespace EventManager1.Controllers
{
    public class OnlineStreamingController : Controller
    {
        // GET: OnlineStreaming
        public ActionResult Index(string ev = "")
        {
            //if(Session["LiveStream"]!=null)
            //{
            //    ev = Session["LiveStream"].ToString();
            //}
            if (ev != "")
            {
                string userid;
                //HttpCookie reqCookies = Request.Cookies["userInfo"];
                if (ManageSession.UserSession != null)
                {
                    //userid = reqCookies["UserName"].ToString();
                    userid = ManageSession.UserSession.Id.ToString();
                    var ip = Request.UserHostAddress;

                    HomeModel hdb = new HomeModel();
                    if (userid != "")
                    {
                        var id = DecryptString(ev);
                        var res = hdb.verifyuser(Convert.ToInt32(id), Convert.ToInt32(userid), ip);
                        if (res.eventkey != null)
                        {
                            ViewBag.liveurl = res.eventkey; //"http://18.222.135.45:8000/live/" + res.eventkey + "/index.m3u8"; //".flv";
                            ViewBag.eventname = res.eventname;
                            ViewBag.userId = userid;
                            ViewBag.EventId = id;
                            //ViewBag.username = res.username;
                            ViewBag.username = ManageSession.UserSession.FirstName.ToString();
                            ViewBag.date = res.datetime;
                            ViewBag.IsStreaming = res.IsStreaming;

                            Session["LiveStream"] = ev;
                        }
                        if (Request.Cookies["CheckVideoSession"] == null || Request.Cookies["VideoId"] == null)
                        {
                            Random r = new Random();
                            int VideoSession = r.Next(10000, 999999);
                            Response.Cookies["CheckVideoSession"].Value = VideoSession.ToString();
                            Response.Cookies["VideoId"].Value = id.ToString();

                            hdb.UserLoggedForLiveStreaming(Convert.ToInt32(userid), Convert.ToInt32(id), VideoSession);
                        }
                        else if (Request.Cookies["CheckVideoSession"].Value == "" || Request.Cookies["VideoId"].Value == "")
                        {
                            Random r = new Random();
                            int VideoSession = r.Next(10000, 999999);
                            Response.Cookies["CheckVideoSession"].Value = VideoSession.ToString();
                            Response.Cookies["VideoId"].Value = id.ToString();
                            hdb.UserLoggedForLiveStreaming(Convert.ToInt32(userid), Convert.ToInt32(id), VideoSession);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Userlogin", "Account", new { Area = "" });
                       
                    }
                }
                else {
                   
                    return RedirectToAction("Userlogin", "Account", new { Area = "",returnUrl= "/OnlineStreaming/Index?ev="+ev });
                }
            }

            return View();
        }
        public ActionResult Chat()
        {
            return View();
        }
        public ActionResult stream()
        {

            return View();
        }
        public ActionResult hlstest()
        {
            return View();
        }
        public ActionResult mediaelementhls()
        {
            return View();
        }
        public string DecryptString(string encrString)
        {
            byte[] b;
            string decrypted;
            try
            {
                b = Convert.FromBase64String(encrString);
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
            }
            catch (FormatException fe)
            {
                decrypted = "";
            }
            return decrypted;
        }
        public ActionResult Livestream(int Id = 0)
        {
            string userid;
            //HttpCookie reqCookies = Request.Cookies["userInfo"];
            if (ManageSession.UserSession != null)
            {
                userid = ManageSession.UserSession.Id.ToString();
                //userid = reqCookies["UserName"].ToString();
                //string utype = reqCookies["type"].ToString();
                string utype = "2";
                HomeModel hdb = new HomeModel();
                if (userid != "")
                {
                    ViewBag.type = utype;
                    liveevent evnt = new liveevent();
                    string url = Request.Url.AbsoluteUri.ToString();
                    ViewBag.ID = Id;
                    evnt = hdb.getwebrtcEvent(Id);
                    return View(evnt);
                }
            }
            //return Redirect("https://stream233.com/Account/Login");
            return RedirectToAction("Userlogin", "Account", new { Area = "" });
        }
        [HttpGet]
        public ActionResult savestream(string url, int eventId)
        {
            HomeModel hdb = new HomeModel();
            var res = hdb.InsertURL(url, eventId);
            return Json(true);
        }
        [HttpPost]
        public ActionResult message(string eventId)
        {
            var result = HandleChat.GetMessage(Convert.ToInt32(eventId));
            return new JsonResult { Data = new { result } };
        }
    }
}