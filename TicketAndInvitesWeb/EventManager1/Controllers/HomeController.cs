using EventManager1.Areas.Organizer.Models;
using EventManager1.DBCon;
using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EventManager1.Controllers
{
    public class HomeController : Controller
    {
        common cdb = new common();
        HandleEvent hdb = new HandleEvent();
        EventmanagerEntities db = new EventmanagerEntities();
        public ActionResult Index()
        {
            HandleBanner br = new HandleBanner();
            var Banner = br.getBannerList();
            ViewBag.BannerList = Banner;
            LoginphoneModel ru = new LoginphoneModel();
            //if (returnUrl != null) { TempData["returnUrl"] = returnUrl; }
            ru.Country = CommonDbLogic.GetCountry();
            foreach (var item in ru.Country)
            {
                if (item.Text == "Ghana")
                { ru.CountryId = item.Id; }
            }
            ViewBag.country = ru;
            //common.GenerateBarcode("1");
            if (ManageSession.UserSession != null && ManageSession.CompanySession != null)
            {
                return Redirect("/User/User/Dashboard");
            }
            else
            {
                return Redirect("/Grabticket/Index");
            }

        }

        //public ActionResult Index()
        //{
        //    try
        //    {
        //        HandleBanner br = new HandleBanner();
        //        var Banner = br.getBannerList();
        //        ViewBag.BannerList = Banner;

        //    }
        //    catch (Exception ex)
        //    {
        //        CusomlogWriter.Loghacksawgaming("Error Log:" + ex, "Error Log");

        //    }

        //    //common.GenerateBarcode("1");            
        //    return View();
        //}

        public ActionResult EventDetail(int Id = 0)
        {
            Id = Convert.ToInt32(Request.QueryString["Id"]);
            ManageSession.TicketCartSession = new TicketCartSession();
            if (Id <= 0) { return RedirectToAction("/"); }
            var res = DbLogic.GetEventDetail(Id);
            ViewBag.country = CommonDbLogic.GetCountry();
            TempData["evtype"] = res.EventType;
            return View(res);
        }
        public ActionResult About()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Terms()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Policy()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult CreateEvent()
        {
            return View();
        }

        public ActionResult MyTickets(int evId = 0, int comId = 0)
        {
            try
            {
                if (TempData["Error"] != null)
                {
                    ViewBag.Error = TempData["Error"];

                }
                if (evId != 0 && comId != 0)
                {
                    var res = CommonDbLogic.GetCompanyProfile(evId, comId);
                    ViewBag.URL = res.website;
                    ViewBag.Logo = res.Image;
                    ViewBag.comId = comId;
                    ViewBag.evId = evId;
                }
            }
            catch (Exception ex)
            { }

            return View();
        }

        public ActionResult TicketsLists(string EmailId = "", int comId = 0, int evId = 0)
        {
            EventmanagerEntities db = new EventmanagerEntities();
            List<Event_> ct = new List<Event_>();
            ViewBag.Email = EmailId;
            try
            {
                if (EmailId != null && EmailId != "" && comId != 0)
                {
                    var users = db.Users.Where(m => m.Email == EmailId).FirstOrDefault();
                    if (users != null)
                    {
                        List<TicketTypemodel> tType = cdb.GetticketTypes();
                        var act = hdb.GetUserTickets(users.Id, comId);
                        if (act != null && act.Count()>0)
                        {
                            ct = act.ToList();
                            if (ct[0].EventId != 0 && ct[0].BusinessOwner_Id != 0)
                            {
                                var res = CommonDbLogic.GetCompanyProfile(ct[0].EventId, ct[0].BusinessOwner_Id);
                                ViewBag.URL = res.website;
                                ViewBag.Logo = res.Image;

                            }
                        }
                        else
                        {
                            TempData["Error"] = "User does not exist!";
                            return RedirectToAction("MyTickets", "Home", new { evId = evId, comId = comId });
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "User does not exist!";
                    return RedirectToAction("MyTickets", "Home", new { evId= evId, comId = comId });
                }


            }
            catch (Exception ex)
            {
                return RedirectToAction("MyTickets", "Home", new { evId = evId, comId = comId });
            }
            return View(ct);
        }

        //[HttpPost]
        //public async Task<JsonResult> SendEmail(string[] ticketlist, int userid)
        //{
        //    EventmanagerEntities dbConn = new EventmanagerEntities();
        //    int message = 0;
        //    if (ticketlist != null)
        //    {
        //        try
        //        {
        //            foreach (var tickets in ticketlist)
        //            {
        //                var tick = tickets.Split(',');
        //                int ticket_id = (int)Convert.ToInt64(tick[0]);
        //                DateTime ticket_date = Convert.ToDateTime(tick[1]);
        //                var tiketdetail = dbConn.TickeUserMaps.Where(x => x.Id == ticket_id).FirstOrDefault();
        //                var objTicket = dbConn.EventTickets.Where(a => a.Id == tiketdetail.TicketId).FirstOrDefault();
        //                string eventname = objTicket.Event.Event_name;
        //                var request = HttpContext.Request;
        //                //var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
        //                var address = ConfigurationManager.AppSettings["ReturnURL"];
        //                string rootpath = Server.MapPath("/Views/EmailTemplate/");
        //                var Users = dbConn.Users.Where(x => x.Id == tiketdetail.UserId).FirstOrDefault();
        //                Log4Net.Error("GetSendTicketToUser:" + "BarCodeNumber:- " + tiketdetail.BarCodeNumber + "firstname: -" + Users.FirstName + "EmailId :-" + Users.Email
        //                 + "request: " + request.ToString() + "address: " + address + "rootpath :" + rootpath + "TicketMapId :" + tickets + "TicketId :" + tiketdetail.TicketId);
        //                await PaymentLogic.GenerateBarcodeandsendTicke(tiketdetail.BarCodeNumber, Users.FirstName, Users.Email, request.ToString(), address, rootpath, tiketdetail.Id, tiketdetail.TicketId, ticket_date);

        //                tiketdetail.IsTicketSendToUser = true;
        //                dbConn.SaveChanges();

        //            }
        //        }

        //        catch
        //        {
        //            return Json(new { code = 0, message = "Failed..", JsonRequestBehavior.AllowGet });
        //        }
        //    }
        //    return Json(new { code = 1, message = "Tickets sent..", JsonRequestBehavior.AllowGet });
        //}

        [HttpPost]
        public async Task<JsonResult> SendEmail(string[] ticketlist, int userid)
        {
            EventmanagerEntities dbConn = new EventmanagerEntities();
            int message = 0;
            if (ticketlist != null)
            {
                try
                {
                    foreach (var tickets in ticketlist)
                    {
                        var tick = tickets.Split(',');
                        int ticket_id = (int)Convert.ToInt64(tick[0]);
                        DateTime ticket_date = Convert.ToDateTime(tick[1]);

                        var tiketdetail = dbConn.TickeUserMaps.Where(x => x.Id == ticket_id).FirstOrDefault();
                        var objTicket = dbConn.EventTickets.Where(a => a.Id == tiketdetail.TicketId).FirstOrDefault();
                        string eventname = objTicket.Event.Event_name;
                        var request = HttpContext.Request;
                        //var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
                        var address = ConfigurationManager.AppSettings["ReturnURL"];
                        string rootpath = Server.MapPath("/Views/EmailTemplate/");
                        var Users = dbConn.Users.Where(x => x.Id == tiketdetail.UserId).FirstOrDefault();
                        Log4Net.Error("GetSendTicketToUser:" + "BarCodeNumber:- " + tiketdetail.BarCodeNumber + "firstname: -" + Users.FirstName + "EmailId :-" + Users.Email
                         + "request: " + request.ToString() + "address: " + address + "rootpath :" + rootpath + "TicketMapId :" + tickets + "TicketId :" + tiketdetail.TicketId);
                        await PaymentLogic.GenerateBarcodeandsendTicke(tiketdetail.BarCodeNumber, Users.FirstName, Users.Email, request.ToString(), address, rootpath, tiketdetail.Id, tiketdetail.TicketId, ticket_date);

                        tiketdetail.IsTicketSendToUser = true;
                        dbConn.SaveChanges();

                    }
                }
                catch
                {
                    message = 0;
                    return Json(message, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                message = 1;
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            message = 2;
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTicket(int EventId, string Name)
        {
            string p = "";
            try
            {
                TicketModelPopup result = DbLogic.GetTicket(EventId);
                if (Name != null && Name != "undefined")
                {
                    var tic = result.Tickest.Where(a => a.TicketName == Name).ToList();
                    result.Tickest = tic;
                }
                return PartialView("_partialTicket", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_Events");
        }

        [HttpPost]
        public ActionResult PostTicket(int EventId, string txtname, string txtEmail, string txtphone)
        {
            try
            {
                Log4Net.Error("PostTicket page  called" + EventId.ToString() + "name " + txtname);
                if (!string.IsNullOrEmpty(txtname) && !string.IsNullOrEmpty(txtEmail) && !string.IsNullOrEmpty(txtphone))
                {
                    Log4Net.Error("PostTicket page  called");
                    if (ManageSession.TicketCartSession != null && ManageSession.TicketCartSession.TickeCarts != null)
                    {
                        ManageSession.TicketCartSession.EventId = EventId;
                        ManageSession.TicketCartSession.name = txtname;
                        ManageSession.TicketCartSession.email = txtEmail;
                        ManageSession.TicketCartSession.phone = txtphone;
                        return RedirectToAction("Summery", "Payment", new { EventId = EventId, Area = "" });
                    }
                    else
                    {
                        return RedirectToAction("EventDetail", "Home", new { Id = EventId });
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Net.Error("PostTicket exception  called");
            }
            return RedirectToAction("EventDetail", "Home", new { Id = EventId });
        }
        [HttpPost]
        public ActionResult PostTicketnew(string EventIds, string tickettype, string price)
        {
            int tickettypee = Convert.ToInt32(tickettype);
            int EventId = Convert.ToInt32(EventIds);
            try
            {
                Log4Net.Error("PostTicket page  called" + EventId.ToString() + "name " + ManageSession.UserSession.FirstName);
                if (!string.IsNullOrEmpty(EventIds) && !string.IsNullOrEmpty(tickettype))
                {
                    Log4Net.Error("PostTicket page  called");
                    if (ManageSession.TicketCartSession != null && ManageSession.TicketCartSession.TickeCarts != null)
                    {
                        ManageSession.TicketCartSession.EventId = EventId;
                        ManageSession.TicketCartSession.name = ManageSession.UserSession.FirstName;
                        ManageSession.TicketCartSession.email = ManageSession.UserSession.EmailId;
                        ManageSession.TicketCartSession.phone = ManageSession.UserSession.PhoneNo;
                        //var a = AddTicketToSession(tickettypee, 1, Convert.ToInt32(Convert.ToDecimal(price)));
                        return Json(true);
                    }
                    else if (tickettypee > 0)
                    {
                        //var a = AddTicketToSession(tickettypee, 1, Convert.ToInt32(Convert.ToDecimal(price)));
                        ManageSession.TicketCartSession.EventId = EventId;
                        ManageSession.TicketCartSession.name = ManageSession.UserSession.FirstName;
                        ManageSession.TicketCartSession.email = ManageSession.UserSession.EmailId;
                        ManageSession.TicketCartSession.phone = ManageSession.UserSession.PhoneNo;
                        return Json(true);
                    }

                }
            }
            catch (Exception ex)
            {
                Log4Net.Error("PostTicket exception  called");
            }
            return Json(true);
        }

        public int CheckTicketBalance(int EventId = 0, int TicketId = 0, int SelectedTickets = 0)
        {
            TicketModelPopup result = new TicketModelPopup();
            int count = 0;
            int cart = 0;
            int AvailableBalance = 0;
            try
            {
                if (TicketId > 0 && EventId > 0)
                {
                    if (ManageSession.TicketCartSession != null && ManageSession.TicketCartSession.TickeCarts != null)
                    {
                        foreach (var item in ManageSession.TicketCartSession.TickeCarts)
                        {
                            if (item.TicketId == TicketId)
                            {
                                cart = item.Qnty;
                            }
                        }
                    }
                    result = DbLogic.GetTicketNew(EventId, TicketId);
                    if (result != null)
                    {
                        foreach (var item in result.Tickest)
                        {
                            AvailableBalance = (int)item.AvailableQnty;
                        }
                    }

                    count = AvailableBalance - (SelectedTickets + cart);
                }

            }
            catch (Exception ex)
            {


            }
            return count;
        }
        public ActionResult AddTicketToSession(int TicketId, int Qty, string Price, int paymentType, string TicketName, int TotalTicket, int evid = 0, int eventype = 0)
        {
            string Message = "";
            int result = 0;
            try
            {
                result = CheckTicketBalance(evid, TicketId, Qty);
                if (result > 0)
                {
                    if (ManageSession.TicketCartSession != null)
                    {
                        if (ManageSession.TicketCartSession.Eventtype == eventype && ManageSession.TicketCartSession.EventId == evid)
                        {
                            if (ManageSession.TicketCartSession.TickeCarts.Count() > 0)
                            {
                                if (ManageSession.TicketCartSession.TickeCarts.Where(x => x.currencyType != paymentType).ToList().Count() > 0)
                                {
                                    Message = "Your cart can only contain tickets with the same currency for each purchase.";
                                    // return new JsonResult { Data = "Please select same currency, There are ticket with different currency in your cart." };
                                    //ViewBag.Response = "Please select same currency, There are ticket with different currency in your cart.";
                                }
                                else
                                {
                                    if (ManageSession.TicketCartSession.TickeCarts.Where(x => x.TicketId == TicketId).FirstOrDefault() != null)
                                    {
                                        var currentTicket = ManageSession.TicketCartSession.TickeCarts.Where(x => x.TicketId == TicketId).FirstOrDefault();
                                        ManageSession.TicketCartSession.TickeCarts.Remove(currentTicket);
                                        TickeCart ticket = new TickeCart();
                                        ticket.TicketId = TicketId;
                                        ticket.Qnty = Qty + currentTicket.Qnty;
                                        ticket.Price = Convert.ToDecimal(Price);
                                        ticket.currencyType = paymentType;
                                        ticket.TicketAdded = true;
                                        ticket.TicketName = TicketName;
                                        ticket.TotalTicket = TotalTicket;

                                        ManageSession.TicketCartSession.TickeCarts.Add(ticket);
                                    }
                                    else
                                    {
                                        TickeCart ticket = new TickeCart();
                                        ticket.TicketId = TicketId;
                                        ticket.Qnty = Qty;
                                        ticket.Price = Convert.ToDecimal(Price);
                                        ticket.currencyType = paymentType;
                                        ticket.TicketAdded = true;
                                        ticket.TicketName = TicketName;
                                        ticket.TotalTicket = TotalTicket;
                                        ManageSession.TicketCartSession.TickeCarts.Add(ticket);
                                    }

                                }
                            }
                            else
                            {
                                ManageSession.TicketCartSession.TickeCarts = new List<TickeCart>();
                                TickeCart ticket = new TickeCart();
                                ticket.TicketId = TicketId;
                                ticket.Qnty = Qty;
                                ticket.Price = Convert.ToDecimal(Price);
                                ticket.currencyType = paymentType;
                                ticket.TicketAdded = true;
                                ticket.TicketName = TicketName;
                                ticket.TotalTicket = TotalTicket;
                                ManageSession.TicketCartSession.TickeCarts.Add(ticket);
                            }

                        }
                        else
                        {
                            ManageSession.TicketCartSession = new TicketCartSession();
                            ManageSession.TicketCartSession.Eventtype = eventype;
                            ManageSession.TicketCartSession.EventId = evid;
                            ManageSession.TicketCartSession.TickeCarts = new List<TickeCart>();
                            TickeCart ticket = new TickeCart();
                            ticket.TicketId = TicketId;
                            ticket.Qnty = Qty;
                            ticket.Price = Convert.ToDecimal(Price);
                            ticket.currencyType = paymentType;
                            ticket.TicketAdded = true;
                            ticket.TicketName = TicketName;
                            ticket.TotalTicket = TotalTicket;
                            ManageSession.TicketCartSession.TickeCarts.Add(ticket);
                        }
                    }
                    else
                    {
                        ManageSession.TicketCartSession = new TicketCartSession();
                        ManageSession.TicketCartSession.Eventtype = eventype;
                        ManageSession.TicketCartSession.EventId = evid;
                        ManageSession.TicketCartSession.TickeCarts = new List<TickeCart>();
                        TickeCart ticket = new TickeCart();
                        ticket.TicketId = TicketId;
                        ticket.Qnty = Qty;
                        ticket.Price = Convert.ToDecimal(Price);
                        ticket.currencyType = paymentType;
                        ticket.TicketAdded = true;
                        ticket.TicketName = TicketName;
                        ticket.TotalTicket = TotalTicket;
                        ManageSession.TicketCartSession.TickeCarts.Add(ticket);
                    }
                }
                else
                {
                    Message = "No more Tickets are available..";
                }
            }
            catch (Exception ex)
            {
                Message = "Something went wrong..";

            }
            return new JsonResult { Data = Message };
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AllEvents(int skip, int take)
        {
            try
            {
                List<IndexView> result = DbLogic.GetIndexData(skip, take);
                return PartialView("_AllEvents", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_AllEvents");
        }

        public ActionResult GetEvents(int skip, int take)
        {
            try
            {
                List<IndexView> result = DbLogic.GetIndexData(skip, take, 1);
                return PartialView("_Events", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_Events");
        }
        public ActionResult MyVideo()
        {
            if (ManageSession.UserSession != null && !string.IsNullOrEmpty(ManageSession.UserSession.PhoneNo))
            {
                List<Event_> ct = new List<Event_>();
                try
                {
                    List<TicketTypemodel> tType = cdb.GetticketTypes();
                    var act = hdb.GetUserTickets(ManageSession.UserSession.Id);
                    ct = act.Where(x => Convert.ToDateTime(x.EndDate) >= DateTime.UtcNow).ToList();
                    return PartialView("_MyVideo", ct);
                }
                catch (Exception ex) { RedirectToAction("Login", "Account", new { area = "" }); }
            }
            return PartialView("_MyVideo");
        }
        public ActionResult LiveEvents(int skip, int take, int catg = 0)
        {
            try
            {

                List<IndexView> result = null; //DbLogic.GetLiveEvent(skip, take, catg);
                return PartialView("_AllEvents", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_Events");
        }
        public ActionResult GeteEvents(int skip, int take)
        {
            try
            {
                List<IndexView> result = DbLogic.GetIndexeEvent(skip, take, 1);
                return PartialView("_Events", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_Events");
        }
        public ActionResult EventsSearchResult(string eventname = null, string place = null, string Eventday = null, int eventtype = 0)
        {
            List<IndexView> result = DbLogic.GetSearchEventData(eventname, place, Eventday, eventtype);
            return View(result);
        }
        public ActionResult Events()
        {
            HandleBanner br = new HandleBanner();
            var Banner = br.getBannerList();
            ViewBag.BannerList = Banner;
            //List<IndexView> result = DbLogic.GetSearchEventData(eventname, place, Eventday, eventtype);
            List<IndexView> result = DbLogic.GetIndexeEvent(0, 0);
            return View(result);
        }
        public ActionResult Videos()
        {
            HandleBanner br = new HandleBanner();
            var Banner = br.getBannerList();
            ViewBag.BannerList = Banner;
            List<IndexView> result = DbLogic.GetVideoEvent();
            return View(result);
        }
        public ActionResult LiveStream()
        {

            HandleBanner br = new HandleBanner();
            var Banner = br.getBannerList();
            ViewBag.BannerList = Banner;
            List<IndexView> result = DbLogic.GetLiveEvent();
            return View(result);
        }
        //public ActionResult PaymentSupport()
        //{
        //    PaymentSupportModel p = new PaymentSupportModel();
        //    return View(p);
        //}
        //[HttpPost]
        //public ActionResult PaymentSupport(PaymentSupportModel model)
        //{
        //    if (string.IsNullOrEmpty(model.TrxId)) { model.TrxId = "xxxxxxx"; }
        //    if (ModelState.IsValid)
        //    {
        //        ApiResponse res = new ApiResponse();
        //        res = DbLogic.submitPaymentSupport(model);
        //        if (res.Code == (int)ApiResponseCode.ok)
        //        {
        //            ModelState.Clear();
        //        }
        //        ViewData["Error"] = res;
        //    }
        //    return View();
        //}
        //public ActionResult FAQ()
        //{
        //    return View();
        //}

        public async Task<ActionResult> SuccessPaymentViewPage(string Message = "", string TransId = "")
        {
            try
            {
                if (Message != null && TransId != null)
                {
                    if (Message == "success")
                    {
                        ViewBag.Response = "Okay";
                        ViewBag.TransactionId = TransId;

                        //send email 


                        var checkPaymenttransation = db.Payments.Where(x => x.TransactionId == TransId).FirstOrDefault();

                        if (checkPaymenttransation != null)
                        {
                            var act = hdb.GetPurchasedTicket(checkPaymenttransation.UserId, checkPaymenttransation.Id);
                            foreach (var tickets in act)
                            {
                                var UpdateTicket = db.TickeUserMaps.Where(x => x.Id == tickets.TicketmapId).FirstOrDefault();
                                if (UpdateTicket.IsTicketSendToUser == false)
                                {
                                    var objTicket = db.EventTickets.Where(a => a.Id == tickets.TicketId).FirstOrDefault();
                                    string eventname = objTicket.Event.Event_name;
                                    var request = HttpContext.Request;
                                    //var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
                                    //var address = "https://vsprocessorpro.com"; 
                                    var address = ConfigurationManager.AppSettings["ReturnURL"];


                                    string rootpath = Server.MapPath("/Views/EmailTemplate/");
                                    var Users = db.Users.Where(x => x.Id == tickets.UserId).FirstOrDefault();
                                    Log4Net.Error("Ticket details to be sent to user: " + "BarCodeNumber: " + tickets.Barcode + " firstname:  " + Users.FirstName + " EmailId : " + Users.Email
                                     + " request: " + request.ToString() + " address: " + address + " rootpath : " + rootpath + " TicketMapId : " + tickets.TicketmapId + " TicketId : " + tickets.TicketId);
                                    await PaymentLogic.GenerateBarcodeandsendTicke(tickets.Barcode, Users.FirstName, Users.Email, request.ToString(), address, rootpath, tickets.TicketmapId, tickets.TicketId, tickets.ticketdate);
                                    var compid = objTicket.Event.Company_Id;
                                    var res = CommonDbLogic.GetCompanyProfile((int)objTicket.Event_Id, (int)compid);
                                    ViewBag.URL = res.website;
                                }

                                UpdateTicket.IsTicketSendToUser = true;
                                //UpdateTicket.BarCodeNumber = tickets.Barcode;
                                UpdateTicket.Status = 1;
                                db.SaveChanges();

                            }
                        }
                    }
                    else
                    {
                        ViewBag.Response = "Fail";
                        ViewBag.TransactionId = TransId;
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Net.Error("GetSendTicketToUser Exception:" + ex.Message);
            }
            return View();
        }
        public ActionResult ReissueTicket(string EmailId = "", string TransId = "")
        {
            ViewBag.EmailId = EmailId;
            ViewBag.TransId = TransId;
            return View();
        }
        public ActionResult ReIssueTicketsLists(string EmailId = "", string Url = "", string Logo = "")
        {
            ViewBag.EmailId = EmailId;
            ViewBag.Url = Url;
            ViewBag.Logo = Logo;
            return View();
        }
        [HttpPost]
        public ActionResult ListReIssueTicket(string EmailId = "", string Url = "")
        {
            string UrlPath = Url;
            EventmanagerEntities db = new EventmanagerEntities();
            List<ReissueFailedpaymentmodel> list = new List<ReissueFailedpaymentmodel>();
            try
            {
                var users = db.Users.Where(m => m.Email == EmailId).FirstOrDefault();
                if (users != null)
                {
                    var tick = db.TickeUserMaps.Include("Payment").Where(x => x.UserId == users.Id && x.Payment.Status != (int)PaymentStatus.PaymentSuccess && x.Qty > 0).OrderByDescending(a => a.CreateDate).ToList();
                    if (tick != null && tick.Count > 0)
                    {
                        var res = PaymentLogic.getReissueFailedpayment(users.Id, (int)tick[0].EventTicket.Event_Id);
                        return PartialView("_ReissueTicket", res);

                    }
                    else
                    {
                        return PartialView("_ReissueTicket", list);
                    }
                }
                else
                {
                    return PartialView("_ReissueTicket", list);
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction(UrlPath);
                //return RedirectToAction("MyTicket", "Home", new { area = "" });
            }
        }

        public dynamic ChecktrxStatusUser(string PaymentId, string TrxNo, string paymentfor, string PayGateway, int EventId)
        {
            StatusResponse result = new StatusResponse();
            if (PayGateway == PaymentGatewayName.PayStack.ToString())
            {
                result = HandlePayment.PayStackstatus(TrxNo);
            }
            else if (PayGateway == PaymentGatewayName.Stripe.ToString())
            {
                result = HandlePayment.Stripestatus(TrxNo);
            }
            if (result.status == "Success")
            {
                result.status = "SUCCESSFUL";
                if (paymentfor == "Invitation")
                {
                    result.message = "Invitation sent successfully.";
                    result.Page = "Invitation";
                }
                else
                {
                    result.Page = "Ticket";
                    result.message = "Ticket sent successfully.";
                    var resp = PaymentLogic.Addtowallet(Convert.ToInt32(PaymentId), EventId);
                }
                var updatepay = PaymentLogic.UpdatePaymentStatus(Convert.ToInt32(PaymentId), result.status, result.financialTransactionId);
                if (updatepay == true)
                {
                    return RedirectToAction("ReissueSendEmail", "Home", new { TransId = TrxNo });
                }
            }
            return PartialView("_FailedPayStatusInUser", result);
        }
        public async Task ReissueSendEmail(string TransId = "")
        {
            var checkPaymenttransation = db.Payments.Where(x => x.TransactionId == TransId).FirstOrDefault();

            if (checkPaymenttransation != null)
            {
                var act = hdb.GetPurchasedTicket(ManageSession.UserSession.Id, checkPaymenttransation.Id);
                foreach (var tickets in act)
                {
                    var UpdateTicket = db.TickeUserMaps.Where(x => x.Id == tickets.TicketmapId).FirstOrDefault();
                    if (UpdateTicket.IsTicketSendToUser == false)
                    {
                        var objTicket = db.EventTickets.Where(a => a.Id == tickets.TicketId).FirstOrDefault();
                        string eventname = objTicket.Event.Event_name;
                        var request = HttpContext.Request;
                        //var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
                        var address = ConfigurationManager.AppSettings["ReturnURL"];
                        string rootpath = Server.MapPath("/Views/EmailTemplate/");
                        var Users = db.Users.Where(x => x.Id == tickets.UserId).FirstOrDefault();
                        Log4Net.Error("GetSendTicketToUser:" + "BarCodeNumber:- " + tickets.Barcode + "firstname: -" + Users.FirstName + "EmailId :-" + Users.Email
                                             + "request: " + request.ToString() + "address: " + address + "rootpath :" + rootpath + "TicketMapId :" + tickets.TicketmapId + "TicketId :" + tickets.TicketId);
                        await PaymentLogic.GenerateBarcodeandsendTicke(tickets.Barcode, Users.FirstName, Users.Email, request.ToString(), address, rootpath, tickets.TicketmapId, tickets.TicketId, tickets.ticketdate);
                    }

                    UpdateTicket.IsTicketSendToUser = true;
                    //UpdateTicket.BarCodeNumber = tickets.Barcode;
                    UpdateTicket.Status = 1;
                    db.SaveChanges();

                }
            }
        }
    }
}