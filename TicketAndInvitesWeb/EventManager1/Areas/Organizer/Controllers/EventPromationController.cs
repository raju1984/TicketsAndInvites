using EventManager1.Areas.Organizer.Models;
using EventManager1.DBCon;
using EventManager1.Models;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using static EventManager1.MvcApplication;

namespace EventManager1.Areas.Organizer.Controllers
{
    [OrganizerSessionExpire]
    public class EventPromationController : Controller
    {
        // GET: Organizer/EventPromation
        public ActionResult Promotion(int type = 0, string startdate = null, string enddate = null)

        {
            return View(OrganizerDbOperation.GetSalesOverView(ManageSession.CompanySession.CompanyId, type, startdate, enddate));
        }

        // GET: Organizer/EventPromation
        public ActionResult Offers()
        {
            OffersModel model = new OffersModel();            
            model.ExistingOffers = OrganizerDbOperation.GetOffers();
            model.Events = OrganizerDbOperation.GetEventName("");
            return View(model);
        }
        // GET: Organizer/EventPromation
        public ActionResult PostOffers(OffersModel OffersModel)
        {
            if (OffersModel != null && OffersModel.valus > 0 && OffersModel.EventId != 0 && OffersModel.OfferPageCategory == (int)OfferPageCategory.Offer)
            {
                OrganizerDbOperation.AddOffers(OffersModel);

                return RedirectToAction("Offers");
                //return View();
            }
            else
            {
                EventmanagerEntities DbEntity = new EventmanagerEntities();
                decimal? CoupanFee = DbEntity.PaymentSetups.FirstOrDefault().CoupanFee;
                Session["Offers"] = OffersModel;
                OrganizerPay op = new OrganizerPay();
                op.eventId = OffersModel.EventId.ToString();
                op.EventName = DbEntity.Events.FirstOrDefault(x => x.Id == OffersModel.EventId).Event_name; ;
                op.page = "Coupan";
                op.subtype = OffersModel.NoofCoupans.ToString();
                op.total = (OffersModel.NoofCoupans * CoupanFee).ToString();
                op.AdminFee = CoupanFee.ToString();
                TempData["op"] = op;
                return RedirectToAction("Summery", "Transaction", new { Area = "Organizer" });
            }

        }
        // GET: Organizer/EventPromation
        public ActionResult EditOffers(int OfferId)
        {
            OffersModel model = new OffersModel();
            model = OrganizerDbOperation.GetOffersById(OfferId);
            model.Events = OrganizerDbOperation.GetEventName("");
            model.TicketTypeddl = OrganizerDbOperation.GetTicketType((int)model.EventId);
            return PartialView("_EditOffers", model);
        }
        public ActionResult GetCoupons(int OfferId)
        {
            List<CouponModel> model = new List<CouponModel>();
            model = OrganizerDbOperation.GetCoupons(OfferId);
            //model.Events = OrganizerDbOperation.GetEventName("");
            return PartialView("_CouponList", model);
        }

        

