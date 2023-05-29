using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using EventManager1.Models;
using System.Net;
using System.Collections.Generic;
using EventManager1.DBCon;
using System.Web.Security;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;

namespace EventManager1.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {

        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            TempData["returnUrl"] = returnUrl;
            return View();
        }
        [AllowAnonymous]
        public ActionResult Userlogin(string returnUrl)
        {
            LoginphoneModel ru = new LoginphoneModel();
            //if (returnUrl != null) { TempData["returnUrl"] = returnUrl; }
            ru.Country = CommonDbLogic.GetCountry();
            foreach (var item in ru.Country)
            {
                if (item.Text == "Ghana")
                { ru.CountryId = item.Id; }
            }
            ViewBag.ReturnUrl = returnUrl;
            TempData["returnUrl"] = returnUrl;

            if (ManageSession.UserSession != null && returnUrl.Contains("OnlineStreaming"))
            {
                return Redirect(returnUrl);
            }
            
            return View(ru);
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Userlogin(LoginphoneModel model, string returnUrl)
        {
            string sender = "VSPASS";
            string URL = ConfigurationManager.AppSettings["URL"];
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (returnUrl != null || returnUrl != "")
            {
                TempData["returnUrl"] = returnUrl;
            }
            else if (TempData["returnUrl"] != null)
            {
                returnUrl = TempData["returnUrl"].ToString();
                TempData["returnUrl"] = returnUrl;
            }
            if (model != null)
            {
                HandleUser hdb = new HandleUser();
                var login = hdb.userLogin(model);
                if (login != null && !string.IsNullOrEmpty(login.FirstName) && login.Status == (int)companytatus.active)
                {
                    Session["UserID"] = login.Id;
                    var OPT = hdb.CreateOTP(Convert.ToInt32(Session["UserID"]));
                    if (OPT != null)
                    {
                        string ccode = "+" + login.PhoneCode;
                        string PhoneReciever = "";
                        if (model.Phone != null)
                        {
                            PhoneReciever = "00" + login.PhoneCode + model.Phone;
                            if (model.Phone.Length == 10 && login.PhoneCode == "233")
                            {
                                PhoneReciever = "00233" + model.Phone.Substring(1);
                            }
                            
                        }
                        string Mobile = model.Phone;

                        if (model.Phone != null)
                        {
                            Regex regexM = new Regex(@"^\d{10}$");
                            Match matchM = regexM.Match(Mobile);
                            if (matchM.Success)
                            {
                                TempData["Phone"] = Mobile;
                                TempData["Code"] = login.PhoneCode;
                                var Text = " OTP code is: " + "" + OPT + " " + "for VSPROCESSOR" + "." + "" + "This code is valid for 15 minutes.";
                                
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(login.Token);
                                HttpResponseMessage CMSmsresponse = client.GetAsync("api/TicketAPI/SendCMSms?PhoneSender=" + sender + "&PhoneReciever=" + PhoneReciever + "&Text=" + Text + "").Result;

                            }
                        }
                        string email = model.EmailId;
                        if (model.EmailId != null)
                        {
                            Regex regexE = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                            Match matchE = regexE.Match(email);
                            if (matchE.Success)
                            {
                                TempData["Email"] = email;
                                string body = string.Format(OPT + " is the your verification code to activate your access on VSPROCESSOR. This code is valid for 15 minutes.");

                                int mailer = (int)Ticketforwebsite.vsprocessor;
                                string MailfromName = "Stream233";
                                string Subject = "OTP";
                                int eventid = 0;
                                await EmailSending.SendCMEmail(email, body, MailfromName, Subject, mailer, eventid);
                            }
                        }
                                               
                    }
                    return RedirectToAction("VerifyOTP", "Account", new { Area = "", returnUrl = returnUrl });
                   
                }
                else
                {
                    ApiResponse resp = new ApiResponse();
                    RegisterUser_ user = new RegisterUser_();
                    user.UserName = model.Phone;

                    if (model.Phone != null)
                    {
                        Regex regexMM = new Regex(@"^\d{10}$");
                        Match matchMM = regexMM.Match(model.Phone);
                        if (matchMM.Success)
                        {
                            TempData["Phone"] = model.Phone;
                            TempData["Code"] = model.Phone_CountryCode;
                            user.PhoneNo = model.Phone;
                        }
                        else
                        {
                            Nullable<int> Mobile = null;
                            user.PhoneNo = Mobile.ToString();
                        }
                    }
                    if (model.Phone != null)
                    {
                        user.FirstName = model.Phone;
                    }
                    else
                    {
                        user.FirstName = model.EmailId;
                    }
                    if (model.Phone != null)
                    {
                        user.LastName = model.Phone;
                    }
                    else
                    {
                        user.LastName = model.EmailId;
                    }
                    //user.Email = model.Phone;
                    if (model.EmailId != null)
                    {
                        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                        Match match = regex.Match(model.EmailId);
                        if (match.Success)
                        {
                            user.Email = model.EmailId;
                            TempData["Email"] = model.EmailId;
                        }
                        else
                        {
                            Nullable<int> Email = null;
                            user.Email = Email.ToString();
                        }
                    }

                    user.Phone_CountryCode = model.Phone_CountryCode;
                    resp = hdb.Register(user);
                    if (resp.Code == (int)ApiResponseCode.ok)
                    {
                        Session["UserID"] = resp.Msg;
                        var OPT = hdb.CreateOTP(Convert.ToInt32(Session["UserID"]));
                        if (OPT != null)
                        {
                            string ccode = model.Phone_CountryCode;
                            string PhoneReciever = "";
                            if (model.Phone != null)
                            {
                                PhoneReciever = "00" + model.Phone_CountryCode + model.Phone;
                                if (model.Phone.Length == 10 && model.Phone_CountryCode == "233")
                                {
                                    PhoneReciever = "00233" + model.Phone.Substring(1);
                                }
                            }
                            string Mobile = model.Phone;
                            if (model.Phone != null)
                            {
                                Regex regexM = new Regex(@"^\d{10}$");
                                Match matchM = regexM.Match(Mobile);
                                if (matchM.Success)
                                {
                                    TempData["Phone"] = Mobile;
                                    TempData["Code"] = model.Phone_CountryCode;
                                    var Text = " OTP code is: " + "" + OPT + " " + "for VSPROCESSOR" + "." + "" + "This code is valid for 15 minutes.";
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(resp.Token);
                                    HttpResponseMessage CMSmsresponse = client.GetAsync("api/TicketAPI/SendCMSms?PhoneSender= " + sender + "&PhoneReciever=" + PhoneReciever + "&Text=" + Text + "").Result;


                                }
                            }
                            string email = model.EmailId;
                            if (model.EmailId != null)
                            {
                                Regex regexE = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                                Match matchE = regexE.Match(email);
                                if (matchE.Success)
                                {
                                    string body = string.Format(OPT + " is the your verification code to activate your access on VSPROCESSOR. This code is valid for 15 minutes.");

                                    int mailer = (int)Ticketforwebsite.Stream233;
                                    string MailfromName = "VSPASS";
                                    string Subject = "OTP";
                                    int eventid = 0;
                                    await EmailSending.SendCMEmail(email, body, MailfromName, Subject, mailer, eventid);

                                }
                            }
                        }
                    }
                    return RedirectToAction("VerifyOTP", "Account", new { Area = "", returnUrl = returnUrl });
                }
            }
            return View();
            
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Userloginpop(LoginphoneModel2 models)
        {
            string sender = "VSPASS";
            string URL = ConfigurationManager.AppSettings["URL"];
            HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://localhost:62113/");
            client.BaseAddress = new Uri(URL);

            LoginphoneModel model = new LoginphoneModel();
            model.CountryId = Convert.ToInt32(models.CountryId);
            model.Phone = models.Phone;
            model.EmailId = models.EmailId;

            if (model != null && (model.EmailId != null || model.Phone != null))
            {
                HandleUser hdb = new HandleUser();

                var login = hdb.userLogin(model);
                if (login != null && !string.IsNullOrEmpty(login.FirstName) && login.Status == (int)companytatus.active)
                {
                    Session["UserID"] = login.Id;
                    var OPT = hdb.CreateOTP(Convert.ToInt32(Session["UserID"]));
                    if (OPT != null)
                    {
                        string ccode = "+" + login.PhoneCode;
                        string PhoneReciever = "";
                        if (model.Phone != null)
                        {
                            PhoneReciever = "00" + login.PhoneCode + model.Phone;
                            if (model.Phone.Length == 10 && login.PhoneCode == "233")
                            {
                                PhoneReciever = "00233" + model.Phone.Substring(1);
                            }
                            else if (model.Phone.Length == 9 && model.Phone_CountryCode == "233")
                            {
                                PhoneReciever = "00233" + model.Phone;
                            }

                        }
                        string Mobile = model.Phone;

                        if (model.Phone != null)
                        {
                            Regex regexM = new Regex(@"^\d{10}$");
                            Match matchM = regexM.Match(Mobile);
                            if (matchM.Success)
                            {
                                TempData["Phone"] = Mobile;
                                TempData["Code"] = login.PhoneCode;
                                var Text = "OTP code is: " + "" + OPT + " " + "for VSPASS" + "." + "" + " Valid for 15 minutes.";
                                //var smssendd = CommonSMSCallF.SendSMSHubtel(Text, PhoneReciever);
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(login.Token);
                                var text = "api/TicketAPI/SendCMSms?PhoneSender=" + sender + "&PhoneReciever=" + PhoneReciever + "&Text=" + Text + "";
                                HttpResponseMessage CMSmsresponse = client.GetAsync(text).Result;

                            }
                        }
                        string email = model.EmailId;
                        if (model.EmailId != null)
                        {
                            Regex regexE = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                            Match matchE = regexE.Match(email);
                            if (matchE.Success)
                            {
                                TempData["Email"] = email;
                                string body = string.Format(OPT + " is your verification code to activate your access on VSPASS. This code is valid for 15 minutes.");

                                int mailer = (int)Ticketforwebsite.vsprocessor;
                                string MailfromName = "VSPASS";
                                string Subject = "OTP";
                                int eventid = 0;
                                //EmailSending.SendEmail(email, body, "Stream233", "OTP");
                                //HttpResponseMessage Emailresponse = client.GetAsync("api/TicketAPI/SendLoginRegistrationEmail?email= " + email + "&body=" + body + "&MailfromName=" + MailfromName + "&Subject=" + Subject + "&mailer=" + mailer + "&eventid=" + eventid + "").Result;
                                await EmailSending.SendCMEmail(email, body, MailfromName, Subject, mailer, eventid);
                            }
                        }
                    }
                    return Json(true);
                }
                else
                {
                    ApiResponse resp = new ApiResponse();
                    RegisterUser_ user = new RegisterUser_();
                    user.UserName = model.Phone;
                    //user.PhoneNo = model.Phone;

                    if (model.Phone != null)
                    {
                        Regex regexMM = new Regex(@"^\d{10}$");
                        Match matchMM = regexMM.Match(model.Phone);
                        if (matchMM.Success)
                        {
                            TempData["Phone"] = model.Phone;
                            TempData["Code"] = model.CountryId.ToString();
                            user.PhoneNo = model.Phone;
                        }
                        else
                        {
                            Nullable<int> Mobile = null;
                            user.PhoneNo = Mobile.ToString();
                        }
                    }
                    if (model.Phone != null)
                    {
                        user.FirstName = model.Phone;
                    }
                    else
                    {
                        user.FirstName = model.EmailId;
                    }
                    if (model.Phone != null)
                    {
                        user.LastName = model.Phone;
                    }
                    else
                    {
                        user.LastName = model.EmailId;
                    }
                    //user.Email = model.Phone;
                    if (model.EmailId != null)
                    {
                        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                        Match match = regex.Match(model.EmailId);
                        if (match.Success)
                        {
                            user.Email = model.EmailId;
                            TempData["Email"] = model.EmailId;
                        }
                        else
                        {
                            Nullable<int> Email = null;
                            user.Email = Email.ToString();
                        }
                    }

                    user.Phone_CountryCode = model.CountryId.ToString();
                    resp = hdb.Register(user);
                    if (resp.Code == (int)ApiResponseCode.ok)
                    {
                        Session["UserID"] = resp.Msg;
                        var OPT = hdb.CreateOTP(Convert.ToInt32(Session["UserID"]));
                        if (OPT != null)
                        {
                            string ccode = model.CountryId.ToString();
                            string PhoneReciever = "";
                            if (model.Phone != null)
                            {
                                PhoneReciever = "00" + model.Phone_CountryCode + model.Phone;
                                if (model.Phone.Length == 10 && model.CountryId.ToString() == "233")
                                {
                                    PhoneReciever = "00233" + model.Phone.Substring(1);
                                }
                                else if (model.Phone.Length == 9 && model.CountryId.ToString() == "233")
                                {
                                    PhoneReciever = "00233" + model.Phone;
                                }

                            }
                            string Mobile = model.Phone;
                            if (model.Phone != null)
                            {
                                Regex regexM = new Regex(@"^\d{10}$");
                                Match matchM = regexM.Match(Mobile);
                                if (matchM.Success)
                                {
                                    TempData["Phone"] = Mobile;
                                    TempData["Code"] = model.Phone_CountryCode;
                                    var Text = " OTP code is: " + "" + OPT + " " + "for VSPROCESSOR" + "." + "" + " Valid for 15 minutes.";
                                    //var smssendd = CommonSMSCallF.SendSMSHubtel(Text, PhoneReciever);
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(resp.Token);
                                    HttpResponseMessage CMSmsresponse = client.GetAsync("api/TicketAPI/SendCMSms?PhoneSender= " + sender + "&PhoneReciever=" + PhoneReciever + "&Text=" + Text + "").Result;


                                }
                            }
                            string email = model.EmailId;
                            if (model.EmailId != null)
                            {
                                Regex regexE = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                                Match matchE = regexE.Match(email);
                                if (matchE.Success)
                                {
                                    string body = string.Format(OPT + " is your verification code to activate your access on VSPROCESSOR. This code is valid for 15 minutes.");

                                    int mailer = (int)Ticketforwebsite.vsprocessor;
                                    string MailfromName = "VSPASS";
                                    string Subject = "OTP";
                                    int eventid = 0;
                                    //EmailSending.SendEmail(email, body, "Stream233", "OTP");
                                    //HttpResponseMessage Emailresponse = client.GetAsync("api/TicketAPI/SendLoginRegistrationEmail?email= " + email + "&body=" + body + "&MailfromName=" + MailfromName + "&Subject=" + Subject + "&mailer=" + mailer + "&eventid=" + eventid + "").Result;
                                    await EmailSending.SendCMEmail(email, body, MailfromName, Subject, mailer, eventid);

                                }
                            }

                        }

                        return Json(true);
                    }
                    return Json(false);
                }
            }
            return Json(false);

        }
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            if (model != null)
            {
                HandleUser hdb = new HandleUser();
                var login = hdb.Login(model);
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
                    if (returnUrl != "")
                    {
                        return RedirectToAction("EventDetail", "Home", new { Id = returnUrl });

                    }
                    else { return RedirectToAction("Index", "Home", new { Area = "" }); }
                }
                else if (login.Status == (int)companytatus.block) { ViewBag.message = "Your Account is blocked. Please contact admin for activation"; }

                else
                {
                    ViewBag.message = "User name or password is invalid.";

                }

            }

            return View();

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login1(LoginViewModel1 Loginmodel)
        {
            ApiResponse result = new ApiResponse();
            if (Loginmodel != null)
            {
                LoginViewModel m = new LoginViewModel();
                m.Email = Loginmodel.Email; m.Password = Loginmodel.Password;
                HandleUser hdb = new HandleUser();
                var login = hdb.Login(m);
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

                    // return RedirectToAction("EventDetail", "Home", new { Id = returnUrl });

                    result.Code = (int)ApiResponseCode.ok;
                    result.Msg = login.FirstName;
                    return new JsonResult { Data = new { result } };


                }
                else if (login.Status == (int)companytatus.block)
                {
                    result.Code = (int)ApiResponseCode.fail;
                    result.Msg = "Your Account is blocked. Please contact admin for activation";
                }
                else
                {
                    result.Code = (int)ApiResponseCode.fail;
                    //result.Msg = "Your Account is blocked. Please contact admin for activation";
                    ViewBag.msg = "User name or password is invalid.";

                }

            }

            return View();
            //}
        }

        [AllowAnonymous]
        public ActionResult OrganizerLogin()
        {
            if (TempData["Success"] != null) { ViewBag.msg = "Great! Your Registration was successful."; }
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult OrganizerLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model != null)
            {
                HandleUser hdb = new HandleUser();
                var login = hdb.OrganizerLogin(model);
                if (login != null && !string.IsNullOrEmpty(login.EmailId) && login.status == (int)companytatus.active)
                {
                    HttpCookie userInfo = new HttpCookie("userInfo");
                    userInfo["UserName"] = login.Id.ToString();
                    userInfo["type"] = "1";
                    userInfo.Expires.Add(new TimeSpan(0, 1, 0));
                    userInfo.Domain = "stream233.com";
                    Response.Cookies.Add(userInfo);
                    TempData["temp"] = login;
                    ManageSession.CompanySession = new CompanySession();
                    ManageSession.CompanySession.Id = login.Id;
                    ManageSession.CompanySession.EmailId = login.EmailId;
                    ManageSession.CompanySession.FirstName = login.FirstName;
                    ManageSession.CompanySession.LastName = login.LastName;
                    ManageSession.CompanySession.CompName = login.CompName;
                    if (login.CompanyId == 0)
                    {

                        ManageSession.CompanySession.CompanyId = login.Id;
                    }
                    else
                    {
                        ManageSession.CompanySession.CompanyId = login.CompanyId;
                    }

                    return RedirectToAction("Dashboard", "Organizer", new { area = "Organizer" });
                }
                else if (login.status == (int)companytatus.block) { ViewBag.message = "Your Account is blocked. Please contact admin for activation"; }
                else { ViewBag.message = "User name or password is invalid."; }
            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult UserRegister(string returnUrl)
        {
            if (TempData["returnUrl"] != null)
            {
                TempData["returnUrl"] = TempData["returnUrl"];
            }
            Users_Heave ru = new Users_Heave();
            return View(ru);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult UserRegister(Users_Heave obj, string returnUrl)
        {
            int EventId = 0;
            if (TempData["returnUrl"] != null)
            {
                TempData["returnUrl"] = TempData["returnUrl"];
                EventId = Convert.ToInt32(TempData["returnUrl"]);
            }
            HandleUser hdb = new HandleUser();
            var register = hdb.UserRegister_heave(obj);
            return RedirectToAction("Summary", "ev", new { Id = EventId });
            //return Json(EventId,JsonRequestBehavior.AllowGet);
        }
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            if (TempData["returnUrl"] != null)
            {
                TempData["returnUrl"] = TempData["returnUrl"];
            }
            RegisterUser_ ru = new RegisterUser_();
            //if (returnUrl != null) { TempData["returnUrl"] = returnUrl; }
            ru.Country = CommonDbLogic.GetCountry();
            foreach (var item in ru.Country)
            {
                if (item.Text == "Ghana")
                { ru.CountryId = item.Id; }
            }
            if (TempData["register"] != null) { ru = TempData["register"] as RegisterUser_; }
            return View(ru);
        }
        [AllowAnonymous]
        public ActionResult Signup(string returnUrl)
        {
            if (TempData["returnUrl"] != null)
            {
                TempData["returnUrl"] = TempData["returnUrl"];
            }
            RegisterUser_ ru = new RegisterUser_();
            //if (returnUrl != null) { TempData["returnUrl"] = returnUrl; }
            ru.Country = CommonDbLogic.GetCountry();
            foreach (var item in ru.Country)
            {
                if (item.Text == "Ghana")
                { ru.CountryId = item.Id; }
            }
            if (TempData["register"] != null) { ru = TempData["register"] as RegisterUser_; }
            return View(ru);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Signup(RegisterUser_ model)
        {
            // model.CountryId = strDDLValue;
            //model.PhoneNo = model.phoneNumber;
            ApiResponse resp = new ApiResponse();
            resp.Msg = "Please enter valid details.";
            if (ModelState.IsValid)
            {
                HandleUser hdb = new HandleUser();
                resp = hdb.Register(model);
                if (resp.Code == (int)ApiResponseCode.ok)
                {
                    Session["UserID"] = resp.Msg;
                    var OPT = hdb.CreateOTP(Convert.ToInt32(Session["UserID"]));
                    if (OPT != null)
                    {
                        string ccode = model.Phone_CountryCode;
                        string PhoneReciever = "";
                        if (model.PhoneNo.Length == 10 && model.Phone_CountryCode == "233")
                        {
                            PhoneReciever = "233" + model.PhoneNo.Substring(1);
                        }
                        else if (model.PhoneNo.Length == 9 && model.Phone_CountryCode == "233")
                        {
                            PhoneReciever = "233" + model.PhoneNo;
                        }
                        else if (model.Phone_CountryCode != "233")
                        {
                            PhoneReciever = "+" + model.Phone_CountryCode + model.PhoneNo;
                        }
                        var Text = OPT + " is the OTP for activate your account on VSPROCESSOR. OTP is valid for 15 min from the request.";
                        var smssendd = CommonSMSCallF.SendSMSHubtel(Text, PhoneReciever);
                        


                    }
                    return RedirectToAction("VerifyOTP", "Account", new { Area = "" });
                   

                }
            }

            model.Country = CommonDbLogic.GetCountry();
            model.Error = resp;
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult VerifyOTP(string returnUrl)
        {

            if (!String.IsNullOrEmpty(returnUrl))
            {
                ViewBag.returnUrl = returnUrl;
            }
            if (TempData["Phone"] != null)
            {
                ViewBag.Phone = TempData["Phone"];
                ViewBag.Code = TempData["Code"];
            }
            if (TempData["Email"] != null)
            {
                ViewBag.Email = TempData["Email"];
            }
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Userlogin", "Account", new { Area = "" });
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult VerifyOTP(string OTP, string Email, string returnUrl = "")
        {
            HandleUser hdb = new HandleUser();
            if (OTP != null)
            {
                var res = hdb.verifyOTP(OTP, Convert.ToInt32(Session["UserID"]));
                if (res)
                {
                    HttpCookie userInfo = new HttpCookie("userInfo");
                    userInfo["UserName"] = ManageSession.UserSession.Id.ToString();
                    userInfo["type"] = "2";
                    userInfo.Expires.Add(new TimeSpan(0, 1, 0));
                    userInfo.Domain = "VSPROCESSOR.com";
                    Response.Cookies.Add(userInfo);
                    if (String.IsNullOrEmpty(returnUrl))
                    {
                        if (TempData["returnUrl"] != null)
                        {
                            returnUrl = TempData["returnUrl"].ToString();
                        }
                    }


                    if (String.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "GrabTicket", new { Area = "" });
                    }
                    else
                    {
                        TempData["returnUrl"] = returnUrl;
                        if (returnUrl.Contains("OnlineStreaming"))
                        {
                            return Redirect(returnUrl);
                        }

                        string URL = "";
                        var tid = Convert.ToInt32(returnUrl);
                        if (tid > 0)
                        {
                            HandleEvent ev = new HandleEvent();
                            var tickets = ev.GetUserTickets(ManageSession.UserSession.Id);
                            Event_ ticket = new Event_();
                            foreach (var i in tickets)
                            {
                                if (i.Tickets.FirstOrDefault(y => y.Id == tid) != null)
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

                                    URL = "/OnlineStreaming/Index/?ev=" + ticket.evid;
                                }
                                else
                                {
                                    URL = "/User/User/Tickets?id=" + ticket.Tickets.FirstOrDefault().TmapID + "&tmap=" + ticket.Tickets.FirstOrDefault().TmapID;
                                }
                            }

                        }
                        if (URL != "")
                        {
                            return Redirect(URL);
                        }
                        else
                        {
                            return RedirectToAction("Index", "GrabTicket", new { Area = "" });
                        }
                        
                    }
                }
                else
                {
                    ViewBag.Msg = "Please Enter a valid OTP.";
                }
            }
            else
            {
                ViewBag.Msg = "Please Enter a valid OTP.";
            }
            return View();
        }
        common cdb = new common();
        [HttpPost]
        [AllowAnonymous]
        public ActionResult VerifyOTPpop(string OTP)
        {
            ApiResponse res = new ApiResponse();
            res.Code = (int)ApiResponseCode.fail; res.Msg = "Please Enter a valid OTP.";
            HandleUser hdb = new HandleUser(); int user = 0;
            if (Session["UserID"] != null)
            {
                user = Convert.ToInt32(Session["UserID"]);
            }
            if (OTP != null && user > 0)
            {
                var resp = hdb.verifyOTP(OTP, user);
                if (resp)
                {
                    //HttpCookie userInfo = new HttpCookie("userInfo");
                    //userInfo["UserName"] = ManageSession.UserSession.Id.ToString();
                    //userInfo["type"] = "2";
                    //userInfo.Expires.Add(new TimeSpan(0, 1, 0));
                    //userInfo.Domain = "Stream233.com";
                    //Response.Cookies.Add(userInfo);      
                    res.Code = (int)ApiResponseCode.ok; res.Msg = "successful.";
                }

            }
            return Json(res);
        }
        [AllowAnonymous]
        public ActionResult OrganizerRegister()
        {
            try
            {
                List<Models.Country> ct = cdb.GetCountry();
                ViewBag.country = ct;
                foreach (var item in ct)
                {
                    if (item.Name == "Ghana")
                    { ViewBag.countryId = item.Id; }
                }
            }
            catch { }
            return View();
        }
        [AllowAnonymous]
        public ActionResult ForgetPassword()
        {
            var q = Request.QueryString["f"];
            ViewBag.type = q;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgetPassword(string email, int type = 1, string UserType = "")
        {
            if (!string.IsNullOrEmpty(UserType))
            {
                ViewBag.type = UserType;
            }

            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            result.Msg = "Invalid Model";
            RegisterOrganization m = new RegisterOrganization();
            HandleUser hdb = new HandleUser();
            result = await hdb.ForgetPasswordMail(email, type);
            ViewBag.message = result.Msg;
            return View();

        }

        [HttpPost]
        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult OrganizerRegister(RegisterOrganization Register)
        {
            var isValidModel = true; //TryUpdateModel(Register);
            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            result.Msg = "Invalid Model";
            if (isValidModel)
            {
                RegisterOrganization m = new RegisterOrganization();
                HandleUser hdb = new HandleUser();
                result = hdb.RegisterCompany(Register);
                if (result.Code == (int)ApiResponseCode.ok) { TempData["Success"] = "Success"; }
                return new JsonResult { Data = new { result } };

            }
            return new JsonResult { Data = new { result } };
        }
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterUser_ model)
        {
            // model.CountryId = strDDLValue;
            ApiResponse resp = new ApiResponse();
            if (ModelState.IsValid)
            {
                HandleUser hdb = new HandleUser();
                resp = hdb.Register(model);
                if (resp.Code == (int)ApiResponseCode.ok)
                {
                    if (TempData["returnUrl"] == null)
                    {
                        return RedirectToAction("Dashboard", "User", new { Area = "User" });
                    }
                    else { return RedirectToAction("EventDetail", "Home", new { Id = TempData["returnUrl"] }); }
                }
            }

            model.Country = CommonDbLogic.GetCountry();
            model.Error = resp;
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register1(RegisterUser_ model)
        {
            // model.CountryId = strDDLValue;
            ApiResponse result = new ApiResponse();
            HandleUser hdb = new HandleUser();
            result = hdb.Register(model);
            if (result.Code == (int)ApiResponseCode.ok)
            {
                //HttpCookie userInfo = new HttpCookie("userInfo");
                //userInfo["UserName"] = login.Id.ToString();
                //userInfo["type"] = "2";
                //userInfo.Expires.Add(new TimeSpan(0, 1, 0));
                //userInfo.Domain = "rovermanproductions.com";
                //Response.Cookies.Add(userInfo);                        
                //return new JsonResult { Data = new { resp } };

            }

            return new JsonResult { Data = new { result } };
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {

            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            HandleUser hdb = new HandleUser();
            UserSession Isuser = hdb.CheckExternalLogin(loginInfo.Email);
            if (Isuser.FirstName != null && Isuser.Id > 0)
            {
                ManageSession.UserSession = new UserSession();
                ManageSession.UserSession.Id = Isuser.Id;
                ManageSession.UserSession.FirstName = Isuser.FirstName;
                ManageSession.UserSession.EmailId = Isuser.EmailId;
                if (returnUrl == null) { return RedirectToAction("Dashboard", "User", new { area = "user" }); }

                else { return RedirectToAction("Summery", "Payment", new { EventId = returnUrl }); }
            }
            else
            {
                RegisterUser_ u = new RegisterUser_();
                u.Email = loginInfo.Email;
                u.Country = CommonDbLogic.GetCountry();
                var Name = loginInfo.ExternalIdentity.Name;
                var firstSpaceIndex = Name.IndexOf(" ");
                u.FirstName = Name.Substring(0, firstSpaceIndex);
                u.LastName = Name.Substring(firstSpaceIndex + 1);
                TempData["register"] = u;
                return RedirectToAction("Register", "Account", new { ReturnUrl = returnUrl });
                //int res = hdb.Register(u);
            }


        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult AdminLogin()
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [AllowAnonymous]
        public ActionResult AdminLogout()
        {
            //ViewBag.ReturnUrl = returnUrl;
            ManageSession.AdminSession = null;
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AdminLogin(LoginViewModel model)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            if (model != null && !string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Password))
            {
                HandleUser hdb = new HandleUser();
                var login = hdb.AdminLogin(model);
                if (login != null && !string.IsNullOrEmpty(login.FirstName))
                {
                    ManageSession.AdminSession = new UserSession();
                    ManageSession.AdminSession.FirstName = login.FirstName;
                    ManageSession.AdminSession.LastName = login.LastName;
                    ManageSession.AdminSession.EmailId = login.EmailId;
                    ManageSession.AdminSession.Id = login.Id;
                    return RedirectToAction("Dashboard", "Admin", new { Area = "Admin" });
                }
                else
                {
                    ViewBag.message = "User name or password is invalid.";
                }
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult Redirect()
        {
            if (ManageSession.CompanySession != null && !string.IsNullOrEmpty(ManageSession.CompanySession.EmailId))
            {
                return RedirectToAction("CreateEvent", "Organizer", new { area = "Organizer" });
            }
            else if (ManageSession.UserSession != null && !string.IsNullOrEmpty(ManageSession.UserSession.EmailId))
            {
                return RedirectToAction("CreateEvent", "User", new { area = "User" });
            }
            else
            {
                return RedirectToAction("CreateEvent", "Organizer", new { area = "Organizer" });
            }

        }

        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}