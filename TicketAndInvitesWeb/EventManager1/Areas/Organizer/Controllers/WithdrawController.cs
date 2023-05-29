using EventManager1.Areas.Organizer.Models;
using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static EventManager1.MvcApplication;

namespace EventManager1.Areas.Organizer.Controllers
{
    [OrganizerSessionExpire]
    public class WithdrawController : Controller
    {
        // GET: Organizer/Withdraw
        public ActionResult Withdrawal(int type = 0, string startdate = null, string enddate = null)
        {
            WithdrawalModel res = new WithdrawalModel();
            //List<WithdrawModel> result = new List<WithdrawModel>();
            try { //result = OrganizerDbOperation.Getwithdrawal();
                res = OrganizerDbOperation.Getwithdrawals(type, startdate, enddate);
               
            }
            catch { }
            var msg = TempData["msg"];
            if (msg!= null)
            {
                ApiResponse re = new ApiResponse();
                re.Msg = msg.ToString();
                res.Resp = re;
            }
            return View(res);
           
        }
        [HttpPost]
        public ActionResult Withdrawal(decimal txtAmount)
        {
            
            var amount = OrganizerDbOperation.GetwithdrawalAmount();
            int acc = OrganizerDbOperation.GetbankDetails(); 
            decimal minamount = OrganizerDbOperation.Getminamount(txtAmount);
            if (minamount <= txtAmount) { 
            if (acc != 3) {
                if (amount > 0) {
                    if (txtAmount <= amount) {
                        var res = OrganizerDbOperation.reqwithdrawals(txtAmount);
                        ModelState.Clear();
                        TempData["msg"] = "Withdraw request sent successfully";
                    }
                        else { TempData["msg"] = "requested amount is greater than your balance"; }
                    }
                else { TempData["msg"] = "You have not enough balance"; }
            }
            else { TempData["msg"] = "Please update your banking and mobile money details."; }
            }
            else { if (minamount > 0) { TempData["msg"] = "You cann't withdraw amount less then "+ minamount; } }
            return RedirectToAction("Withdrawal");
        }

        public ActionResult updaterequest(int id)
        {   
            if(id > 0) { 
            var res = OrganizerDbOperation.updatereqwithdrawals(id);
            if (res) { TempData["msg"] = "Your request updated successfully."; }
            else { TempData["msg"] = "Something went wrong."; }
            }
            return RedirectToAction("Withdrawal");
        }

        public ActionResult Withdrawaldetails(int EventId)
        {
            try
            {
                List<WithdrawModel> result = CommonDbLogic.Getwithdrawaldetails(EventId);
                return PartialView("_withdraw", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_withdraw");
        }

    }

}