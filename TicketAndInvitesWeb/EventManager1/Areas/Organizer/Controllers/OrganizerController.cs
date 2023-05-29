using EventManager1.Areas.Admin.Models;
using EventManager1.Areas.Organizer.Models;
using EventManager1.DBCon;
using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static EventManager1.MvcApplication;

namespace EventManager1.Areas.Organizer.Controllers
{
    [OrganizerSessionExpire]
    public class OrganizerController : Controller
    {
        // GET: Organizer/Organizer
        EventmanagerEntities db = new EventmanagerEntities();
        common cdb = new common();
        HandleEvent hdb = new HandleEvent();
        // GET: Organizer
        public ActionResult CreateEvent()
        {
            var chkTimeOut = Session.Timeout;
            if (chkTimeOut <= 20)
            {
                // set new time out to session  
                Session.Timeout = 60;
            }
            int id = Convert.ToInt32(Request.QueryString["id"]);
            int clone = Convert.ToInt32(Request.QueryString["cl"]);
            List<EventManager1.Models.Country> ct = cdb.GetCountry();
            ViewBag.eventType = cdb.GetEventType();
            ViewBag.country = ct;
            ViewBag.compName = ManageSession.CompanySession.CompName;
            Event_ ev = new Event_();
            if (id > 0)
            {
                ev = cdb.GetEvents(id);
                ViewBag.id = id;
                ViewBag.cl = clone;

                //var x = ev.Multimedia;
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CreateEvent(Event_ Register)
        {
            var chkTimeOut = Session.Timeout;
            if (chkTimeOut <= 20)
            {
                //set new time out to session  
                Session.Timeout = 60;
            }
            var isValidModel = true;
            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            result.Msg = "Invalid Model";
            Register.BusinessOwner_Id = ManageSession.CompanySession.Id;
            if (isValidModel)
            {
                HandleEvent hdb = new HandleEvent();
                result = hdb.EventRegister(Register);
                return new JsonResult { Data = new { result } };
            }
            return new JsonResult { Data = new { result } };
        }
        [HttpPost]
        public ActionResult geteventbyid(int id)
        {
            Event_ ev = new Event_();
            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            try
            {
                if (id > 0)
                {
                    ev = cdb.GetEvents(id);
                }
                return Json(ev);
            }
            catch (Exception ex) { return new JsonResult { Data = new { result } }; }
        }
        public ActionResult CreateTicket(string date = null, string EventType = null, string Eventname = null, string EventId = null)
        {
            try
            {
                List<TicketTypemodel> tType = cdb.GetticketTypes();
                List<Event_> ct = hdb.GetEvents(ManageSession.CompanySession.Id);

                ViewBag.compName = ManageSession.CompanySession.CompName;

                if (ct != null && ct.Count() > 0)
                {
                    if (!string.IsNullOrEmpty(date))
                    {
                        ct = ct.Where(a => Convert.ToDateTime(a.StartDate).Date == Convert.ToDateTime(date).Date).ToList();
                    }

                    else if (!string.IsNullOrEmpty(EventId))
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

                    ViewData["events"] = ct;
                    ViewBag.tType = tType;

                }
            }
            catch (Exception ex) { RedirectToAction("Login", "Account", new { area = "" }); }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CreateTicket(createTicket ticket)
        {
            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            result.Msg = "Invalid Model";
            if (ticket.EventId > 0)
            {
                HandleEvent hdb = new HandleEvent();
                result = hdb.CreateTicket(ticket);
                return new JsonResult { Data = new { result } };

            }
            return new JsonResult { Data = new { result } };
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CreateTicketType(string ticket)
        {
            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            result.Msg = "Invalid Model";
            if (ticket != null)
            {
                HandleEvent hdb = new HandleEvent();
                result = hdb.CreateTicketType(ticket);
                return new JsonResult { Data = new { result } };

            }
            return new JsonResult { Data = new { result } };
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult DeleteTicket(int id)
        {
            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            result.Msg = "Invalid Model";
            if (id > 0)
            {
                HandleEvent hdb = new HandleEvent();
                result = hdb.DeleteTicket(id);
                return new JsonResult { Data = new { result } };

            }
            return new JsonResult { Data = new { result } };
        }
        [HttpPost]
        public ActionResult UpdateTicketQty(int id, int qty)
        {
            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            result.Msg = "Invalid Model";
            if (id > 0)
            {
                HandleEvent hdb = new HandleEvent();
                result = hdb.UpdateTicket(id, qty);
                return new JsonResult { Data = new { result } };

            }
            return new JsonResult { Data = new { result } };
        }
        [HttpPost]
        public ActionResult UpdateTicketstate(int id, bool isEnable)
        {
            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            result.Msg = "Something went wrong!";
            if (id > 0)
            {
                HandleEvent hdb = new HandleEvent();
                result = hdb.UpdateTicketstate(id, isEnable);
                return new JsonResult { Data = new { result } };

            }
            return new JsonResult { Data = new { result } };
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                string name = ManageSession.CompanySession.FirstName.ToString().Trim();
                try
                {

                    string[] imgpath = new string[3];
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        var request = System.Web.HttpContext.Current.Request;
                        var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
                        imgpath[i] = address + "/ImageEvent/" + name + "/" + fname.Replace(" ", "");
                        string path = Server.MapPath("~/ImageEvent/" + name); //Session["name"]
                        if (Directory.Exists(path))
                        { }
                        else { path = Directory.CreateDirectory(path).ToString(); }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/ImageEvent/"), path, fname.Replace(" ", ""));

                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json(imgpath);
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult UploadVideo()
        //{
        //    // Checking no of files injected in Request object  
        //    if (Request.Files.Count > 0)
        //    {
        //        string name = ManageSession.CompanySession.FirstName.ToString().Trim();
        //        try
        //        {

        //            string[] imgpath = new string[3];
        //            //  Get all files from Request object  
        //            HttpFileCollectionBase files = Request.Files;
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
        //                //string filename = Path.GetFileName(Request.Files[i].FileName);  

        //                HttpPostedFileBase file = files[i];
        //                string fname;

        //                // Checking for Internet Explorer  
        //                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //                {
        //                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //                    fname = testfiles[testfiles.Length - 1];
        //                }
        //                else
        //                {
        //                    fname = file.FileName;
        //                }
        //                var request = System.Web.HttpContext.Current.Request;
        //                var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
        //                imgpath[i] = address + "/VideoEvent/" + name + "/" + fname.Replace(" ", "");
        //                string path = Server.MapPath("~/VideoEvent/" + name); //Session["name"]
        //                if (Directory.Exists(path))
        //                { }
        //                else { path = Directory.CreateDirectory(path).ToString(); }

        //                // Get the complete folder path and store the file inside it.  
        //                fname = Path.Combine(Server.MapPath("~/VideoEvent/"), path, fname.Replace(" ", ""));

        //                file.SaveAs(fname);
        //            }
        //            // Returns message that successfully uploaded  
        //            return Json(imgpath);
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json("Error occurred. Error details: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        return Json("No files selected.");
        //    }
        //}
        [HttpPost]
        [AllowAnonymous]
        public ActionResult UploadVideo(string VideoPath="")
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                string name = ManageSession.CompanySession.FirstName.ToString().Trim();
                try
                {
                    // Returns message that successfully uploaded  
                    return Json(VideoPath);
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
        public ActionResult Dashboard()
        {
            DashboardOrgModel dbm = new DashboardOrgModel();
            try
            {

                dbm = OrganizerDbOperation.GetDashboardDataOrg();
            }
            catch (Exception ex) { }
            return View(dbm);
        }
        public ActionResult UserDashboardForOrganizer()
        {
            return View();
        }
       
        public JsonResult loginUserDashboardForOrganizer(string UserId="")
        {
            int msg = 0;
            //if (UserId =="")
            //{
            //    return View(UserId);
            //}
          
          
            if (UserId != "")
            {
                HandleUser hdb = new HandleUser();
                var login = hdb.LoginUserDashboardForOrganizer(UserId);
                if (login != null && !string.IsNullOrEmpty(login.FirstName) && login.Status == (int)companytatus.active)
                {
                    //FormsAuthentication.SetAuthCookie(model.Email, false);
                    //Response.Cookies["userName"].Value =login.Id;
                    HttpCookie userInfo = new HttpCookie("userInfo");
                    userInfo["UserName"] = login.Id.ToString();
                    userInfo["type"] = "2";
                    userInfo.Expires.Add(new TimeSpan(0, 1, 0));
                    userInfo.Domain = "stream233.com";
                    Response.Cookies.Add(userInfo);
                    TempData["temp"] = login;
                    ManageSession.UserSession = new UserSession();
                    ManageSession.UserSession.FirstName = login.FirstName;
                    ManageSession.UserSession.LastName = login.LastName;
                    ManageSession.UserSession.EmailId = login.EmailId;
                    ManageSession.UserSession.Id = login.Id;
                    ManageSession.UserSession.PhoneNo = login.PhoneNo;
                    msg = 1;
                }
               
              }
            //return RedirectToAction("Dashboard", "User", new { Area = "User" });
            return Json(msg,JsonRequestBehavior.AllowGet);
        }
        public ActionResult VisitorOverview(string EventId = null)
        {
            OvertrackModels result = new OvertrackModels();
            try
            {
                result.Events = OrganizerDbOperation.GetEventSearchEvent("");
                if (EventId == "null") { if (result.Events.Count > 0 && result.Events != null) { EventId = result.Events.FirstOrDefault().Id.ToString(); } }
                result.Tickets = OrganizerDbOperation.GetEventStatusTickets(EventId, ManageSession.CompanySession.Id);
                return PartialView("_VisitorOverview", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_VisitorOverview", result);
        }



        public ActionResult Invitation()
        {
            return View();
        }
        public ActionResult Offer()
        {
            return View();
        }

        public ActionResult Sale()
        {
            return View();
        }
        public ActionResult Settings()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Settings(FormCollection collection)
        {
            string email = collection["txtemail"];
            string name = collection["txtname"];
            ViewData["Error"] = OrganizerDbOperation.UpdateDetail(email, name, ManageSession.CompanySession.Id);
            return View();
        }
        [HttpPost]
        public ActionResult Security(FormCollection collection)
        {
            string current = collection["txtcurretpassword"];
            string newpass = collection["txtnewpassword"];
            ViewData["Error"] = OrganizerDbOperation.UpdatePassword(newpass, current, ManageSession.CompanySession.Id);
            return View("Settings");
        }
        public ActionResult Subscribe()
        {
            return View();
        }
        public ActionResult TicketDetail()
        {
            return View();
        }
        public ActionResult Tickets()
        {
            return View();
        }
        public ActionResult Withdrawal()
        {
            return View();
        }
        public ActionResult Broadcast()
        {
            return View();
        }
        public ActionResult History()
        {
            return View();
        }
        public ActionResult CompanyProfile(string tabtype = null)
        {
            return View(OrganizerDbOperation.GetComnayProfile(ManageSession.CompanySession.Id));
        }

        [HttpPost]
        public ActionResult UpdateCompanyDetail(string txtemail, string txtcmname, string txtnumber, string txtaddress, string CountryId, string hftype)
        {
            OrganizerDbOperation.UpdateCompanyDetail(txtemail, txtcmname, txtnumber, txtaddress, CountryId);
            return RedirectToAction("CompanyProfile", "Organizer", new { tabtype = hftype });
        }

        [HttpPost]
        public ActionResult UpdateAdminSecurity(FormCollection collection)
        {
            string current = collection["txtcurretpassword"];
            string newpass = collection["txtnewpassword"];
            ViewData["Error"] = OrganizerDbOperation.UpdatePassword(newpass, current, ManageSession.CompanySession.Id);
            return RedirectToAction("CompanyProfile");
        }

        [HttpPost]
        public ActionResult AddUpdateBankDetail(FormCollection collection)
        {
            string BankId = collection["BankId"];
            string txtaccounumber = collection["txtaccounumber"];
            string bankreg = collection["txtbankreg"];
            string txtholdername = collection["txtholdername"];
            string MobileAccountId = Convert.ToString(collection["MobileAccountId"]);
            if (MobileAccountId == "0\"")
            { MobileAccountId = "0"; }
            string txtmobileuniquenumber = collection["txtmobileuniquenumber"];

            BankDetails adobj = new BankDetails();
            adobj.BankId = !string.IsNullOrEmpty(BankId) ? Convert.ToInt32(BankId) : 0;
            adobj.Account_Number = txtaccounumber;
            adobj.Bank_Registration_Number = bankreg;
            adobj.Account_Holder_Name = txtholdername;
            if (MobileAccountId != "0")
            {
                adobj.MobileAccountId = !string.IsNullOrEmpty(MobileAccountId.ToString()) ? Convert.ToInt32(MobileAccountId) : 0;
            }
            adobj.mobile_money_UniqueId = txtmobileuniquenumber;
            OrganizerDbOperation.UpdateAddBankDetail(adobj);
            return RedirectToAction("CompanyProfile", new { tabtype = "bnkdetial" });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Ticketdetail(int eventid)
        {
            // Checking no of files injected in Request object 
            var check = true;
            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            result.Msg = "Invalid Model";
            if (check)
            {
                HandleEvent hdb = new HandleEvent();
                var results = hdb.Gettickets(eventid);
                return new JsonResult { Data = new { results } };
            }
            return new JsonResult { Data = new { result } };
        }

        public ActionResult Logout()
        {
            ManageSession.CompanySession = null;
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        [HttpPost]
        public ActionResult GetTicketbyid(int id)
        {
            List<EventTicket_> ev = new List<EventTicket_>();
            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            try
            {
                if (id > 0)
                {
                    ev = hdb.Gettickets(id);
                }
                return Json(ev);
            }
            catch (Exception ex) { return new JsonResult { Data = new { result } }; }
        }
        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult CreateTicket(string date =null, string EventType = null, string Eventname = null, string EventId = null)
        //{
        //    List<Event_> ct = new List<Event_>();
        //    try
        //    {
        //        ct = hdb.GetEvents(ManageSession.CompanySession.Id);
        //        if (ct != null && ct.Count() > 0)
        //        {
        //            if (!string.IsNullOrEmpty(date))
        //            {
        //                ct = ct.Where(a => Convert.ToDateTime(a.StartDate).Date == Convert.ToDateTime(date).Date).ToList();
        //            }
        //            else if (!string.IsNullOrEmpty(EventType))
        //            {
        //                if (EventType == "2")
        //                {
        //                    ct = ct.Where(a => Convert.ToDateTime(a.StartDate).Date == DateTime.UtcNow.Date).ToList();
        //                    //live
        //                }
        //                else if (EventType == "3")
        //                {
        //                    ct = ct.Where(a => Convert.ToDateTime(a.StartDate).Date < DateTime.UtcNow.Date).ToList();
        //                    //past
        //                }
        //                else if (EventType == "3")
        //                {
        //                    ct = ct.Where(a => Convert.ToDateTime(a.StartDate).Date >= DateTime.UtcNow.Date).ToList();
        //                    //upcoming
        //                }
        //                //1=all
        //            }
        //            if (!string.IsNullOrEmpty(EventId))
        //            {
        //                ct = ct.Where(a => a.Id == Convert.ToInt32(EventId)).ToList();
        //            }
        //            else if (!string.IsNullOrEmpty(Eventname))
        //            {
        //                ct = ct.Where(a => a.EventName.ToLower().Contains(Eventname.ToLower())).ToList();
        //            }
        //        }

        //        ViewBag.compName = Session["compName"];
        //    }
        //    catch (Exception ex) { }
        //    return View();
        //}
        public ActionResult BannerSetting(int Id = 0)
        {
            Banner brr = new Banner();
            HandleBanner hdb = new HandleBanner();
            //BannerTimer br = new BannerTimer();
            if (Id > 0)
            {
                brr = hdb.getBanner(Id);
            }
            else { brr.ImagePath = "../../images/placeholder-image.jpg"; }
            List<Banner> list = new List<Banner>();
            ViewBag.BannerList = hdb.getBannerList();             
            return View(brr);
        }
        public ActionResult BannerTimer(int Id = 0)
        {
            Banner brr = new Banner();
            BannerTimer br = new BannerTimer();
            if (Id > 0)
            {
                br = db.BannerTimers.Where(c => c.Id == Id).FirstOrDefault();
            }
            List<BannerTimer> list = new List<BannerTimer>();
            list = db.BannerTimers.Where(c => c.UserID == ManageSession.CompanySession.Id.ToString()).ToList();
            if (list != null && list.Count > 0)
            {
                ViewBag.BannerList = list;
            }
            return View(br);
        }
        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult SaveBanner(Banner bt)
        //{
        //    int i = 0;
        //    string Message = "";
        //    string FileName = "";
        //    string ImageURL = "";
        //    string StartTime = bt.StartTime.Replace(" ", string.Empty);
        //    string EndTime = bt.EndTime.Replace(" ", string.Empty);
        //    string extension = System.IO.Path.GetExtension(bt.Image.FileName);
        //    if (bt != null)
        //    {
        //        if (bt.ID > 0)
        //        {
        //            if (extension == ".jpg" || extension == ".png" || extension == ".gif" || extension == ".jpeg")
        //            {
        //                BannerTimer tt = db.BannerTimers.Where(id => id.Id == bt.ID).FirstOrDefault();
        //                Guid guid = Guid.NewGuid();
        //                FileName = guid.ToString() + Path.GetExtension(bt.Image.FileName);
        //                ImageURL = "/BannerImage/" + FileName;
        //                bt.Image.SaveAs(Server.MapPath(ImageURL));
        //                tt.ImagePath = ImageURL;
        //                tt.StartDate = bt.StartDate;
        //                tt.EndDate = bt.EndDate;
        //                tt.StartTime = StartTime;
        //                tt.EndTime = EndTime;
        //                tt.IsTimeEnable = bt.IsTimeEnable;
        //                tt.IsImageEnble = bt.IsImageEnble;
        //                tt.UserID = ManageSession.CompanySession.Id.ToString();
        //                db.Entry(tt).State = EntityState.Modified;
        //                i = db.SaveChanges();
        //                if (i > 0)
        //                {
        //                    Message = "Update";
        //                }
        //                else
        //                {
        //                    Message = "Failed";
        //                } 
        //            }
        //            else
        //            {
        //                Message = "Image Formate";
        //            }
        //        }
        //        else
        //        {
        //            if (extension == ".jpg" || extension == ".png" || extension == ".gif" || extension == ".jpeg")
        //            {
        //                var Id = db.BannerTimers.Count();
        //                if (Id > 0)
        //                {
        //                    Id = db.BannerTimers.Max(x => x.Id);
        //                }
        //                else
        //                {
        //                    Id = 1;
        //                }
        //                var Ent = db.BannerTimers;
        //                Guid guid = Guid.NewGuid();
        //                FileName = guid.ToString() + Path.GetExtension(bt.Image.FileName);
        //                ImageURL = "/BannerImage/" + FileName;
        //                bt.Image.SaveAs(Server.MapPath(ImageURL));
        //                BannerTimer ee = new BannerTimer
        //                {
        //                    Id = Id + 1,
        //                    ImagePath = ImageURL,
        //                    StartDate = bt.StartDate,
        //                    EndDate = bt.EndDate,
        //                    StartTime = StartTime,
        //                    EndTime = EndTime,
        //                    IsTimeEnable = bt.IsTimeEnable,
        //                    IsImageEnble = bt.IsImageEnble,
        //                    UserID = ManageSession.CompanySession.Id.ToString()
        //                };
        //                Ent.Add(ee);
        //                i = db.SaveChanges();
        //                if (i > 0)
        //                {
        //                    Message = "Success";
        //                }
        //                else
        //                {
        //                    Message = "Failed";
        //                } 
        //            }
        //            else
        //            {
        //                Message = "Image Formate";
        //            }
        //        }

        //    }

        //    return Json(Message, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SaveBannerbt(Banner bt)
        {
            string Message="";
            HandleBanner hdb = new HandleBanner();
            if(bt != null) { 
            var res = hdb.SaveBanner(bt);
            if(res==1)
            {
                Message = "Updated successfully.";
            }
                else
                {
                    Message = "Someting went wrong.";
                }
            }
            else
            {
                Message = "Invalid model.";
            }           

            return RedirectToAction("BannerSetting", "Organizer", new { area = "Organizer" });
        }
        public ActionResult DeleteBanner(int Id)
        {
            int i = 0;
            string Message = "";
            if (Id > 0)
            {
                BannerTimer ent = db.BannerTimers.Where(id => id.Id == Id).FirstOrDefault();
                db.Entry(ent).State = EntityState.Deleted;
                i = db.SaveChanges();
            }
            if (i > 0)
            {
                Message = "Delete";
            }
            else
            {
                Message = "Failed";
            }
            return Json(Message, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EnableDisableBannerTimer(int Id, int EnableTimer)
        {
            int i = 0;
            string Message = "";
            if (Id > 0)
            {
                BannerTimer ent = db.BannerTimers.Where(id => id.Id == Id).FirstOrDefault();
                ent.IsTimeEnable = EnableTimer;
                db.Entry(ent).State = EntityState.Modified;
                i = db.SaveChanges();
            }
            if (i > 0)
            {
                Message = "Delete";
            }
            else
            {
                Message = "Failed";
            }
            return Json(Message, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BannerShow()
        {
            List<BannerTimer> list = new List<BannerTimer>();
            list = db.BannerTimers.Where(c => c.UserID == ManageSession.CompanySession.Id.ToString() && c.IsImageEnble == 0).ToList();
            if (list != null && list.Count > 0)
            {
                ViewBag.BannerList = list;
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult UploadBannerFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                string name = ManageSession.CompanySession.FirstName.ToString().Trim();
                try
                {

                    string[] imgpath = new string[1];
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        var request = System.Web.HttpContext.Current.Request;
                        var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
                        imgpath[i] = address + "/Images/dashboard/" + fname.Replace(" ", "");
                        string path = Server.MapPath("~/Images/dashboard/" ); //Session["name"]
                        if (Directory.Exists(path))
                        { }
                        else { path = Directory.CreateDirectory(path).ToString(); }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Images/dashboard/"), path, fname.Replace(" ", ""));

                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json(imgpath);
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
        public ActionResult AWSStreamingPage()
        {
            List<AWSStreamPage> ct = new List<AWSStreamPage>();
            ct = hdb.GetAWSChannelDetailForAdmin();
            return View(ct);

        }
      
        [HttpPost]
        public ActionResult StartChannel(string ChannelId)
        {
            AWSChannelModal hdb = new AWSChannelModal();
            var res = hdb.Startchannels(ChannelId);
            return Json(1);
        }
        [HttpPost]
        public ActionResult StopChannel(string ChannelId)
        {
            AWSChannelModal hdb = new AWSChannelModal();
            var res = hdb.Stopchannels(ChannelId);
            return Json(1);
        }
    }
}