using EventManager1.DBCon;
using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventManager1.Controllers
{
    public class evController : Controller
    {
        // GET: ev
        HandleEvent hdb = new HandleEvent();
        public ActionResult Summary(int Id = 0)
        {
            HandleUser hd = new HandleUser();
            if (Id <= 0) { return RedirectToAction("Index", "GrabTicket", new { tid = Id }); }
            if (ManageSession.UserSession != null)
            {
                string URL = "";
                if (Id > 0)
                {
                    HandleEvent ev = new HandleEvent();
                    var tickets = ev.GetUserTickets(ManageSession.UserSession.Id);
                    Event_ ticket = new Event_();
                    foreach (var i in tickets)
                    {
                        if (i.Tickets.FirstOrDefault(y => y.Id == Id) != null)
                        {
                            ticket = i;
                        }
                    }
                    // var ticket = tickets.FirstOrDefault(x => x.Tickets.FirstOrDefault(y => y.Id == tid).Id == tid);
                    if (ticket != null && ticket.EventName != null)
                    {
                        if (ticket.Eventtype == 1 && ticket.paymentStatus == 1)
                        {
                            URL = "/User/Events/Index?v=" + ticket.Multimedia.FirstOrDefault().videoId;
                        }
                        else if (ticket.Eventtype == 2 && ticket.paymentStatus == 1)
                        {

                            URL = "/OnlineStreaming/Index?ev=" + ticket.evid;
                        }
                        else
                        {
                            URL = "/User/User/Tickets?id=" + ticket.Tickets.FirstOrDefault().TmapID + "&tmap=" + ticket.Tickets.FirstOrDefault().TmapID;
                            //URL = "/user/user/dashboard";
                        }
                    }

                }
                if (URL != "")
                {
                    return Redirect(URL);
                }

                return RedirectToAction("Index", "GrabTicket", new { tid = Id });
                //var OrganizerId= hd.OrganizerExist(Id);
                //if (OrganizerId>0)
                //{
                //    var UserExit = hd.UserExit(ManageSession.UserSession.EmailId, ManageSession.UserSession.PhoneNo);
                //    if (UserExit == false)
                //    {
                //        return RedirectToAction("UserRegister", "Account", new { returnUrl = Id.ToString() });
                //    }
                //}

                ViewBag.evId = Id;
                TicketModelPopup result = new TicketModelPopup();
                result = DbLogic.GetTicket(Id);
                var tic = result.Tickest.ToList();
                result.Tickest = tic;
                var act = hdb.GetUserTickets(ManageSession.UserSession.Id);
                foreach (var item in act)
                {
                    //if (item.paymentStatus == 1)
                    //{
                    //var validDay = item.Tickets.FirstOrDefault().validdays;
                    //DateTime validDate = item.purchaseDate.AddHours(24 * validDay);
                    //DateTime CurrentDate = DateTime.UtcNow;
                    //if (CurrentDate > item.purchaseDate.AddHours(0) && CurrentDate <= validDate)
                    //{
                    if (item.EventId == Id)
                    {
                        if (item.Eventtype == 2)
                        {
                            if (item.streamType == 2)
                            {
                                string evid = EnryptString(item.EventId.ToString());
                                //return Redirect("http://stream.stream233.com/?ev=" + item.evid);
                                return Redirect("/OnlineStreaming/Index/?ev=" + item.evid);
                            }
                        }
                        else
                        {
                            //return RedirectToAction("Userlogin", "Events", new { area= "User" });
                            return Redirect("/User/Events/Index?v=" + item.Multimedia.FirstOrDefault().videoId);
                        }
                    }
                    //}
                    //}
                }
                return View(result);
            }
            else
            {
                return RedirectToAction("Userlogin", "Account", new { returnUrl = Id.ToString() });
            }


        }
        public string EnryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }
        public ActionResult GetSummary(int Id = 102)
        {
            HandleUser hd = new HandleUser();
            ViewBag.evId = Id;
            TicketModelPopup result = new TicketModelPopup();
            result = DbLogic.GetTicket(Id);
            var tic = result.Tickest.ToList();
            result.Tickest = tic;
            var act = hdb.GetUserTickets(ManageSession.UserSession.Id);
            foreach (var item in act)
            {
                if (item.EventId == Id)
                {
                    if (item.Eventtype == 2)
                    {
                        if (item.streamType == 2)
                        {
                            string evid = EnryptString(item.EventId.ToString());
                            // return Redirect("/OnlineStreaming/Index/?ev=" + item.evid);
                            result.LivestreamTiccket = "/OnlineStreaming/Index/?ev=" + item.evid;
                        }
                    }
                    else
                    {
                        result.VideoTicket = "/User/Events/Index?v=" + item.Multimedia.FirstOrDefault().videoId;
                        //return Redirect("/User/Events/Index?v=" + item.Multimedia.FirstOrDefault().videoId);
                    }
                }
            }
            return PartialView("_SummaryPartial", result);
        }

        public ActionResult PaymentMode(int hfeventid = 0, int UserID = 0)
        {
            try
            {

                DbLogic.InsertSessions(UserID);

                if (ManageSession.UserSession != null)
                {
                    if (ManageSession.TicketCartSession != null && ManageSession.TicketCartSession.TickeCarts != null)
                    {
                        bool dbopetaion = DbLogic.RemoveNotAddedSessions();
                        if (dbopetaion)
                        {
                            bool Isinserted = false;
                            foreach (var items in ManageSession.TicketCartSession.TickeCarts)
                            {
                                if (Isinserted == false)
                                {
                                    ViewBag.paymentType = items.currencyType;
                                    Isinserted = true;
                                }

                                bool result = DbLogic.CheckTicketAvailable();
                                if (result == false)
                                {
                                    return RedirectToAction("NewSummary", "ev", new { id = hfeventid });
                                }
                                //ViewBag.StripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];
                                if (hfeventid > 0)
                                {
                                    ViewBag.evId = hfeventid;
                                    ViewBag.Userid = ManageSession.UserSession;
                                }

                                if (items.currencyType == (int)PaymentCurrency.Ghanna)
                                {
                                    return RedirectToAction("PayStackPayment", "Payment", new { hfeventid = hfeventid });
                                }
                                else
                                {
                                    return RedirectToAction("Checkout", "Payment", new { hfeventid = hfeventid });
                                }

                            }
                        }
                    }

                }
                
            }
            catch (Exception ex)
            {
                return RedirectToAction("NewSummary", "ev", new { id = hfeventid });
            }
            //return RedirectToAction("NewSummary", "ev", new { id = hfeventid });
            return View();

        }
        //public ActionResult NewSummary(int Id = 0, string p = "", int tid = 0)

        //{
        //    TicketModelPopup result = new TicketModelPopup();
        //    try
        //    {
        //        if (p != null)
        //        {
        //            HandleUser hd = new HandleUser();
        //            if (Id <= 0) { return RedirectToAction("Index", "Home"); }
        //            ManageSession.TicketCartSession = null;
        //            if (ManageSession.UserSession == null)
        //            {
        //                ViewBag.evId = Id;
        //                result = DbLogic.GetTicketNew(Id, p, tid);
        //                var tic = result.Tickest.ToList();
        //                result.Tickest = tic;
        //            }
        //            else
        //            {
        //              var re=  ManageSession.UserSession == null;
        //                ViewBag.evId = Id;
        //                result = DbLogic.GetTicketNew(Id, p, tid);
        //                var tic = result.Tickest.ToList();
        //                result.Tickest = tic;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return View(result);
        //}

        //public ActionResult NewSummary(int Id = 0, int comid =0)
        //{
        //    TicketModelPopup result = new TicketModelPopup();
        //    try
        //    {
        //        HandleUser hd = new HandleUser();
        //        if (Id <= 0) { return RedirectToAction("Index", "Home"); }
        //        ManageSession.TicketCartSession = null;
        //        if (ManageSession.UserSession != null)
        //        {
        //            ManageSession.UserSession = null;
        //        }
        //        if (ManageSession.UserSession == null)
        //        {
        //            ViewBag.evId = Id;
        //            var eventDetail = DbLogic.GetEventDetail(Id);
        //            ViewBag.eventType = eventDetail.EventType;
        //            result = DbLogic.GetTicket(Id);
        //            var tic = result.Tickest.ToList();
        //            result.Tickest = tic;
        //            var res = CommonDbLogic.GetCompanyProfile(Id,comid);
        //            ViewBag.logo = res.Image;
        //        }
        //        //else
        //        //{
        //        //    return RedirectToAction("Login", "Account", new { returnUrl = Id.ToString() });
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return View(result);
        //}
        public ActionResult NewSummary(int Id = 0, string p = "", int tid = 0, int comid = 0)

        {
            TicketModelPopup result = new TicketModelPopup();
            try
            {
                if (p != null)
                {
                    HandleUser hd = new HandleUser();
                    if (Id <= 0) { return RedirectToAction("Index", "Home"); }
                    ManageSession.TicketCartSession = null;
                    if (ManageSession.UserSession == null)
                    {
                        ViewBag.evId = Id;
                        result = DbLogic.GetTicketNew(Id, p, tid);
                        var tic = result.Tickest.ToList();
                        result.Tickest = tic;
                    }
                    else
                    {
                        var re = ManageSession.UserSession == null;
                        ViewBag.evId = Id;
                        result = DbLogic.GetTicketNew(Id, p, tid);
                        var tic = result.Tickest.ToList();
                        result.Tickest = tic;
                    }
                    var res = CommonDbLogic.GetCompanyProfile(Id, comid);
                    if (res.Image != null)
                    {
                        ViewBag.logo = res.Image;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return View(result);
        }
        //public ActionResult GetTicketDetails(int EventId = 0, int TicketId = 0)
        //{
        //    TicketModelPopup result = new TicketModelPopup();
        //    try
        //    {
        //        if (EventId > 0 && TicketId > 0)
        //        {
        //            ViewBag.evId = EventId;
        //            result = DbLogic.GetTicketNew(EventId, TicketId);
        //            var tic = result.Tickest.ToList();
        //            result.Tickest = tic;

        //           // result = DbLogic.GetTicket(EventId);
        //            if (result != null && result.Tickest != null && result.Tickest.Count() > 0)
        //            {
        //                result.Company = HandleEvent.getcompany(EventId);
        //                ManageSession.CompanySession = new CompanySession();
        //                ManageSession.CompanySession.CompName = result.Company.Name;
        //                ManageSession.CompanySession.CompanyId = result.Company.Id;
        //                ManageSession.CompanySession.FirstName = result.Company.Logo;
        //                ManageSession.CompanySession.website = result.Company.website;
        //            }
        //            else
        //            {
        //                ViewData["error"] = "this event don't have ticket or ticket is disabled please check";

        //            }
        //            if (ManageSession.TicketCartSession != null && ManageSession.TicketCartSession.TickeCarts != null)
        //            {
        //                //if ()
        //                //    ManageSession.TicketCartSession.TickeCarts.Clear();

        //            }

        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //    return PartialView("_GetTicketDetails", result);
        //}

        public ActionResult GetTicketDetails(int EventId = 0, int TicketId = 0)
        {
            TicketModelPopup result = new TicketModelPopup();
            try
            {
                if (EventId > 0 && TicketId > 0)
                {
                    ViewBag.evId = EventId;
                    result = DbLogic.GetTicketNew(EventId, TicketId);
                    var tic = result.Tickest.ToList();
                    result.Tickest = tic;

                }
            }
            catch (Exception)
            {

                throw;
            }

            return PartialView("_GetTicketDetails", result);
        }

        //public ActionResult AddTickets()
        //{
        //    List<TickeCart> viewModel = new List<TickeCart>();

        //    try
        //    {
        //        if (ManageSession.TicketCartSession != null && ManageSession.TicketCartSession.TickeCarts != null)
        //        {
        //            bool paymenType = false;
        //            int CurrenctType = 0;


        //            foreach (var ticket in ManageSession.TicketCartSession.TickeCarts)
        //            {
        //                if (ticket.TotalTicket >= ticket.Qnty)
        //                {
        //                    if (paymenType == false)
        //                    {
        //                        CurrenctType = ticket.currencyType;
        //                        paymenType = true;
        //                    }
        //                    if (CurrenctType == ticket.currencyType)
        //                    {
        //                        ticket.TicketAdded = true;
        //                        viewModel.Add(ticket);
        //                    }
        //                    else
        //                    {
        //                        ManageSession.TicketCartSession.TickeCarts.Remove(ticket);
        //                        ViewBag.Response = "Please select same currenies tickets only!!";

        //                    }
        //                }
        //                else
        //                {
        //                    ManageSession.TicketCartSession.TickeCarts.Remove(ticket);
        //                    ViewBag.Response = "Selected ticket is not available! Ticket availability count is" + ticket.TotalTicket;
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return PartialView("_AddTickets", viewModel);
        //}

        public ActionResult AddTickets(int evID = 0, string Coupon = "")
        {
            List<TickeCart> viewModel = new List<TickeCart>();
            TickeCart cart = new TickeCart();
            decimal totalPrice = 0;
            decimal discount = 0;
            bool isCouponApplied = false;
            if (evID > 0)
            {
                ViewBag.evid = evID;
            }
            try
            {
                if (ManageSession.TicketCartSession != null)
                {
                    viewModel.AddRange(ManageSession.TicketCartSession.TickeCarts);
                    var OferTicket = ManageSession.TicketCartSession.TickeCarts.Where(x => x.OfferId > 0).FirstOrDefault();
                    totalPrice = ManageSession.TicketCartSession.TickeCarts.ToList().Sum(x => x.Price.Value * x.Qnty);

                    if (OferTicket != null)
                    {
                        using (EventmanagerEntities dbConn = new EventmanagerEntities())
                        {
                            var offer = dbConn.Offers.Where(x => x.Id == OferTicket.OfferId).FirstOrDefault();
                            if (offer.OfferType == (int)ApplicationEnum.Percentage)
                            {
                                discount = (totalPrice * offer.Value.Value) / 100;
                                ViewBag.Discount = offer.Value.Value + " %";
                            }
                            else
                            {
                                discount = offer.Value.Value;
                                ViewBag.Discount = offer.Value.Value;
                            }

                            isCouponApplied = true;
                        }
                    }
                }
                totalPrice = totalPrice - discount;
                ViewBag.TotalPrice = totalPrice;

                if (!string.IsNullOrEmpty(Coupon) && isCouponApplied)
                {
                    ViewBag.Couponapplied = Coupon;
                }
                if (!string.IsNullOrEmpty(Coupon))
                {
                    ViewBag.Couponselected = Coupon;
                }
            }
            catch (Exception ex)
            {
            }

            return PartialView("_AddTickets", viewModel);
        }

        public JsonResult RemoveTicket(int TicketId = 0)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (TicketId > 0)
                {
                    if (ManageSession.TicketCartSession != null && ManageSession.TicketCartSession.TickeCarts != null)
                    {
                        foreach (var item in ManageSession.TicketCartSession.TickeCarts)
                        {
                            if (item.TicketId == TicketId)
                            {
                                ManageSession.TicketCartSession.TickeCarts.Remove(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {


            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckEmailExists(string EmailId)
        {
            User_ user = new User_();
            try
            {
                if (EmailId != null)
                {
                    user = DbLogic.GetEmailResponse(EmailId);
                    
                }
                else
                {
                    RedirectToAction("NewSummary", "ev");
                }

            }
            catch (Exception ex)
            {

            }
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckAndCreateNewUserId(TicketModelPopup model)
        {
            UserSession response = new UserSession();
            try
            {
                if (ModelState.IsValid)
                {

                    response = DbLogic.InsertUserIdIntoSession(model);
                    if (response != null)
                    {
                        response.Code = (int)ApiResponseCode.ok;
                    }
                    else
                    {
                        response.Code = (int)ApiResponseCode.fail;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(response, JsonRequestBehavior.AllowGet);


        }
    }
}