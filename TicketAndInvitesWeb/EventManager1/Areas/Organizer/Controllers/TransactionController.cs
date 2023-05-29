using EventManager1.Areas.Organizer.Models;
using EventManager1.DBCon;
using EventManager1.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static EventManager1.MvcApplication;

namespace EventManager1.Areas.Organizer.Controllers
{
    [OrganizerSessionExpire]
    public class TransactionController : Controller
    {
        common cdb = new common();
        HandleEvent hdb = new HandleEvent();
        // GET: Organizer/Transaction
        public ActionResult History(int type = 0, string startdate = null, string enddate = null)
        {
            List<TransactionHistory> ct = hdb.GettransHistory(type, startdate, enddate);
            return View(ct);
        }
        public ActionResult Withdrawal()
        {
            return View();
        }
        public ActionResult Summery()
        {
            var op = TempData["op"] as OrganizerPay;
            if (op != null)
            {
                op.ImageUrl = hdb.Getimageurl(Convert.ToInt32(op.eventId));
                if (op.page == "Coupan")
                {

                }
                else { op.AdminFee = hdb.GetInvitaionfee(op.page); }

                TempData.Keep("op");
            }
            else { RedirectToAction("Dashboard", "Organizer", new { Area = "Organizer" }); }
            return View(op);
        }
        public bool broadcastEmail(string EventId, int paymId)
        {
            List<EmailList> emails = ManageSession.EmailDataSession;
            BroadcastEmails broadcost = TempData["broadcastList"] as BroadcastEmails;
            if (emails != null && broadcost != null && !string.IsNullOrEmpty(broadcost.Message) && !string.IsNullOrEmpty(broadcost.Subject))
            {
                int BMId = ModelInviation.savebroadcastmessage(Convert.ToInt32(EventId), broadcost);
                if (BMId > 0)
                {
                    foreach (var item in emails)
                    {
                        //var mail = ModelInviation.SendBroadcast(Convert.ToInt32(EventId), item.EmailAddress, broadcost);
                        var db = ModelInviation.savebroadcast(Convert.ToInt32(EventId), item.EmailAddress, item.Mobile, paymId, broadcost, BMId);
                        //send email here or add to queue list in database here
                    }
                }

            }
            else if (broadcost != null && broadcost.Emails != null && !string.IsNullOrEmpty(broadcost.Message) && !string.IsNullOrEmpty(broadcost.Subject))
            {
                int BMId = ModelInviation.savebroadcastmessage(Convert.ToInt32(EventId), broadcost);
                if (BMId > 0)
                {
                    foreach (var item in broadcost.Emails)
                    {
                        //var mail = ModelInviation.SendBroadcast(Convert.ToInt32(EventId), item.EmailAddress, broadcost);

                        var db = ModelInviation.savebroadcast(Convert.ToInt32(EventId), item.EmailAddress, item.Mobile, paymId, broadcost, BMId);
                        //send email here or add to queue list in database here
                    }
                }
            }
            ManageSession.EmailDataSession = null;
            return true;
        }
        public ActionResult SubscriptionSumemry(OrganizerPay op)
        {
            op.ImageUrl = hdb.Getimageurl(Convert.ToInt32(op.eventId));
            TempData["op"] = op;
            return View(op);
        }
        [HttpPost]
        public ActionResult summeryPay(int hfeventId, string mob, string total, string type, string Page, int InvType = 0)
        {
            OrganizerPay op = new OrganizerPay();
            if (TempData["op"] != null)
            {
                op = TempData["op"] as OrganizerPay;

                string TransId = Guid.NewGuid().ToString();
                int PayId = PaymentLogic.InsertPaymentWithPendingStatus(TransId, Page, Convert.ToDecimal(total), false, PaymentGatewayName.Mono_Payment.ToString());
                string clientsecret = "";// HandlePayment.Getclientsecret(TransId);
                decimal? TotalAmount = Convert.ToDecimal(total);
                PaymentModel pay = new PaymentModel();
                Payer p = new Payer();
                p.partyIdType = "MSISDN";
                if (mob.Length == 10)
                {
                    p.partyId = "233" + mob.Substring(1);
                }
                else if (mob.Length == 9)
                { p.partyId = "233" + mob; }
                //p.partyId = mob;// "46733123453"; // pay.payer.partyId;
                PaymentModel mpay = new PaymentModel();
                mpay.amount = (TotalAmount != null ? Convert.ToDecimal(TotalAmount) : 0).ToString();//pay.amount;
                mpay.currency = "GHS";// pay.currency;
                mpay.externalId = hfeventId.ToString();// pay.externalId;
                mpay.payer = p;
                mpay.payeeNote = Page;//pay.payeeNote;
                mpay.payerMessage = type;// pay.payerMessage;                      
                //StatusResponse status;//= CheckPayment_Status(TransId);    
                TempData["TransactionId"] = TransId;
                if (PayId > 0)
                {
                    try
                    {
                        HttpResponseMessage responses = HandlePayment.MakePaymentRequest(TransId, clientsecret, mpay);
                        TempData["TransactionId"] = TransId;
                        TempData["total"] = total;
                        if (responses.StatusCode.ToString() == "Accepted" || responses.StatusCode.ToString() == "Conflict")
                        {
                            PaymentStatuss paymodel = new PaymentStatuss();
                            //paymodel.clientsecret = clientsecret;
                            paymodel.eventId = hfeventId;
                            paymodel.Page = Page;
                            paymodel.PaymentId = PayId;
                            paymodel.substype = type;
                            paymodel.TransId = TransId;
                            TempData["paymodel"] = paymodel;
                            if (paymodel.Page == "Invitation")
                            {
                                var update = SaveInvitation(hfeventId.ToString(), PayId, op.SendInviteType);
                                //  var update = SendInvitation(paym.eventId.ToString(), paym.PaymentId);

                            }
                            else if (Page == "BroadCast")
                            {
                                var update = broadcastEmail(hfeventId.ToString(), PayId);
                            }
                            else if (Page == "Subscription")
                            {
                                UpdateEvevntSubscription(hfeventId.ToString(), op.subtype, PayId);
                            }
                            return RedirectToAction("Status", "Transaction", new { Area = "Organizer", Status = "", page = Page, PaymentId = PayId, eventId = hfeventId, subtype = type, invType = InvType });

                            //TempData["additionalData"] = "Person created successfully";
                            //return RedirectToAction("Status", "Payment", new { Area = "Organizer" });
                        }
                        else
                        {
                            //status = HandlePayment.CheckPayment_Status(TransId, clientsecret);
                            //status = "Server Error";
                            var updatepay = PaymentLogic.UpdatePaymentStatus(PayId, "Failed", null);
                            return RedirectToAction("Status", "Transaction", new { Area = "Organizer", Status = "fail", page = Page, PaymentId = PayId, eventId = hfeventId, subtype = type, invType = InvType });
                        }
                        //var updates = SendInvitation(hfeventId.ToString(), PayId);
                        //return RedirectToAction("Status", "Transaction", new { Area = "Organizer", Status = status.status, PaymentId = PayId, eventId= hfeventId, subtype = type, invType = InvType });

                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Status", "Transaction", new { Area = "Organizer", Status = "Server Error", page = Page, PaymentId = PayId, eventId = hfeventId, subtype = type, invType = InvType });
                    }
                }
                return RedirectToAction("Status", "Transaction", new { Area = "Organizer", Status = "Server Error", page = Page, PaymentId = PayId, eventId = hfeventId, subtype = type, invType = InvType });
            }
            else
            {
                op.page = Page;
                op.eventId = hfeventId.ToString();
                TempData["op"] = op;
                return RedirectToAction("Dashboard", "Organizer", new { Area = "Organizer" });
            }
        }
        [HttpPost]
        public ActionResult CardPay(int hfeventId, string total, string type, string Page, int InvType = 0)
        {
            OrganizerPay op = new OrganizerPay();
            if (TempData["op"] != null)
            {
                op = TempData["op"] as OrganizerPay; int PayId = 0;
                string TransId = Guid.NewGuid().ToString();
                //int PayId = PaymentLogic.InsertPaymentWithPendingStatus(TransId, Page, Convert.ToDecimal(total), false, PaymentGatewayName.GP_CardPay.ToString());

                decimal? TotalAmount = Convert.ToDecimal(total);

                //HTTP POST
                var result = HandlePayment.MakeCardPaymentRequest(TransId, TotalAmount.ToString(), hfeventId.ToString(), ManageSession.CompanySession.Id.ToString(), Page); // client.PostAsync("", content);

                if (result.IsSuccessStatusCode)
                {
                    string refId = result.Content.ReadAsStringAsync().Result;
                    PayId = PaymentLogic.InsertPaymentWithPendingStatus(TransId, Page, Convert.ToDecimal(TotalAmount), false, PaymentGatewayName.GP_CardPay.ToString());
                    if (PayId > 0)
                    {
                        try
                        {
                            PaymentStatuss paymodel = new PaymentStatuss();
                            //paymodel.clientsecret = clientsecret;
                            paymodel.eventId = hfeventId;
                            paymodel.Page = Page;
                            paymodel.PaymentId = PayId;
                            paymodel.substype = type;
                            paymodel.TransId = TransId;
                            TempData["paymodel"] = paymodel;
                            if (paymodel.Page == "Invitation")
                            {
                                var update = SaveInvitation(hfeventId.ToString(), PayId, op.SendInviteType);
                                //  var update = SendInvitation(paym.eventId.ToString(), paym.PaymentId);
                            }
                            else if (Page == "BroadCast")
                            {
                                var update = broadcastEmail(hfeventId.ToString(), PayId);
                            }
                            else if (Page == "Subscription")
                            {
                                UpdateEvevntSubscription(hfeventId.ToString(), op.subtype, PayId);
                            }
                            if (refId.Length < 15)
                            {
                                TempData["payId"] = PayId; TempData["refId"] = refId; TempData["evId"] = hfeventId;
                                //var paymentUrl = "https://www.zenithbank.com.gh/api.globalpay/Test/PaySecure?GPID=" + ConfigurationManager.AppSettings["GPID"] + "&tid=" + refId;
                                var paymentUrl = CardPayURL.PaymentSubmitURL + "?GPID=" + ConfigurationManager.AppSettings["GPID"] + "&tid=" + refId;
                                var updateRef = PaymentLogic.UpdateCardtrxId(PayId, refId);
                                return Redirect(paymentUrl);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.LogException(ex);
                            Log4Net.Error("Api response in MakeCardPayException:-" + ex.ToString());
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Invitation", new { Area = "Organizer" });
        }
        [HttpPost]
        public ActionResult PaymentReq(int hfeventId, string mob, string total, string type, string Page, int InvType = 0)
        {
            OrganizerPay op = new OrganizerPay();
            if (TempData["op"] != null)
            {
                op = TempData["op"] as OrganizerPay;
                var dbConn = new EventmanagerEntities();
                //if (ManageSession.TicketCartSession == null)
                //{
                //    return RedirectToAction("EventDetail", "Home", new { Id = hfeventId });
                //}
                string TransId = Guid.NewGuid().ToString();

                decimal? TotalAmount = Convert.ToDecimal(total);

                PaymentHubtelModel mpay = new PaymentHubtelModel();
                mpay.totalAmount = TotalAmount.ToString(); mpay.clientReference = TransId; mpay.description = "Purchase.";
                HttpResponseMessage responses = HandlePayment.MakeHubtelPaymentRequest(TransId, mpay);
                if (responses.StatusCode.ToString() == "OK")
                {
                    var resp = responses.Content.ReadAsAsync<HubtelResponse>().Result;

                    if (resp.status == "Success")
                    {
                        try
                        {
                            int PayId = PaymentLogic.InsertPaymentWithPendingStatus(TransId, Page, 0, false, PaymentGatewayName.Hubtel.ToString());
                            if (PayId > 0)
                            {
                                PaymentStatuss paymodel = new PaymentStatuss();
                                //paymodel.clientsecret = clientsecret;
                                paymodel.eventId = hfeventId;
                                paymodel.Page = Page;
                                paymodel.PaymentId = PayId;
                                paymodel.substype = type;
                                paymodel.TransId = TransId;
                                TempData["paymodel"] = paymodel;
                                if (paymodel.Page == "Invitation")
                                {
                                    var update = SaveInvitation(hfeventId.ToString(), PayId, op.SendInviteType);
                                    //  var update = SendInvitation(paym.eventId.ToString(), paym.PaymentId);
                                }
                                else if (Page == "BroadCast")
                                {
                                    var update = broadcastEmail(hfeventId.ToString(), PayId);
                                }
                                else if (Page == "Subscription")
                                {
                                    UpdateEvevntSubscription(hfeventId.ToString(), op.subtype, PayId);
                                }
                                TempData["payId"] = PayId; TempData["refId"] = resp.data.checkoutId; TempData["evId"] = hfeventId;
                                var paymentUrl = resp.data.checkoutUrl;
                                var updateRef = PaymentLogic.UpdateCardtrxId(PayId, resp.data.checkoutId);
                                return Redirect(paymentUrl);
                            }
                        }
                        catch (Exception)
                        {

                        }

                    }

                }
                return RedirectToAction("Dashboard", "Organizer", new { Area = "Organizer" });
            }
            else
            {
                op.page = Page;
                op.eventId = hfeventId.ToString();
                TempData["op"] = op;
                return RedirectToAction("Dashboard", "Organizer", new { Area = "Organizer" });
            }
        }
        [HttpPost]
        public ActionResult expressPayReq(int hfeventId, string mob, string total, string type, string Page, int InvType = 0)
        {
            OrganizerPay op = new OrganizerPay();
            if (TempData["op"] != null)
            {
                op = TempData["op"] as OrganizerPay;
                var dbConn = new EventmanagerEntities();
                //if (ManageSession.TicketCartSession == null)
                //{
                //    return RedirectToAction("EventDetail", "Home", new { Id = hfeventId });
                //}
                string TransId = Guid.NewGuid().ToString();

                decimal? TotalAmount = Convert.ToDecimal(total);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(PayStackURL.PaymentRequestURL);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer sk_test_8bae07bc7cfcbacdcc6b92827d550569a3a77096");
                    var CurrentURL = System.Web.HttpContext.Current.Request;
                    string ReturnURL = string.Format("{0}://{1}", CurrentURL.Url.Scheme, CurrentURL.Url.Authority) + "/Payment/paystackCallback";
                    var values = new Dictionary<string, string>
                    {
                        { "first_name", ManageSession.CompanySession.FirstName },
                        { "last_name", ManageSession.CompanySession.LastName },
                        { "email", ManageSession.CompanySession.EmailId },
                        { "phone", "0222222222" },
                        { "amount", TotalAmount.ToString() },
                        { "currency", "GHS" },
                        { "reference", TransId },
                        { "callback_url", ReturnURL },

                    };
                    var content = new FormUrlEncodedContent(values);
                    //HTTP POST
                    var postTask = client.PostAsync("", content);
                    postTask.Wait();

                    var responses = postTask.Result;
                    if (responses.IsSuccessStatusCode)
                    {
                        string refId = responses.Content.ReadAsStringAsync().Result;
                        string jsonString = responses.Content.ReadAsStringAsync()
                                               .Result
                                               .Replace("-", "_");
                        var resp = JsonConvert.DeserializeObject<PayStackRequestResponse>(jsonString);
                        Log4Net.Error("payStack request :  " + jsonString);

                        if (resp.data != null && resp.data.authorization_url != null)
                        {
                            try
                            {
                                int PayId = PaymentLogic.InsertPaymentWithPendingStatus(TransId, Page, Convert.ToDecimal(TotalAmount), false, PaymentGatewayName.PayStack.ToString());
                                if (PayId > 0)
                                {
                                    PaymentStatuss paymodel = new PaymentStatuss();
                                    //paymodel.clientsecret = clientsecret;
                                    paymodel.eventId = hfeventId;
                                    paymodel.Page = Page;
                                    paymodel.PaymentId = PayId;
                                    paymodel.substype = type;
                                    paymodel.total = Convert.ToDecimal(TotalAmount).ToString();
                                    paymodel.TransId = TransId;
                                    TempData["paymodel"] = paymodel;
                                    if (paymodel.Page == "Invitation")
                                    {
                                        var update = SaveInvitation(hfeventId.ToString(), PayId, op.SendInviteType);
                                        //  var update = SendInvitation(paym.eventId.ToString(), paym.PaymentId);
                                    }
                                    else if (Page == "BroadCast")
                                    {
                                        var update = broadcastEmail(hfeventId.ToString(), PayId);
                                    }
                                    else if (Page == "Subscription")
                                    {
                                        UpdateEvevntSubscription(hfeventId.ToString(), op.subtype, PayId);
                                    }
                                    //string paymentUrl = ExpressPayURL.PaymentSubmitURL + "?token=" + resp.data.reference;
                                    string paymentUrl = resp.data.authorization_url;
                                    TempData["paymodel"] = paymodel;
                                    //return Redirect(paymentUrl);
                                    var updatepay = PaymentLogic.UpdatePaymentStatus(PayId, "SUCCESSFUL", "", "");
                                    PaymentStatuss p = new PaymentStatuss();
                                    p.status = "SUCCESSFUL";
                                    p.TransId = TransId;
                                    p.total = "0";
                                    p.Page = "Subscription";
                                    p.message = "Broadcast Email Sent successfully!";
                                    TempData["status"] = p;
                                    return Redirect(paymentUrl);
                                    //return RedirectToAction("Status", "Transaction", new { Status = p.status, PaymentId = PayId, eventId = hfeventId, subtype = type, Area = "Organizer" });
                                }

                            }
                            catch (Exception)
                            {

                            }

                        }

                    }
                    return RedirectToAction("Dashboard", "Organizer", new { Area = "Organizer" });
                }
            }
            else
            {
                op.page = Page;
                op.eventId = hfeventId.ToString();
                TempData["op"] = op;
                return RedirectToAction("Dashboard", "Organizer", new { Area = "Organizer" });
            }

            //return RedirectToAction("EventDetail", "Home", new { Id = hfeventid });
        }
        [HttpPost]
        public ActionResult PayStackReq(int hfeventId, string mob, string total, string type, string Page, int InvType = 0)
        {
            OrganizerPay op = new OrganizerPay();
            if (TempData["op"] != null)
            {
                op = TempData["op"] as OrganizerPay;
                var dbConn = new EventmanagerEntities();
                //if (ManageSession.TicketCartSession == null)
                //{
                //    return RedirectToAction("EventDetail", "Home", new { Id = hfeventId });
                //}
                string TransId = Guid.NewGuid().ToString();

                decimal? TotalAmount = Convert.ToDecimal(total);
                if (TotalAmount > 0)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(PayStackURL.PaymentRequestURL);
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer sk_test_8bae07bc7cfcbacdcc6b92827d550569a3a77096");
                        //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigurationManager.AppSettings["Paystack_secretKey"]);
                        var CurrentURL = System.Web.HttpContext.Current.Request;
                        string ReturnURL = string.Format("{0}://{1}", CurrentURL.Url.Scheme, CurrentURL.Url.Authority) + "/Payment/paystackCallback";
                        string CancelURL = string.Format("{0}://{1}", CurrentURL.Url.Scheme, CurrentURL.Url.Authority) + "/Payment/paystackCanceltransaction";
                        var values = new Dictionary<string, string>
                    {
                        { "first_name", ManageSession.CompanySession.FirstName },
                        { "last_name", ManageSession.CompanySession.LastName },
                        { "email", ManageSession.CompanySession.EmailId },
                        { "phone", "0222222222" },
                        { "amount", (TotalAmount*100).ToString() },
                        { "currency", "GHS" },
                        { "reference", TransId },
                        { "callback_url", ReturnURL },
                        //{ "custom_fields",{"display_name","Cancel Payment"}, { "variable_name","Cancel"}, { "value",CancelURL} }
                    };
                        var content = new FormUrlEncodedContent(values);
                        //HTTP POST
                        var postTask = client.PostAsync("", content);
                        postTask.Wait();

                        var responses = postTask.Result;
                        if (responses.IsSuccessStatusCode)
                        {
                            string refId = responses.Content.ReadAsStringAsync().Result;
                            string jsonString = responses.Content.ReadAsStringAsync()
                                                   .Result
                                                   .Replace("-", "_");
                            var resp = JsonConvert.DeserializeObject<PayStackRequestResponse>(jsonString);
                            Log4Net.Error("payStack request :  " + jsonString);

                            if (resp.data != null && resp.data.authorization_url != null)
                            {
                                try
                                {
                                    int PayId = PaymentLogic.InsertPaymentWithPendingStatus(TransId, Page, Convert.ToDecimal(TotalAmount), false, PaymentGatewayName.PayStack.ToString());
                                    if (PayId > 0)
                                    {
                                        PaymentStatuss paymodel = new PaymentStatuss();
                                        PaymentStatuss p = new PaymentStatuss();
                                        //paymodel.clientsecret = clientsecret;
                                        paymodel.eventId = hfeventId;
                                        paymodel.Page = Page;
                                        paymodel.PaymentId = PayId;
                                        paymodel.substype = type;
                                        paymodel.total = Convert.ToDecimal(TotalAmount).ToString();
                                        paymodel.TransId = TransId;
                                        TempData["paymodel"] = paymodel;
                                        p.total = "0";
                                        if (paymodel.Page == "Invitation")
                                        {
                                            var update = SaveInvitation(hfeventId.ToString(), PayId, op.SendInviteType);
                                            //  var update = SendInvitation(paym.eventId.ToString(), paym.PaymentId);
                                        }
                                        else if (Page == "BroadCast")
                                        {
                                            var update = broadcastEmail(hfeventId.ToString(), PayId);
                                            p.Page = "BroadCast";
                                            p.message = "Broadcast Email Sent successfully!";

                                        }
                                        else if (Page == "Coupan")
                                        {
                                            if (Session["Offers"] != null)
                                            {
                                                OrganizerDbOperation.AddOffers(Session["Offers"] as OffersModel, PayId);
                                                p.total = Convert.ToDecimal(TotalAmount).ToString();
                                                p.Page = "Coupan";
                                                p.message = "Coupon created successfully!";
                                            }
                                        }
                                        else if (Page == "Subscription")
                                        {
                                            UpdateEvevntSubscription(hfeventId.ToString(), op.subtype, PayId);
                                        }
                                        //string paymentUrl = ExpressPayURL.PaymentSubmitURL + "?token=" + resp.data.reference;
                                        string paymentUrl = resp.data.authorization_url;
                                        TempData["paymodel"] = paymodel;
                                        //return Redirect(paymentUrl);
                                        var updatepay = PaymentLogic.UpdatePaymentStatus(PayId, "SUCCESSFUL", "", "");

                                        p.status = "SUCCESSFUL";
                                        p.TransId = TransId;


                                        TempData["status"] = p;
                                        return Redirect(paymentUrl);
                                        //return RedirectToAction("Status", "Transaction", new { Status = p.status, PaymentId = PayId, eventId = hfeventId, subtype = type, Area = "Organizer" });
                                    }

                                }
                                catch (Exception)
                                {

                                }

                            }

                        }
                        return RedirectToAction("Dashboard", "Organizer", new { Area = "Organizer" });
                    }
                }
                else
                {
                    try
                    {
                        int PayId = PaymentLogic.InsertPaymentWithPendingStatus(TransId, Page, Convert.ToDecimal(TotalAmount), false, PaymentGatewayName.PayStack.ToString());
                        if (PayId > 0)
                        {
                            PaymentStatuss paymodel = new PaymentStatuss();
                            PaymentStatuss p = new PaymentStatuss();
                            //paymodel.clientsecret = clientsecret;
                            paymodel.eventId = hfeventId;
                            paymodel.Page = Page;
                            paymodel.PaymentId = PayId;
                            paymodel.substype = type;
                            paymodel.total = Convert.ToDecimal(TotalAmount).ToString();
                            paymodel.TransId = TransId;
                            TempData["paymodel"] = paymodel;
                            p.total = "0";
                            if (paymodel.Page == "Invitation")
                            {
                                var update = SaveInvitation(hfeventId.ToString(), PayId, op.SendInviteType);
                                //  var update = SendInvitation(paym.eventId.ToString(), paym.PaymentId);
                            }
                            else if (Page == "BroadCast")
                            {
                                var update = broadcastEmail(hfeventId.ToString(), PayId);
                                p.Page = "BroadCast";
                                p.message = "Broadcast Email Sent successfully!";

                            }
                            else if (Page == "Coupan")
                            {
                                if (Session["Offers"] != null)
                                {
                                    OrganizerDbOperation.AddOffers(Session["Offers"] as OffersModel, PayId);
                                    p.total = Convert.ToDecimal(TotalAmount).ToString();
                                    p.Page = "Coupan";
                                    p.message = "Coupon created successfully!";
                                }
                            }
                            else if (Page == "Subscription")
                            {
                                UpdateEvevntSubscription(hfeventId.ToString(), op.subtype, PayId);
                            }
                            TempData["paymodel"] = paymodel;
                            var updatepay = PaymentLogic.UpdatePaymentStatus(PayId, "SUCCESSFUL", "", "");
                            p.status = "SUCCESSFUL";
                            p.TransId = TransId;
                            TempData["status"] = p;
                          
                            //return RedirectToAction("Status", "Transaction", new { Status = p.status, PaymentId = PayId, eventId = hfeventId, subtype = type, Area = "Organizer" });
                        }

                    }
                    catch (Exception)
                    {

                    }
                    return RedirectToAction("Dashboard", "Organizer", new { Area = "Organizer" });
                }
            }
            else
            {
                op.page = Page;
                op.eventId = hfeventId.ToString();
                TempData["op"] = op;
                return RedirectToAction("Dashboard", "Organizer", new { Area = "Organizer" });
            }

            //return RedirectToAction("EventDetail", "Home", new { Id = hfeventid });
        }
        public ActionResult Status(string Status, int PaymentId, int eventId, int subtype, int invType = 1)
        {

            // TempData["op"] = null;
            if (!string.IsNullOrEmpty(Status) && Status == "Failed")
            {
                ViewData["Status"] = "fail";
            }
            else
            {
                ViewData["Status"] = null;
            }
            TempData["totals"] = TempData["total"];
            TempData["invType"] = invType; // TempData["invType"];
            ViewData["total"] = TempData["total"];
            ViewData["TransactionId"] = TempData["TransactionId"];
            TempData.Keep("total");
            if (Status != null & TempData["status"] != null)
            {
                var p = TempData["status"] as PaymentStatuss;
                ViewData["Status"] = Status;
                ViewBag.trxId = p.TransId;
                ViewBag.msg = p.message;
                ViewBag.total = p.total;
                if (p.Page == "Invitation") { ViewBag.Page = p.Page; }

            }
            return View();
        }

        public dynamic CheckPayStatus()
        {
            if (TempData["paymodel"] != null)
            {
                var paym = TempData["paymodel"] as PaymentStatuss;
                TempData["paymodel"] = paym;
                TempData.Keep("paymodel");
                StatusResponse status = new StatusResponse();//= CheckPayment_Status(TransId);  
                try
                {
                    PaymentStatuss p = new PaymentStatuss();
                    //System.Threading.Thread.Sleep(10000);
                    status = HandlePayment.CheckPayment_Status(paym.TransId, paym.clientsecret);
                    //while (status.status == "PENDING")
                    //{
                    //    System.Threading.Thread.Sleep(5000);
                    //    status = HandlePayment.CheckPayment_Status(paym.TransId, paym.clientsecret);
                    //};
                    //status.status = "PENDING";
                    //status.message = "Your Payment is pending";
                    var updatepay = PaymentLogic.UpdatePaymentStatus(paym.PaymentId, status.status, status.financialTransactionId);
                    if (status.status == "SUCCESSFUL")
                    {
                        if (paym.Page == "Subscription")
                        {
                            var update = UpdateEvevntSubscription(paym.eventId.ToString(), paym.substype, paym.PaymentId);
                            if (update) { p.message = "Subscription created successfully!"; }
                        }
                        else if (paym.Page == "Invitation")
                        {
                            p.message = "Invitation Send successfully!";
                            //  var update = false;
                            // var  InvType = Convert.ToInt32( TempData["invType"].ToString());
                            //  if (InvType == (int)SendInvitationType.Email)
                            //  {
                            //      update = SendInvitation(paym.eventId.ToString(), paym.PaymentId);
                            //  }
                            //  else if (InvType == (int)SendInvitationType.SMS)
                            //  {
                            //      update = SendInvitationBySMS(paym.eventId.ToString(), paym.PaymentId);
                            //  }
                            //  else if (InvType == (int)SendInvitationType.Both)
                            //  {
                            //      update = SendInvitationEmailSMS(paym.eventId.ToString(), paym.PaymentId);
                            //  }
                            ////  var update = SendInvitation(paym.eventId.ToString(), paym.PaymentId);
                            //  if (update) { p.message = "Invitation Send successfully!"; }
                        }
                        else if (paym.Page == "BroadCast")
                        {
                            //var update = broadcastEmail(paym.eventId.ToString(), paym.PaymentId);
                            p.message = "Broadcast Email Send successfully!";
                        }

                        p.status = status.status;
                        p.TransId = paym.TransId;
                        p.total = status.amount; //Convert.ToString(TempData["totals"]) ;
                        return Json(p);
                    }
                    else if (status.status == "PENDING")
                    {
                        p.message = "Your Payment is pending by telco.";
                    }
                    else
                    {
                        p.status = status.status;
                        if (status.message == "PENDING")
                        {
                            status.status = "PENDING";
                        }
                    }
                    p.TransId = paym.TransId;
                    p.message = status.message;
                    p.total = status.amount;
                    return Json(p);
                    //return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        public bool UpdateEvevntSubscription(string EventId, string SubscriptionType, int paymId)
        {
            try
            {
                ApiResponse Resp = new ApiResponse();
                if (!string.IsNullOrEmpty(EventId) && !string.IsNullOrEmpty(SubscriptionType))
                {
                    Resp = OrganizerDbOperation.UpdateSubscription(Convert.ToInt32(EventId), Convert.ToInt32(SubscriptionType), paymId);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }
        public ActionResult FailedPayment()
        {
            var EventList = OrganizerDbOperation.GetEventName("");
            ViewBag.Eventlist = EventList;
            var res = PaymentLogic.getFailedpayment(ManageSession.CompanySession.Id, 1);

            return View(res);
        }
        public ActionResult getFailedPayment(int eventId)
        {
            var res = PaymentLogic.getFailedpaymentStatus(ManageSession.CompanySession.Id, eventId);
            return PartialView("_FailedPayment", res);
        }
        public dynamic ChecktrxStatus(string PaymentId, string TrxNo, string paymentfor, string PayGateway, int EventId)
        {
            //StatusResponse result = HandlePayment.CheckPayment_Status(TrxNo, "");
            StatusResponse result = new StatusResponse();
            if (PayGateway == PaymentGatewayName.PayStack.ToString())
            {
                result = HandlePayment.PayStackstatus(TrxNo);
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
            }
            return PartialView("_FailedPayStatusInOrganizer", result);
        }
        public bool SaveInvitation(string EventId, int paymId, int sendType)
        {
            try
            {
                if (!string.IsNullOrEmpty(EventId))
                {
                    List<InvitationDetail> result = ManageSession.ExcelDataSession;
                    foreach (var InvitationDetail in result)
                    {
                        if (InvitationDetail.EmailAddress != null) { InvitationDetail.SendInviteType = (int)SendInvitationType.Email; }
                        if (InvitationDetail.MobileNumber != null) { InvitationDetail.SendInviteType = (int)SendInvitationType.SMS; }
                        if (InvitationDetail.EmailAddress != null && InvitationDetail.MobileNumber != null) { InvitationDetail.SendInviteType = (int)SendInvitationType.Both; }

                        try
                        {
                            int EventTickeId = new InvitationController().PostExcelData(Convert.ToInt32(EventId), paymId, InvitationDetail, InvitationDetail.SendInviteType);
                        }
                        catch { }
                    }
                    ManageSession.ExcelDataSession = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }
        public bool SendInvitation(string EventId, int paymId)
        {
            try
            {
                if (!string.IsNullOrEmpty(EventId))
                {
                    List<InvitationDetail> result = ManageSession.ExcelDataSession;
                    foreach (var InvitationDetail in result)
                    {
                        try
                        {
                            int EventTickeId = new InvitationController().PostExcelData(Convert.ToInt32(EventId), paymId, InvitationDetail, 1);
                            if (EventTickeId > 0)
                            {
                                //send inviation email to user
                                ModelInviation.SendInvitation(Convert.ToInt32(EventId), EventTickeId, InvitationDetail.EmailAddress, InvitationDetail.FirstName + " " + InvitationDetail.LastName);
                            }
                        }
                        catch { }
                    }
                    ManageSession.ExcelDataSession = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }

        public bool SendInvitationBySMS(string EventId, int paymId)
        {
            try
            {
                if (!string.IsNullOrEmpty(EventId))
                {
                    List<InvitationDetail> result = ManageSession.ExcelDataSession;
                    foreach (var InvitationDetail in result)
                    {
                        try
                        {
                            EventmanagerEntities DbEntity = new EventmanagerEntities();
                            int EventTickeId = new InvitationController().PostExcelData(Convert.ToInt32(EventId), paymId, InvitationDetail, 2);
                            if (EventTickeId > 0)
                            {

                                var request = System.Web.HttpContext.Current.Request;
                                var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);

                                var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(EventId)));
                                var Link = HttpUtility.UrlDecode(address + "Promotion/SendTicket?EventTickeId=" + EventId + "&Status=" + 1);
                                var Event_Id = Convert.ToInt32(EventId);
                                var EventName = DbEntity.Events.Where(x => x.Id == Event_Id).FirstOrDefault().Event_name; // item.EventTicket.Event.Event_name;
                                //GenerateBarcodeandsendTicke(item.BarCodeNumber);
                                CommonSMSModal commonSMSModal = new CommonSMSModal()
                                {
                                    PhoneReciever = InvitationDetail.MobileNumber, //"+918882262496"
                                    Text = ManageSession.UserSession.FirstName + "  has Sent you an invite using ticketsandinvites. " + "" + " " + Link,
                                    PhoneSender = ""// DbEntity.Companies.Where(x => x.Id == ManageSession.CompanySession.Id).FirstOrDefault().Business_contact_number
                                };
                                Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                threadSMS.Start();
                                //send inviation email to user
                                // ModelInviation.SendInvitation(Convert.ToInt32(EventId), EventTickeId, InvitationDetail.EmailAddress, InvitationDetail.FirstName + " " + InvitationDetail.LastName);
                            }
                        }
                        catch { }
                    }
                    ManageSession.ExcelDataSession = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }

        public bool SendInvitationEmailSMS(string EventId, int paymId)
        {
            try
            {
                if (!string.IsNullOrEmpty(EventId))
                {
                    List<InvitationDetail> result = ManageSession.ExcelDataSession;
                    foreach (var InvitationDetail in result)
                    {
                        try
                        {
                            EventmanagerEntities DbEntity = new EventmanagerEntities();
                            int EventTickeId = new InvitationController().PostExcelData(Convert.ToInt32(EventId), paymId, InvitationDetail, 3);
                            if (EventTickeId > 0)
                            {

                                var request = System.Web.HttpContext.Current.Request;
                                var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);

                                var IdConvert = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(EventId)));
                                var Link = HttpUtility.UrlDecode(address + "Promotion/SendTicket?EventTickeId=" + EventId + "&Status=" + 1);
                                var Event_Id = Convert.ToInt32(EventId);
                                var EventName = DbEntity.Events.Where(x => x.Id == Event_Id).FirstOrDefault().Event_name; // item.EventTicket.Event.Event_name;
                                //GenerateBarcodeandsendTicke(item.BarCodeNumber);
                                CommonSMSModal commonSMSModal = new CommonSMSModal()
                                {
                                    PhoneReciever = InvitationDetail.MobileNumber, //"+918882262496"
                                    Text = ManageSession.UserSession.FirstName + "  has Sent you an invite using ticketsandinvites. " + "" + ":- " + Link,
                                    PhoneSender = ""// DbEntity.Companies.Where(x => x.Id == ManageSession.CompanySession.Id).FirstOrDefault().Business_contact_number
                                };
                                if (InvitationDetail.MobileNumber != null)
                                {
                                    Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                                    threadSMS.Start();
                                }
                                //send inviation email to user
                                ModelInviation.SendInvitation(Convert.ToInt32(EventId), EventTickeId, InvitationDetail.EmailAddress, InvitationDetail.FirstName + " " + InvitationDetail.LastName);
                            }
                        }
                        catch { }
                    }
                    ManageSession.ExcelDataSession = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }
    }
}