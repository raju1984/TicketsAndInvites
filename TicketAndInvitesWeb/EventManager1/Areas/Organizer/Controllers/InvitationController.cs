using EventManager1.Areas.Organizer.Models;
using EventManager1.DBCon;
using EventManager1.Models;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using static EventManager1.MvcApplication;

namespace EventManager1.Areas.Organizer.Controllers
{

    [OrganizerSessionExpire]
    public class InvitationController : Controller
    {
        // GET: Organizer/Invitation
        HandleEvent hdb = new HandleEvent();
        public ActionResult Index(string date = null, string EventType = null, string Eventname = null, string EventId = null)
        {
            List<Event_> ct = new List<Event_>();
            try
            {
                ct = hdb.GetEvents(ManageSession.CompanySession.Id);
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
                            ct = ct.Where(a => Convert.ToDateTime(a.EndDate).Date < DateTime.UtcNow.Date).ToList();
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
            return View(ct);
        }
        // GET: Organizer/SendInvitation
        public ActionResult SendInvitation(int EventId)
        {
            ExceldataModelView result = new ExceldataModelView();
            result = OrganizerDbOperation.GetExceldata(Convert.ToInt32(EventId));
            result.Events = new List<Dropdownlist>();
            result.Events = OrganizerDbOperation.GetEventNamebyUser(ManageSession.CompanySession.Id);
            return View(result);
        }
        //[HttpPost]
        //public ActionResult UploadExcel(HttpPostedFileBase FileUpload,int EventId)
        //{
        //    try
        //    {
        //        EventmanagerEntities objEntity = new EventmanagerEntities();
        //        string data = "";
        //        List<InvitationDetail> result = new List<InvitationDetail>();
        //        if (FileUpload != null)
        //        {
        //            if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //            {

        //                string filename = FileUpload.FileName;

        //                if (filename.EndsWith(".xlsx"))
        //                {
        //                    string targetpath = Server.MapPath("~/Content/InvitationExcel");
        //                    FileUpload.SaveAs(targetpath + filename);
        //                    string pathToExcelFile = targetpath + filename;
        //                    string sheetName = "Sheet1";
        //                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
        //                    var empDetails = from a in excelFile.Worksheet<InvitationDetail>(sheetName) select a;
        //                    foreach (var a in empDetails)
        //                    {
        //                        if (a.EmailAddress != null)
        //                        {
        //                            result.Add(a);
        //                        }
        //                    }
        //                    data = "success";
        //                    ManageSession.ExcelDataSession = new List<InvitationDetail>();
        //                    ManageSession.ExcelDataSession.AddRange(result);
        //                    return View("SendInvitation");
        //                }
        //                else
        //                {
        //                    data = "This file is not valid format";
        //                    ViewBag.Message = data;
        //                }
        //            }
        //            else
        //            {
        //                data = "Only Excel file format is allowed";
        //                ViewBag.Message = data;
        //                return View("SendInvitation");

        //            }

        //        }
        //        else
        //        {
        //            data = "Please choose Excel file";
        //            ViewBag.Message = data;
        //            return View("SendInvitation");
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        return View("SendInvitation");
        //    }
        //}

        [HttpPost]
        public JsonResult UploadFiles()
        {
            List<InvitationDetail> result = new List<InvitationDetail>();
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            var model = Request.Form["EventId"];
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0 && !string.IsNullOrEmpty(model))
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase FileUpload = files[i];
                        if (FileUpload != null)
                        {
                            if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || FileUpload.ContentType== "application/octet-stream")
                            {

                                string filename = FileUpload.FileName;

                                if (filename.EndsWith(".xlsx"))
                                {
                                    string targetpath = Server.MapPath("~/Content/InvitationExcel");
                                    FileUpload.SaveAs(targetpath + filename);
                                    string pathToExcelFile = targetpath + filename;
                                    string sheetName = "Sheet1";
                                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                                    var empDetails = from a in excelFile.Worksheet<InvitationDetail>(sheetName) select a;
                                    foreach (var a in empDetails)
                                    {
                                        if (a.EmailAddress != null)
                                        {
                                            result.Add(a);
                                        }
                                    }
                                    resp.Code = (int)ApiResponseCode.ok;
                                    // Returns message that successfully uploaded  
                                    resp.Msg = "File Uploaded Successfully!";
                                    ManageSession.ExcelDataSession = new List<InvitationDetail>();
                                    ManageSession.ExcelDataSession.AddRange(result);
                                    return Json(resp, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    resp.Msg = "This file is not valid format";
                                    return Json(resp, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                resp.Msg = "Only Excel file format is allowed";
                                return Json(resp, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    resp.Msg = "Error occurred. Error details: " + ex.Message;
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
            }
            resp.Msg = "Only Excel file format is allowed";
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BindExceldata(string EventId, string EventUserId, string page ="", int SenderType=1)
        {
            ExceldataModelView result = new ExceldataModelView();
            try
            {
                //result.InvitationDetail = new List<InvitationDetail>();
                result = OrganizerDbOperation.GetExceldata(Convert.ToInt32(EventId));
                if (string.IsNullOrEmpty(EventUserId))
                {
                    //get data from excel sheet
                    result.InvitationDetail = ManageSession.ExcelDataSession;
                }
                else
                {
                    ManageSession.ExcelDataSession = new List<InvitationDetail>();
                    result.InvitationDetail = new List<InvitationDetail>();
                    //get user from previous event
                    result.InvitationDetail = OrganizerDbOperation.GetUserbyEvent(Convert.ToInt32(EventUserId));
                    ManageSession.ExcelDataSession = result.InvitationDetail;
                }
                if(page != "") { ViewBag.foot = "footer"; }
                return PartialView("_ExcelDataPreview", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_ExcelDataPreview");
        }


        [HttpPost]
        public JsonResult SendInvitation(string EventId, string EventName, string SendType)
        {
            try
            {
                EventmanagerEntities DbEntity = new EventmanagerEntities();
                List<InvitationDetail> result = ManageSession.ExcelDataSession;
                if (result != null)
                {
                    int sentype = Convert.ToInt32(SendType);
                    decimal? Costperemail = 0;
                    OrganizerPay op = new OrganizerPay();
                    op.AdminFeeEmail = DbEntity.PaymentSetups.FirstOrDefault().InvitationOrg;
                    op.AdminFeeSMS = DbEntity.PaymentSetups.FirstOrDefault().InvitationOrgSms;
                    decimal totalcost = 0; int totalinvites = 0;
                    foreach(var i in result)
                    {
                        if(i.EmailAddress != null) { totalcost = Convert.ToDecimal(totalcost + op.AdminFeeEmail); totalinvites++; i.SendInviteType = (int)SendInvitationType.Email; }
                        if(i.MobileNumber != null) { totalcost = Convert.ToDecimal(totalcost + op.AdminFeeSMS); totalinvites++; i.SendInviteType = (int)SendInvitationType.SMS; }
                        if(i.EmailAddress != null && i.EmailAddress != null) { i.SendInviteType = (int)SendInvitationType.Both; }

                    }



                    //if (sentype == (int)SendInvitationType.Email)
                    //{
                    //    Costperemail = op.AdminFeeEmail;
                    //}
                    //else if (sentype == (int)SendInvitationType.SMS)
                    //{
                    //    Costperemail = op.AdminFeeSMS;
                    //}
                    //else if (sentype == (int)SendInvitationType.Both)
                    //{
                    //    Costperemail = op.AdminFeeEmail + op.AdminFeeSMS;
                    //}
                    op.eventId = EventId;
                    op.EventName = EventName;
                    op.total = totalcost.ToString(); //(result.Count() * Costperemail).ToString();
                    op.subtype = totalinvites.ToString(); //result.Count().ToString();
                    op.page = "Invitation";
                    op.SendInviteType = Convert.ToInt32(SendType);
                    TempData["op"] = op;
                    //foreach (var item in result)
                    //{
                    //    //PostExcelData(item, Convert.ToInt32(EventId));
                    //}
                    //return RedirectToAction("Sumemry", "Transaction");
                }
                //ManageSession.ExcelDataSession = null;
                
            }
            catch (Exception ex)
            {

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public int PostExcelData(int EventId, int paymId, InvitationDetail InvitationDetail, int sendType) //InvitationDetail InvitationDetail, int EventId)
        {
            //List<InvitationDetail> result = ManageSession.ExcelDataSession;
            //foreach( var InvitationDetail in result) { 
            try
            {
                EventmanagerEntities DbEntity = new EventmanagerEntities();
                //var checkforexisting = DbEntity.Invitations.Include(a => a.EventTickets).Where(a => a.EmailAddress == InvitationDetail.EmailAddress && a.Event_Id == EventId && (a.CompanyId == ManageSession.CompanySession.Id )).FirstOrDefault();
                //if (checkforexisting == null)
                //{

                    Invitation obj = new Invitation();
                    obj.Title = InvitationDetail.Title;
                   // obj.IsMailSend = false;
                    obj.FirstName = InvitationDetail.FirstName;
                    obj.LastName = InvitationDetail.LastName;
                    obj.Event_Id = EventId;
                    obj.PaymentId = paymId;
                    obj.EmailAddress = InvitationDetail.EmailAddress;
                    obj.MobileNumber = InvitationDetail.MobileNumber;
                    obj.CompanyId = ManageSession.CompanySession.Id;
                    obj.Created_at = DateTime.UtcNow;
                    obj.Updated_at = DateTime.UtcNow;
                    obj.SendType = sendType;
                    if (InvitationDetail.SendInviteType == 1)
                    {
                        obj.IsMailSend = false;
                    }
                    else if (InvitationDetail.SendInviteType == 2)
                    {
                        obj.IsSmsSend = false;
                    }
                    else if (InvitationDetail.SendInviteType == 3)
                    {
                        obj.IsMailSend = false;
                        obj.IsSmsSend = false;
                    }

                    EventTicket eventticket = new EventTicket();
                    eventticket = new EventTicket
                    {
                        Event_Id = EventId,
                        Quantity = 1,
                        Invitation = obj,
                        AvailableQuantity = 1,
                        Seat = InvitationDetail.SeatNumber,
                        tableNo = InvitationDetail.TableNumber,
                        ColorArea = InvitationDetail.ColorCode
                    };                      

                    DbEntity.EventTickets.Add(eventticket);
                    DbEntity.SaveChanges();
                    return eventticket.Id;

                //}
                //else
                //{
                //    checkforexisting.SendType = sendType;
                //    // checkforexisting.IsMailSend = false;
                //    if (checkforexisting.SendType == (int)SendInvitationType.Email)
                //    {
                //        checkforexisting.IsMailSend = false;
                //    }
                //    else if (checkforexisting.SendType == (int)SendInvitationType.SMS)
                //    {
                //        checkforexisting.IsSmsSend = false;
                //    }
                //    else if (checkforexisting.SendType == (int)SendInvitationType.SMS)
                //    {
                //        checkforexisting.IsMailSend = false;
                //        checkforexisting.IsSmsSend = false;
                //    }
                //    checkforexisting.PaymentId = paymId;
                //    var eventsticek = DbEntity.EventTickets.Where(a => a.Inviation_Id == checkforexisting.Id && a.Event_Id == EventId ).FirstOrDefault();
                //    if (eventsticek != null)
                //    {
                //        eventsticek.Seat = InvitationDetail.SeatNumber;
                //        eventsticek.tableNo = InvitationDetail.TableNumber;
                //        eventsticek.ColorArea = InvitationDetail.ColorCode;
                //        eventsticek.Updated_at = DateTime.UtcNow;
                //        DbEntity.SaveChanges();
                //        return eventsticek.Id;
                //    }
                //    else
                //    {
                //        EventTicket eventticket = new EventTicket
                //        {
                //            Event_Id = EventId,
                //            Quantity = 1,
                //            Invitation = checkforexisting,
                //            AvailableQuantity = 1,
                //            Seat = InvitationDetail.SeatNumber,
                //            tableNo = InvitationDetail.TableNumber,
                //            ColorArea = InvitationDetail.ColorCode
                //        };
                //        DbEntity.EventTickets.Add(eventticket);
                //        DbEntity.SaveChanges();
                //        return eventticket.Id;
                //    }
                   
                //}
            }
            catch (Exception ex)
            {

            }
            //}
            
            return 0;
        }
        
        [HttpPost]
        public JsonResult SendSinleInvitation(InvitationDetail Invt, string EventId, string EventName)
        {
            EventmanagerEntities DbEntity = new EventmanagerEntities();
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (Invt != null 
                    && !string.IsNullOrEmpty(Invt.FirstName) && !string.IsNullOrEmpty(EventId))
                {
                    ManageSession.ExcelDataSession = new List<InvitationDetail>();
                    ManageSession.ExcelDataSession.Add(Invt);
                    List<InvitationDetail> result = ManageSession.ExcelDataSession;
                    if (result != null)
                    {
                        decimal? Costperemail = 0;//= DbEntity.PaymentSetups.FirstOrDefault().InvitationUser;
                        OrganizerPay op = new OrganizerPay();
                        op.AdminFeeEmail = DbEntity.PaymentSetups.FirstOrDefault().InvitationOrg;
                        op.AdminFeeSMS = DbEntity.PaymentSetups.FirstOrDefault().InvitationOrgSms;
                        decimal totalcost = 0; int totalinvites = 0;
                        foreach (var i in result)
                        {
                            if (i.EmailAddress != null) { totalcost = Convert.ToDecimal(totalcost + op.AdminFeeEmail); totalinvites++; i.SendInviteType = (int)SendInvitationType.Email; }
                            if (i.MobileNumber != null) { totalcost = Convert.ToDecimal(totalcost + op.AdminFeeSMS); totalinvites++; i.SendInviteType = (int)SendInvitationType.SMS; }
                            if (i.EmailAddress != null && i.EmailAddress != null) { i.SendInviteType = (int)SendInvitationType.Both; }

                        }
                        //if (Invt.SendInviteType == (int)SendInvitationType.Email)
                        //{
                        //    Costperemail = op.AdminFeeEmail;
                        //}
                        //else if (Invt.SendInviteType == (int)SendInvitationType.SMS)
                        //{
                        //    Costperemail = op.AdminFeeSMS;
                        //}
                        //else if (Invt.SendInviteType == (int)SendInvitationType.Both)
                        //{
                        //    Costperemail = op.AdminFeeEmail + op.AdminFeeSMS ;
                        //}
                        //decimal? Costperemail = DbEntity.PaymentSetups.FirstOrDefault().InvitationOrg;
                        //OrganizerPay op = new OrganizerPay();
                        op.eventId = EventId;
                        op.EventName = EventName;
                        op.total = totalcost.ToString(); //(result.Count() * Costperemail).ToString();
                        op.subtype = totalinvites.ToString(); //result.Count().ToString();
                        //op.SendInviteType = Invt.SendInviteType;
                        op.page = "Invitation";
                        TempData["op"] = op;

                    }
                    //resp.Code = (int)ApiResponseCode.ok;
                    //int EventTickeId = PostExcelData(Invt, Convert.ToInt32(EventId));
                    //if (EventTickeId > 0)
                    //{
                    //    //send inviation email to user

                    //    ModelInviation.SendInvitation(Convert.ToInt32(EventId), EventTickeId, Invt.EmailAddress, Invt.FirstName + " " + Invt.LastName);

                    //    resp.Msg = "Invitation send Sucessfully";
                    //    return Json(resp, JsonRequestBehavior.AllowGet);
                    //}
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
                resp.Msg = "please fill neccessary details!";
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InvitationList(int EventId)
        {
            List<InvitationDetail> result = OrganizerDbOperation.GetInvition(EventId);
            return View(result);
        }
        public ActionResult SellAllInvitation(int EventId=0)
        {
            List<InvitationDetail> result = OrganizerDbOperation.GetInvition(EventId);
            return View(result);
        }


    }
}