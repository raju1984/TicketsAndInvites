using EventManager1.Areas.Organizer.Models;
using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static EventManager1.MvcApplication;

namespace EventManager1.Areas.Organizer.Controllers
{

    [OrganizerSessionExpire]
    public class UserManagementController : Controller
    {
        // GET: Organizer/UserManagement
        public ActionResult AccountType()
        {
            AcccountTypeViewModel result = new AcccountTypeViewModel();
            result.Objects = OrganizerDbOperation.GetObjectmaster();
            result.Groupname = OrganizerDbOperation.GetGroupName(ManageSession.CompanySession.Id);
            if (TempData["resp"] != null)
            {
                result.Resp = (ApiResponse)TempData["resp"];
            }
            else
            {
                result.Resp = new ApiResponse() { Code = (int)ApiResponseCode.ok };
            }
            return View(result);
        }
        [HttpPost]
        public ActionResult AccountTypePost(AcccountTypeViewModel Model)
        {
            TempData["resp"] = OrganizerDbOperation.AddAccount(Model);
            return RedirectToAction("AccountType");
        }
        public ActionResult CreateUser()
        {
            UsermangeviewModel result = new UsermangeviewModel();
            result.UserModel = OrganizerDbOperation.GetUserbyAccount(ManageSession.CompanySession.Id);
            result.Accounts = OrganizerDbOperation.GetAllGroups(ManageSession.CompanySession.Id);
            if (TempData["resp"] != null)
            {
                result.Resp = (ApiResponse)TempData["resp"];
            }
            return View(result);
        }
        [HttpPost]
        public async Task<ActionResult> CreateUserPost(string txtname, string txtemail, int ddlaccount)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.ok;
            if (!string.IsNullOrEmpty(txtemail) && !string.IsNullOrEmpty(txtname) && ddlaccount > 0)
            {
                Resp = await OrganizerDbOperation.Addusers(txtname, txtemail, ddlaccount);
            }
            else
            {
                Resp.Msg = "Please fill neccesary detail!";
            }
            TempData["resp"] = Resp;
            return RedirectToAction("CreateUser");
        }

        public ActionResult EditUser(int UserId)
        {
            EdituserRoleModel result = new EdituserRoleModel();
            try
            {
                result.UserManageModel = OrganizerDbOperation.GetSubUser(UserId);
                result.Accounts = OrganizerDbOperation.GetAllGroups(ManageSession.CompanySession.Id);
                return PartialView("_EditUserRole", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_EditUserRole", result);
        }
        [HttpPost]
        public JsonResult UpdaetUser(int UserId, string email, int GpId, string name)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if(UserId>0 && !string.IsNullOrEmpty(email) && GpId>0 && !string.IsNullOrEmpty(name))
                {
                    resp = OrganizerDbOperation.UpdateSubUser(UserId, email, GpId, name);
                }
                else
                {
                    resp.Msg ="Invalid Entry";
                }
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteUser(int UserId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if(UserId>0)
                {
                    resp = OrganizerDbOperation.DeleteSubUser(UserId);
                }
                else
                {
                    resp.Msg = "Invalid Request!.";
                }
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                resp.Msg = ex.ToString();
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Organizer/UserManagement
        public ActionResult AccountUserPermission(int GpId)
        {
            AcccountTypeViewModel result = new AcccountTypeViewModel();
            result.Objects = OrganizerDbOperation.GetGroupObjectMap(GpId);
            result.GpId = GpId;
            result.AccountName = result.Objects.FirstOrDefault().Groupname;
            return PartialView("_EditUserPermission", result);
        }

        [HttpPost]
        public ActionResult UpdateAccountTypePost(AcccountTypeViewModel Model)
        {
            TempData["resp"] = OrganizerDbOperation.UpdateGpMappingAccount(Model);
            return RedirectToAction("AccountType");
        }

        public ActionResult ScanUser()
        {
            ScanUserManageModel result = new ScanUserManageModel();
            result.UserModel = OrganizerDbOperation.GetScanUserbyAccount(ManageSession.CompanySession.Id);
            if (TempData["resp"] != null)
            {
                result.Resp = (ApiResponse)TempData["resp"];
            }
            return View(result);
        }

        [HttpPost]
        public ActionResult ScannUser(string txtname, string txtemail, string txtPassword)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.ok;
            if (!string.IsNullOrEmpty(txtemail) && !string.IsNullOrEmpty(txtname) && !string.IsNullOrEmpty(txtPassword))
            {
                Resp = OrganizerDbOperation.AddScannusers(txtname, txtemail, txtPassword);
            }
            else
            {
                Resp.Msg = "Please fill neccesary detail!";
            }
            TempData["resp"] = Resp;
            return RedirectToAction("ScanUser");
        }

        public ActionResult EditScanUser(int UserId)
        {
            EdituserEventPermisionModel result = new EdituserEventPermisionModel();
            try
            {
                result.UserDetail = OrganizerDbOperation.GetScanUser(UserId);
                result.EventDataModel = OrganizerDbOperation.GetAllEventbyCompany(UserId);
                result.Showbutton = true;
                return PartialView("_EditScanUser", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_EditScanUser", result);
        }
        public ActionResult SelectEvent(int UserId)
        {
            EdituserEventPermisionModel result = new EdituserEventPermisionModel();
            try
            {
                result.UserDetail = OrganizerDbOperation.GetScanUser(UserId);
                result.EventDataModel = OrganizerDbOperation.GetSelectEventbyuser(UserId);
                result.Showbutton = false;
                return PartialView("_EditScanUser", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_EditScanUser", result);
        }



        [HttpPost]
        public JsonResult SaveEventPermisionUser(int id, string SelectedId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
               
                string[] values = SelectedId.Split(',');
                if (id > 0 && values.Count()>0)
                {
                    resp = OrganizerDbOperation.SaveUserEventPermission(id, values);
                }
                else
                {
                    resp.Msg = "Invalid Entry";
                }
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EditEventPermisionUser(int id, string SelectedId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {

                string[] values = SelectedId.Split(',');
                if (id > 0 && values.Count() > 0)
                {
                    resp = OrganizerDbOperation.EditUserEventPermission(id, values);
                }
                else
                {
                    resp.Msg = "Invalid Entry";
                }
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteScanUser(int id)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {

                
                if (id != 0)
                {
                    resp = OrganizerDbOperation.DeleteuserScan(id);
                }
                else
                {
                    resp.Msg = "Invalid Entry";
                }
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }
    }
}