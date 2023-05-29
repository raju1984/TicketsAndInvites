using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using static EventManager1.MvcApplication;

namespace EventManager1.Areas.Organizer
{
    [OrganizerSessionExpire]
    public class AllNotificationController : Controller
    {
        // GET: Organizer/AllNotification
        public ActionResult Notification()
        {
            List<NotificationModel> result = new List<NotificationModel>();
            try { result = CommonDbLogic.GetNotification(1); }
            catch { }
            return View(result);
        }
        public ActionResult GetNotification()
        {
            try
            {
                List<NotificationModel> result = CommonDbLogic.GetNotification(1);

                return PartialView("_Notification", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_Notification");
        }
        public ActionResult Message(int mid=0)
        {
            List<NotificationModel> result = new List<NotificationModel>();           
            
            try {
                result = CommonDbLogic.GetMessage(0,1);

                if (mid > 0)
                {
                    List<NotificationModel> results = CommonDbLogic.GetMessage(mid);
                    ViewBag.msglist = results;
                    //return PartialView("_allmessage", results);
                }
            } 
            catch { }
            
            return View(result);
        }
        public ActionResult GetMessage(int id)
        {
            try
            {
                List<NotificationModel> result = CommonDbLogic.GetMessage(id);

                return PartialView("_allmessage", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_allmessage");
        }
        [HttpPost]
        public ActionResult Message(string txtmessagetitle, string txtmessage, int MsgId=0)
        {
            ApiResponse result = new ApiResponse();
            List<NotificationModel> results = new List<NotificationModel>();
            try
            {
                results = CommonDbLogic.GetMessage(0);
                if (MsgId > 0) {
                    result = CommonDbLogic.AddMessage(txtmessage, 1, 0, "", MsgId);
                    List<NotificationModel> resultss = CommonDbLogic.GetMessage(MsgId);
                    ViewBag.msglist = resultss;
                    return RedirectToAction("Message", new{ mid = MsgId });
                    //return View(new { mid = MsgId }, results);
                    //return RedirectToAction("Message?mid="+MsgId, results);                    
                }
                else {
                    result = CommonDbLogic.AddMessage(txtmessage, 1, 0, txtmessagetitle);
                    
                }
                
                
            }
            catch (Exception ex) { result.Msg = ex.ToString(); }
            ViewData["Error"] = result;
            return RedirectToAction("Message");
            //return View(results);
        }
        //public ActionResult Message(string txtmessage, int MsgId = 0)
        //{
        //    ApiResponse result = new ApiResponse();
        //    List<NotificationModel> results = new List<NotificationModel>();
        //    try
        //    {
        //        if (MsgId > 0)
        //        {
        //            result = CommonDbLogic.AddMessage(txtmessage, 1, 0, "", MsgId);
        //            return RedirectToAction("Message", new { mid = MsgId });
        //        }                

        //        results = CommonDbLogic.GetMessage(0);
        //    }
        //    catch (Exception ex) { result.Msg = ex.ToString(); }            

        //    return View(result);
        //}

    }
}