        [HttpPost]
        public JsonResult DeleteOffers(int OfferId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                resp= OrganizerDbOperation.DeleteOffers(OfferId);

            }
            catch (Exception ex)
            {
                resp.Code = (int)ApiResponseCode.fail;
                resp.Msg = ex.Message;
            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult GiftCoupon()
        {
            OffersModel model = new OffersModel();
            model.ExistingOffers = OrganizerDbOperation.GetOffers();
            model.Events = OrganizerDbOperation.GetEventName("");
            return View(model);
        }
        public JsonResult bindEventTicket(int EventId)
        {
            EventmanagerEntities Db = new EventmanagerEntities();
            List<Dropdownlist> result = (from r in Db.EventTickets
                                         where r.Event_Id == EventId && r.Price>0
                                         select new Dropdownlist
                                         {
                                             Id = r.Id,
                                             Text = r.TicketName
                                         }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SendCoupantouser(string CoupanCode, string mobile, string Phone_CountryCode)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                resp = OrganizerDbOperation.SendCoupan(CoupanCode,mobile, Phone_CountryCode);

            }
            catch (Exception ex)
            {
                resp.Code = (int)ApiResponseCode.fail;
                resp.Msg = ex.Message;
            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        // GET: Organizer/EventPromation
        public ActionResult Broadcast()
        {
            BroadcastModel bm = new BroadcastModel();
            bm.Events = OrganizerDbOperation.GetEventName("");
            ManageSession.EmailDataSession = new List<EmailList>();
            return View(bm);
        }
       
        public ActionResult GetBroadcastList(int EventId)
        {
            try
            {
                ExceldataModelView result = OrganizerDbOperation.GetBroadcastList(EventId);
                return PartialView("_PastBroadcast", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_PastBroadcast");
        }

        // GET: Organizer/EventPromation
        public ActionResult Subscribe()
        {
            try { ViewData["subscost"] = OrganizerDbOperation.getsubsctipcost(); }
            catch { }
            return View(OrganizerDbOperation.GetEventName(""));
        }
        
        [HttpPost]
        public JsonResult UpdateEvevntSubscription(string EventId, string SubscriptionType, int paysId)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (!string.IsNullOrEmpty(EventId) && !string.IsNullOrEmpty(SubscriptionType))
                {
                    Resp = OrganizerDbOperation.UpdateSubscription(Convert.ToInt32(EventId), Convert.ToInt32(SubscriptionType), paysId);
                }
            }
            catch (Exception ex)
            {
                Resp.Msg = ex.Message;
                return Json(Resp, JsonRequestBehavior.AllowGet);
            }
            return Json(Resp, JsonRequestBehavior.AllowGet);
        }       

        [HttpPost]
        public JsonResult BroadCost(BroadcastEmails broadcost, int type, string EventId, string EventName)
        {
            EventmanagerEntities DbEntity = new EventmanagerEntities();
            ApiResponse resp = new ApiResponse();
            
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                decimal? Costperemail = DbEntity.PaymentSetups.FirstOrDefault().Broadcast;
                broadcost.Youremail = ManageSession.CompanySession.EmailId;
                //double Costperemail = 0.5; //
                TempData["broadcastList"] = broadcost;
                resp.Msg = "email broadcast successfully!";
                OrganizerPay op = new OrganizerPay();
                op.eventId = EventId;
                op.EventName = EventName;
                op.page = "BroadCast";
                List <EmailList> emails = ManageSession.EmailDataSession; //get email from excel session
                if (emails != null && emails.Count >0 && broadcost != null && !string.IsNullOrEmpty(broadcost.Message) && !string.IsNullOrEmpty(broadcost.Subject))
                {
                    op.total = (emails.Count() * Costperemail).ToString();
                    op.subtype = emails.Count().ToString();
                    
                }
                if (broadcost != null && broadcost.Emails != null && !string.IsNullOrEmpty(broadcost.Message) && !string.IsNullOrEmpty(broadcost.Subject))
                {
                    op.total = (broadcost.Emails.Count() * Costperemail).ToString();
                    op.subtype = broadcost.Emails.Count().ToString();
                    List<EmailList> result = new List<EmailList>();
                    foreach (var i in broadcost.Emails)
                    {
                        result.Add(new EmailList { EmailAddress = i.EmailAddress, Mobile= i.Mobile });
                    }
                    ManageSession.EmailDataSession = new List<EmailList>();                    
                    ManageSession.EmailDataSession.AddRange(result);
                }
                TempData["op"] = op;
                return Json(op, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }       
    

        [HttpPost]
        public JsonResult UploadFiles()
        {
            List<EmailList> result = new List<EmailList>();
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
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
                            if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"|| FileUpload.ContentType == "application/octet-stream")
                            {

                                string filename = FileUpload.FileName;

                                if (filename.EndsWith(".xlsx"))
                                {
                                    string targetpath = Server.MapPath("~/Content/InvitationExcel");
                                    FileUpload.SaveAs(targetpath + filename);
                                    string pathToExcelFile = targetpath + filename;
                                    string sheetName = "Sheet1";
                                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                                    var empDetails = from a in excelFile.Worksheet<EmailList>(sheetName) select a;
                                    foreach (var a in empDetails)
                                    {
                                        if (!string.IsNullOrEmpty(a.EmailAddress))
                                        {
                                            result.Add(a);
                                        }
                                    }
                                    resp.Code = (int)ApiResponseCode.ok;
                                    // Returns message that successfully uploaded  
                                    resp.Msg = "File Uploaded Successfully!";
                                    ManageSession.EmailDataSession = new List<EmailList>();
                                    ManageSession.EmailDataSession.AddRange(result);
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
        public ActionResult BindBroadcastdata()
        {

            try
            {
                ExceldataModelView result = new ExceldataModelView();
                List<EmailList> email = ManageSession.EmailDataSession;
                //get user from previous event
                result.EmailList = email;
                return PartialView("_BroadcastList", result);
            }

            //try
            //{
            //    if (ManageSession.EmailDataSession != null) {
            //    List<EmailList> email = ManageSession.EmailDataSession;
            //    result.EmailList = email;
            //    return PartialView("_BroadcastList", result);
            //    }
            //}
            catch (Exception ex)
            {

            }
            return PartialView("_BroadcastList");
        }

        public ActionResult PastBroadcast(int EventId = 0)
        {
            BroadcastModel bm = new BroadcastModel();
            bm.Events = OrganizerDbOperation.GetEventName("", 1);
            return View(bm);
        }
        public ActionResult PastSubscription(int EventId=0)
        {
            SubcriptionViewModel model =new SubcriptionViewModel();
            model.Subcription = OrganizerDbOperation.GetSubcriptionHistory(EventId);
            model.Events = OrganizerDbOperation.GetEventName("", 1);
            return View(model);
        }
    }
}