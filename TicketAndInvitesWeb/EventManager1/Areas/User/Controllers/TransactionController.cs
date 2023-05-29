using EventManager1.Areas.Organizer.Controllers;
using EventManager1.Areas.Organizer.Models;
using EventManager1.DBCon;
using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using static EventManager1.MvcApplication;


namespace EventManager1.Areas.User.Controllers
{
    [UserSessionExpire]
    public class TransactionController : Controller
    {
        HandleEvent hdb = new HandleEvent();
        // GET: User/Transaction
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Summery()
        {
            var op = TempData["op"] as OrganizerPay;
            if (op != null)
            {
                op.ImageUrl = hdb.Getimageurl(Convert.ToInt32(op.eventId));
                op.AdminFee = hdb.GetInvitaionfee(op.page);                
                TempData.Keep("op");
            }
            return View(op);
        }
        public ActionResult summeryPay(int hfeventId, string mob, string total, string type, string Page, int InvType)
        {
            OrganizerPay op = new OrganizerPay();
            if (TempData["op"] != null)
            {
                op = TempData["op"] as OrganizerPay;
                string TransId = Guid.NewGuid().ToString();
                int PayId = PaymentLogic.InsertPaymentWithPendingStatus(TransId, Page, Convert.ToDecimal(op.total), true, PaymentGatewayName.Mono_Payment.ToString());
                string clientsecret = "";// HandlePayment.Getclientsecret(TransId);
                decimal? TotalAmount = Convert.ToDecimal(op.total);
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
                mpay.externalId = PayId.ToString();// pay.externalId;
                mpay.payer = p;
                mpay.payeeNote = Page;//pay.payeeNote;
                mpay.payerMessage = "TestMessage";// pay.payerMessage;                      
                StatusResponse status;//= CheckPayment_Status(TransId);            
                if (PayId > 0)
                {
                    try
                    {
                        HttpResponseMessage responses = HandlePayment.MakePaymentRequest(TransId, clientsecret, mpay);
                        if (responses.StatusCode.ToString() == "Accepted" || responses.StatusCode.ToString() == "Conflict")
                        {
                            TempData["TransactionId"] = TransId;
                            TempData["InvType"] = InvType;
                            //System.Threading.Thread.Sleep(10000);
                            PaymentStatuss paymodel = new PaymentStatuss();
                            paymodel.eventId = hfeventId;
                            paymodel.Page = Page;
                            paymodel.PaymentId = PayId;
                            paymodel.substype = type;
                            paymodel.TransId = TransId;
                            TempData["paymodel"] = paymodel;
                            var update = SaveInvitation(hfeventId.ToString(), PayId, op.SendInviteType);
                            //TempData["additionalData"] = "Person created successfully";
                            //return RedirectToAction("Status", "Payment", new { Area = "Organizer" });
                        }
                        //else
                        //{
                        //    status = HandlePayment.CheckPayment_Status(TransId, clientsecret);
                        //    //status = "Server Error";
                        //}
                        TempData["total"] = op.total;
                        //if (status.status == "SUCCESSFUL" )
                        //{
                        //    var update = false;
                        //    if (InvType == (int)SendInvitationType.Email)
                        //    {
                        //         update = SendInvitation(hfeventId.ToString(), PayId);
                        //    } else if (InvType == (int)SendInvitationType.SMS)
                        //    {
                        //         update = SendInvitationBySMS(hfeventId.ToString(), PayId);
                        //    } else if(InvType == (int)SendInvitationType.Both)
                        //    {
                        //         update = SendInvitationEmailSMS(hfeventId.ToString(), PayId);
                        //    }

                        //    if (update) { TempData["Message"] = "Invitation Send successfully!"; }

                        //}
                        //var updates = SendInvitation(hfeventId.ToString(), PayId);
                        return RedirectToAction("Status", "Transaction", new { Area = "User", Status = "", PaymentId = PayId, eventId = hfeventId, subtype = type, invType = InvType });

                    }
                    catch (Exception ex)
                    { return RedirectToAction("Status", "Transaction", new { Area = "User", Status = "Server Error", PaymentId = PayId, eventId = hfeventId, subtype = type }); }
                }
                return RedirectToAction("Status", "Transaction", new { Area = "User", Status = "Server Error", PaymentId = PayId, eventId = hfeventId, subtype = type });
            }
            else { return RedirectToAction("Summery", "Transaction", new { Area = "User" }); }
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
                var result = HandlePayment.MakeCardPaymentRequest(TransId, TotalAmount.ToString(), hfeventId.ToString(), ManageSession.UserSession.Id.ToString(), Page); // client.PostAsync("", content);

                if (result.IsSuccessStatusCode)
                {
                    try
                    {
                    string refId = result.Content.ReadAsStringAsync().Result;
                    PayId = PaymentLogic.InsertPaymentWithPendingStatus(TransId, Page, Convert.ToDecimal(TotalAmount) , true, PaymentGatewayName.GP_CardPay.ToString());
                    if (PayId > 0)
                    {
                        
                            PaymentStatuss paymodel = new PaymentStatuss();
                            //paymodel.clientsecret = clientsecret;
                            paymodel.eventId = hfeventId;
                            paymodel.Page = Page;
                            paymodel.PaymentId = PayId;
                            paymodel.substype = type;
                            paymodel.TransId = TransId;
                            paymodel.clientsecret = "U";
                            TempData["paymodel"] = paymodel;
                            
                            var update = SaveInvitation(hfeventId.ToString(), PayId, op.SendInviteType);
                             
                            if (refId.Length < 15)
                            {
                                TempData["payId"] = PayId; TempData["refId"] = refId; TempData["evId"] = hfeventId;
                                //var paymentUrl = "https://www.zenithbank.com.gh/api.globalpay/Test/PaySecure?GPID=" + ConfigurationManager.AppSettings["GPID"] + "&tid=" + refId;
                                var paymentUrl = CardPayURL.PaymentSubmitURL +"? GPID=" + ConfigurationManager.AppSettings["GPID"] + "&tid=" + refId;
                                var updateRef = PaymentLogic.UpdateCardtrxId(PayId, refId);
                                return Redirect(paymentUrl);
                            }
                        
                    }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogException(ex);
                        Log4Net.Error("Api response in MakeCardPayException:-" + ex.ToString());
                    }
                }
            }
            return RedirectToAction("Index", "InvitationUser", new { Area = "User" });
        }
        [HttpPost]
        public ActionResult PaymentReq(int hfeventId, string mob, string total, string type, string Page, int InvType = 0)
        {
            OrganizerPay op = new OrganizerPay();
            if (TempData["op"] != null)
            {
                op = TempData["op"] as OrganizerPay;
                var dbConn = new EventmanagerEntities();
                
                string TransId = Guid.NewGuid().ToString();

                decimal? TotalAmount = Convert.ToDecimal(total);

                PaymentHubtelModel mpay = new PaymentHubtelModel();
                mpay.totalAmount = TotalAmount.ToString(); mpay.clientReference = TransId; mpay.description = "Invitation Purchase.";
                HttpResponseMessage responses = HandlePayment.MakeHubtelPaymentRequest(TransId, mpay);
                if (responses.StatusCode.ToString() == "OK")
                {
                    var resp = responses.Content.ReadAsAsync<HubtelResponse>().Result;

                    if (resp.status == "Success")
                    {
                        try
                        {
                            int PayId = PaymentLogic.InsertPaymentWithPendingStatus(TransId, Page, 0, true, PaymentGatewayName.Hubtel.ToString());
                            if (PayId > 0)
                            {
                                PaymentStatuss paymodel = new PaymentStatuss();
                                //paymodel.clientsecret = clientsecret;
                                paymodel.eventId = hfeventId;
                                paymodel.Page = Page;
                                paymodel.PaymentId = PayId;
                                paymodel.substype = type;
                                paymodel.TransId = TransId;
                                paymodel.clientsecret = "U";
                                TempData["paymodel"] = paymodel;
                                var update = SaveInvitation(hfeventId.ToString(), PayId, op.SendInviteType);
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
                return RedirectToAction("Index", "InvitationUser", new { Area = "User" });
            }
            else
            {
                op.page = Page;
                op.eventId = hfeventId.ToString();
                TempData["op"] = op;
                return RedirectToAction("Index", "InvitationUser", new { Area = "User" });
            }

            //return RedirectToAction("EventDetail", "Home", new { Id = hfeventid });
        }
        public ActionResult Status(string Status, int PaymentId, int eventId, int subtype, int invType=1)
        {
            if (Status != null )
            { ViewData["Status"] = "fail"; }
            TempData["totals"] = TempData["total"];
            TempData["invType"] = invType; // TempData["invType"];
            ViewData["total"] = TempData["total"];
            TempData.Keep("total");
            ViewData["Status"] = null;
            if (Status != null & TempData["status"] != null)
            {
                var p = TempData["status"] as PaymentStatuss;
                ViewData["Status"] = Status;
                ViewBag.trxId = p.TransId;
                ViewBag.msg = p.message;
                ViewBag.total = p.total;
            }
            return View();

        }
        public ActionResult FailedPayment()
        {            
            var res = PaymentLogic.getFailedpayment(ManageSession.UserSession.Id);
            return View(res);
        }
        
        public dynamic ChecktrxStatus(string PaymentId, string TrxNo, string paymentfor,string PayGateway)
        {
            StatusResponse result = new StatusResponse();
            //StatusResponse result = HandlePayment.CheckPayment_Status(TrxNo,"");  
            if (PayGateway == PaymentGatewayName.PayStack.ToString())
            {
                result = HandlePayment.PayStackstatus(TrxNo);
            }
            else {
                result = HandlePayment.HubtelPaymentstatus(TrxNo); }
            if (result.status == "Success")
            {
                result.status = "SUCCESSFUL";
                if (paymentfor== "Invitation") { 
                    result.message = "Invitation sent successfully.";
                    result.Page = "Invitation";
                }
                else
                {                    
                    PaymentLogic.Addtowallet(Convert.ToInt32( PaymentId) );
                    var v= PaymentLogic.reduceqty(Convert.ToInt32( PaymentId));                   
                    result.Page = "Ticket";
                    result.message = "Ticket sent successfully.";
                }
                var updatepay = PaymentLogic.UpdatePaymentStatus(Convert.ToInt32( PaymentId), result.status, result.financialTransactionId);
            
            }
            return PartialView("_FailedPayStatus", result);
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
                            int EventTickeId = new InvitationUserController().PostExcelData(Convert.ToInt32(EventId), paymId, InvitationDetail, InvitationDetail.SendInviteType);
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

        public dynamic CheckPayStatus()
        {
            if (TempData["paymodel"] != null)
            {
                int InvType = Convert.ToInt32( TempData["InvType"]);
                var paym = TempData["paymodel"] as PaymentStatuss;
                TempData["paymodel"] = paym;
                TempData.Keep("paymodel");
                StatusResponse status;//= CheckPayment_Status(TransId);  
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
                    //status.status = "PENDING";//FOR TESTING
                    //status.message = "Your pending is in pending state!";
                    p.status = status.status;
                    p.TransId = paym.TransId;                    
                    p.total = status.amount;
                    if (status.status == "SUCCESSFUL")
                    {
                       
                            p.message = "Invitation Send successfully!";
                            //var update = false;
                            //if (InvType == (int)SendInvitationType.Email)
                            //{
                            //    update = SendInvitation(paym.eventId.ToString(), paym.PaymentId );
                            //}
                            //else if (InvType == (int)SendInvitationType.SMS)
                            //{
                            //    update = SendInvitationBySMS(paym.eventId.ToString(), paym.PaymentId);
                            //}
                            //else if (InvType == (int)SendInvitationType.Both)
                            //{
                            //    update = SendInvitationEmailSMS(paym.eventId.ToString(), paym.PaymentId);
                            //}

                            //if (update) { TempData["Message"] = "Invitation Send successfully!"; 
                        
                        
                        
                    }
                    else if(status.status == "PENDING")
                    {
                        p.message = "Your Payment is pending by telco. Invitation will be send when it confirmed.";
                    }
                    else
                    {
                        if (status.message == "PENDING")
                        {
                            status.status = "PENDING";
                        }
                    }
                    var updatepay = PaymentLogic.UpdatePaymentStatus(paym.PaymentId, status.status, status.financialTransactionId);
                    return Json(p);
                    
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
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
                            int EventTickeId = new InvitationUserController().PostExcelData(Convert.ToInt32(EventId), paymId, InvitationDetail,1);
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
                            int EventTickeId = new InvitationUserController().PostExcelData(Convert.ToInt32(EventId), paymId, InvitationDetail,2);
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
                                    Text =   ManageSession.UserSession.FirstName + "  has Sent you an invite using ticketsandinvites. " + "" + ":- " + Link,
                                    PhoneSender = ""// DbEntity.Users.Where(x => x.Id == ManageSession.UserSession.Id).FirstOrDefault().PhoneNo
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
                            int EventTickeId = new InvitationUserController().PostExcelData(Convert.ToInt32(EventId), paymId, InvitationDetail,3);
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
                                    PhoneReciever =  InvitationDetail.MobileNumber, //"+918882262496"
                                    Text = ManageSession.UserSession.FirstName + "  has Sent you an invite using ticketsandinvites. " + "" + ":- " + Link,
                                    PhoneSender = ""// DbEntity.Users.Where(x => x.Id == ManageSession.UserSession.Id).FirstOrDefault().PhoneNo
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