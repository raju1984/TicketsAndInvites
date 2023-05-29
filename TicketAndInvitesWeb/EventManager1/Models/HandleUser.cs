using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventManager1.DBCon;
using EventManager1.Areas.Organizer.Models;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EventManager1.Models
{
    public class HandleUser
    {
        EventmanagerEntities db;
        public ApiResponse Register(RegisterUser_ u)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                db = new EventmanagerEntities();
                int comp_id;
                var comp = db.Companies.Where(m => m.Business_Email_address == u.Email || m.Business_contact_number == u.PhoneNo).FirstOrDefault();
                if (string.IsNullOrEmpty(u.Email)) { u.Email = u.PhoneNo + "@worlafest.com"; }
                if (comp == null)
                {
                    var useri = db.Users.Where(m => m.Email == u.Email || m.PhoneNo == u.PhoneNo).FirstOrDefault();
                    if (useri != null) { comp_id = 0; }// comp_id = useri.Id; }
                    else { comp_id = 0; }

                }
                else { comp_id = comp.Id; }
                if (comp_id == 0)
                {
                    User users = new User();
                    if (u.PhoneNo != null)
                    {
                        users = db.Users.Where(m => m.PhoneNo == u.PhoneNo).FirstOrDefault();
                    }
                    else
                    {
                        users = db.Users.Where(m => m.Email == u.Email).FirstOrDefault();
                    }
                    if (users != null)
                    {
                        if (u.PhoneNo != "")
                        {
                            users.UserName = u.PhoneNo;
                        }
                        else
                        {
                            users.UserName = u.Email;
                        }
                        users.FirstName = u.FirstName;
                        users.LastName = u.LastName;
                        users.Email = u.Email;
                        users.CountryId = u.CountryId;
                        users.PhoneNo = u.PhoneNo;
                        users.Password = u.Password;
                        users.UserType = 1;
                        users.UserStatus = (int)companytatus.NotVerified;
                        users.CreatedDate = DateTime.Now;
                        users.Phone_CountryCode = u.Phone_CountryCode;
                        users.Token = Guid.NewGuid().ToString();
                        db.SaveChanges();
                        //ManageSession.UserSession = new UserSession();
                        //ManageSession.UserSession.Id = users.Id;
                        //ManageSession.UserSession.EmailId = users.Email;
                        //ManageSession.UserSession.FirstName = users.FirstName;
                        //ManageSession.UserSession.LastName = users.LastName;
                        Resp.Code = (int)ApiResponseCode.ok;
                        Resp.Msg = users.Id.ToString();
                        Resp.Token = users.Token;
                    }
                    else if (users == null && comp == null)
                    {
                        User user = new User();
                        //user.UserName = u.PhoneNo;
                        if (u.PhoneNo != "")
                        {
                            user.UserName = u.PhoneNo;
                        }
                        else
                        {
                            user.UserName = u.Email;
                        }
                        user.FirstName = u.FirstName;
                        user.LastName = u.LastName;
                        user.Email = u.Email;
                        user.CountryId = u.CountryId;
                        user.PhoneNo = u.PhoneNo;
                        user.Password = u.Password;
                        user.Phone_CountryCode = u.Phone_CountryCode;
                        user.UserType = 1;
                        user.UserStatus = (int)companytatus.NotVerified;
                        user.CreatedDate = DateTime.Now;
                        user.Token = Guid.NewGuid().ToString();
                        db.Users.Add(user);

                        // var res = db.Users.Where(m => m.Email == u.Email).FirstOrDefault();

                        if (db.SaveChanges() > 0)
                        {
                            //ManageSession.UserSession = new UserSession();
                            //ManageSession.UserSession.Id = user.Id;
                            //ManageSession.UserSession.EmailId = user.Email;
                            //ManageSession.UserSession.FirstName = user.FirstName;
                            //ManageSession.UserSession.LastName = user.LastName;                            
                            Resp.Code = (int)ApiResponseCode.ok;
                            Resp.Msg = user.Id.ToString();
                            Resp.Token = user.Token;
                        }
                        else
                        {
                            Resp.Msg = "Something went wrong!";
                        }
                    }
                }
                else
                {
                    Resp.Msg = "Email/Contact no. already exist";
                }
            }
            catch (Exception ex)
            {
                Resp.Msg = ex.Message;
            }
            return Resp;
        }
        public string CreateOTP(int userid)
        {
            try
            {
                var OTP = common.AlphanumbericNumber();
                using (EventmanagerEntities dbconnn = new EventmanagerEntities())
                {
                    var date = DateTime.UtcNow.AddMinutes(-15);
                    var ott = dbconnn.OTPVerifications.Where(x => x.created < date).ToList();
                    if(ott !=null && ott.Count>0)
                    {
                        foreach (var ottp in ott)
                        {
                            dbconnn.OTPVerifications.Remove(ottp);
                        }
                    }                    
                    OTPVerification ot = new OTPVerification();
                    ot.OTP = OTP;
                    ot.UserId = userid;
                    ot.created = DateTime.UtcNow;
                    dbconnn.OTPVerifications.Add(ot);
                    if (dbconnn.SaveChanges() > 0)
                    {
                        return OTP;
                    }                    
                }  

            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public bool verifyOTP(string OTP, int UserID)
        {
            try
            {
                using (EventmanagerEntities dbcon = new EventmanagerEntities())
                {
                    var date = DateTime.UtcNow.AddMinutes(-15);
                    var ott = dbcon.OTPVerifications.OrderByDescending(x => x.id).FirstOrDefault(x => x.UserId == UserID && x.OTP == OTP && x.created>date);
                    if(ott != null)
                    {
                        var users = dbcon.Users.Where(m => m.Id== UserID).FirstOrDefault();
                        users.UserStatus = 2;
                        dbcon.SaveChanges();
                        ManageSession.UserSession = new UserSession();
                        ManageSession.UserSession.Id = users.Id;
                        ManageSession.UserSession.EmailId = users.Email;
                        ManageSession.UserSession.FirstName = users.FirstName;
                        ManageSession.UserSession.LastName = users.LastName;
                        ManageSession.UserSession.PhoneNo = users.PhoneNo;
                        return true;
                    }                    
                }
             }
            catch (Exception ex) { }
            return false;
        }
        public int? OrganizerExist(int EventId)
        {
            int? OrganizerId = 0;
            try
            {
               
                using (EventmanagerEntities dbcon = new EventmanagerEntities())
                {

                    var ott = dbcon.Events.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Id == EventId);
                    if (ott != null)
                    {
                        OrganizerId =ott.Company_Id;
                    }
                }

            }
            catch (Exception ex) { }
            return OrganizerId;
        }
        public bool UserExit(string Email, string Phone)
        {
            try
            {

                using (EventmanagerEntities dbcon = new EventmanagerEntities())
                {
                   
                    var ott = dbcon.User_Heave.OrderByDescending(x => x.id).FirstOrDefault(x => x.Email == Email || x.PhoneNo == Phone);
                    if (ott != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                }

            }
            catch (Exception ex) { }
            return false;
        }

        public ApiResponse UserRegister_heave(Users_Heave u)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                db = new EventmanagerEntities();
                User_Heave r = new User_Heave(); bool isnew = false;
                r = db.User_Heave.Where(m => m.user_id == ManageSession.UserSession.Id).FirstOrDefault();
                if (r == null)
                {
                    isnew = true; r = new User_Heave();
                }
                r.Name = u.Name;
                r.WhichEvent = u.WhichEvent;
                r.Package = u.Package;
                r.Firsttimejoin = u.Firsttimejoin;
                r.PhoneNo = u.PhoneNo;
                r.Address = u.Address; 
                r.Email = u.Email;
                r.HearSource = u.HearSource;
                r.Emergency_Contact = u.Email;
                if (u.MedicalCondition=="Yes")
                {
                    r.Medical_condition = true;
                }
                else
                {
                    r.Medical_condition = false;
                }
               
                r.Above_condition = u.Above_condition;
                r.REFUND_POLICY = u.REFUND_POLICY; 
                r.Release_of_Liability = u.Release_of_Liability; 
                r.Social_Media = u.Social_Media;
                r.Payment = u.Payment;
                r.user_id = ManageSession.UserSession.Id;
                if (isnew) { db.User_Heave.Add(r); }
                if(db.SaveChanges()>0)
                {
                    Resp.Code = (int)ApiResponseCode.ok;
                }
            }
            catch (Exception ex)
            {
                Resp.Msg = "Database error.";
            }
            return Resp;
        }
        public UserSession Login(LoginViewModel u)
        {
            UserSession list = new UserSession();
            try
            {

                db = new EventmanagerEntities();
                var res = db.Users.FirstOrDefault(m => m.Password == u.Password && m.UserType==(int)UserType.users &&  (m.PhoneNo == u.Email || m.PhoneNo == u.Email) );
                if (res != null)
                {
                    list.Id = res.Id; list.FirstName = res.FirstName;list.LastName = res.LastName;list.EmailId = res.Email;list.Status = res.UserStatus;
                    return list;
                }
                else { return list; }

            }
            catch (Exception ex) { return list; }
        }
        public UserSession userLogin(LoginphoneModel u)
        {
            UserSession list = new UserSession();
            try
            {

                db = new EventmanagerEntities();
                var res = new User();
                if (u.Phone != null)
                {
                    res = db.Users.FirstOrDefault(m => m.UserType == (int)UserType.users && m.PhoneNo == u.Phone);
                }
                else
                {
                    res = db.Users.FirstOrDefault(m => m.UserType == (int)UserType.users && m.Email == u.EmailId);
                }
                //var res = db.Users.FirstOrDefault(m => m.Password == u.Password && m.UserType == (int)UserType.users && (m.PhoneNo == u.Phone || m.Email == u.Phone));
                if (res != null)
                {
                    res.Token = Guid.NewGuid().ToString();
                    db.SaveChanges();
                    list.Id = res.Id; list.FirstName = res.FirstName; list.LastName = res.LastName; list.EmailId = res.Email; list.country = res.CountryId; list.Status = res.UserStatus; list.PhoneNo = res.PhoneNo; list.PhoneCode = res.Phone_CountryCode != null ? res.Phone_CountryCode : "233"; list.Token = res.Token;
                    return list;
                }
                else { return list; }

            }
            catch (Exception ex) { return list; }
        }
        public UserSession LoginUserDashboardForOrganizer(string UserId)
        {
            UserSession list = new UserSession();
            try
            {

                db = new EventmanagerEntities();
                var res = db.Users.FirstOrDefault(m => m.UserType == (int)UserType.users && (m.PhoneNo == UserId || m.Email == UserId));
                if (res != null)
                {
                    list.Id = res.Id; list.FirstName = res.FirstName; list.LastName = res.LastName; list.EmailId = res.Email; list.country = res.CountryId; list.Status = res.UserStatus; list.PhoneNo = res.PhoneNo; list.PhoneCode = res.Phone_CountryCode != null ? res.Phone_CountryCode : "233";
                    return list;
                }
                else { return list; }

            }
            catch (Exception ex) { return list; }
        }
        public UserSession AdminLogin(LoginViewModel u)
        {
            UserSession list = new UserSession();
            try
            {

                db = new EventmanagerEntities();
                var res = db.Users.FirstOrDefault(m => m.Password == u.Password && m.UserType == (int)UserType.admin && (m.Email == u.Email || m.PhoneNo == u.Email));
                if (res != null)
                {
                    list.Id = res.Id; list.FirstName = res.FirstName; list.LastName = res.LastName; list.EmailId = res.Email;
                    return list;
                }
                else { return list; }

            }
            catch (Exception ex) { return list; }
        }
        public UserSession CheckExternalLogin(string email)
        {
            UserSession list = new UserSession();
            try
            {

                db = new EventmanagerEntities();
                var res = db.Users.FirstOrDefault(m => m.Email == email);
                if (res != null)
                {
                    list.Id = res.Id; list.FirstName = res.FirstName;list.EmailId = res.Email;
                    return list;
                }
                else { return list; }

            }
            catch (Exception ex) { return list; }

        }
        public CompanySession OrganizerLogin(LoginViewModel u)
        {
            CompanySession CompanySession = new CompanySession();
            try
            {
                db = new EventmanagerEntities();
                var res = db.Companies.FirstOrDefault(m => m.Business_contact_number == u.Email || m.Business_Email_address == u.Email && m.Password == u.Password );                
                
                if (res != null)
                {
                    
                   //&& m.Status==(int)companytatus.active
                    CompanySession.Id = res.Id;
                    CompanySession.CompName = res.Name_of_business;
                    CompanySession.EmailId = res.Business_Email_address;
                    CompanySession.FirstName = "--";
                    CompanySession.LastName = "--";
                    if (res.Address_Id != null && res.Address_Id > 0)
                    {
                        res = db.Companies.Include("BusinessOwners").FirstOrDefault(m => m.UserName == u.Email || m.Business_Email_address == u.Email && m.Password == u.Password); CompanySession.FirstName = res.BusinessOwners.FirstOrDefault().FirstName;
                        CompanySession.LastName = res.BusinessOwners.FirstOrDefault().LastName;
                    }
                    CompanySession.status = res.Status;
                    CompanySession.CompanyId = res.Parent_Id == null ? 0 : res.Parent_Id.Value;

                    return CompanySession;
                }
                else { return CompanySession; }

            }
            catch (Exception ex) { return CompanySession; }
        }
        public ApiResponse RegisterCompany(RegisterOrganization u)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                db = new EventmanagerEntities();
                Company user = new Company();
                user.Name_of_business = u.BusinessName;
                user.Business_Email_address = u.Email;
                user.Country_Id = u.CountryId;
                user.Business_contact_number = u.ContactNo;
                user.Password = u.Password;
                user.UserName = u.UserName;
                user.Status = (int)companytatus.active;
                user.Created_at = DateTime.Now;
                int comp_id;
                var comp = db.Companies.Where(m => m.Business_Email_address == u.Email || m.Business_contact_number == u.ContactNo).FirstOrDefault() ;                
                if (comp == null ) {
                    var useri = db.Users.Where(m => m.Email == u.Email || m.PhoneNo == u.ContactNo).FirstOrDefault();
                    if(useri != null) { comp_id = useri.Id; }
                    else { comp_id = 0; }
                }
                else { comp_id = comp.Id; }
                if (comp_id ==0 )
                {
                    Address addr = new Address();
                    addr.AddressLine = u.Address;
                    addr.AddressType = 2;
                    addr.City = u.City;
                    addr.Country_Id = u.CountryId;
                    addr.ZipCode = u.ZipCode;
                    user.Address = addr;
                    foreach (var i in u.compOwners)
                    {
                        BusinessOwner b = new BusinessOwner();
                        b.FirstName = i.FirstName;
                        b.LastName = i.LastName;
                        b.MobileNumber = i.PhoneNo;
                        b.Status = 1;
                        b.Created_at = DateTime.Now;
                        user.BusinessOwners.Add(b);
                    }
                    db.Companies.Add(user);
                    if (db.SaveChanges() > 0)
                    {
                        OrganizerDbOperation.AddDefaultAccount(user.Id);
                        Resp.Code = (int)ApiResponseCode.ok;                        
                    }
                }
                else
                {
                    Resp.Msg = "Your email/contact no. already exists!";
                }

            }
            catch (Exception ex)
            {
                Resp.Msg = ex.Message;
            }
            return Resp;
        }
        //public ApiResponse ForgetPasswordMail(string txtemail, int type)
        //{
        //    ApiResponse Resp = new ApiResponse();
        //    Resp.Code = (int)ApiResponseCode.fail;
        //    try
        //    {
        //        using (EventmanagerEntities dbConn = new EventmanagerEntities())
        //        {
        //            User existinguser = new User();
        //            Company existingcom = new Company();
        //            if (type == 1) { 
        //            existinguser = dbConn.Users.Where(a => a.Email == txtemail).FirstOrDefault();
        //            }
        //            else { existingcom = dbConn.Companies.Include("BusinessOwners").Where(a => a.Business_Email_address == txtemail).FirstOrDefault(); }
        //            if (existinguser.Email != null )
        //            {
        //                Random generator = new Random();
        //                string password = generator.Next(0, 999999).ToString("D6");
        //                existinguser.Password = password;                        
        //                Resp.Msg = "New password sent to your email successfully!";
        //                dbConn.SaveChanges();

        //                string body = string.Format("Hi {0},<br><br>We received a request for forget password " +
        //                    "Your account associated with this email address.<br><br>Your temporary password is:<b>{1}</b><br><br>Please open the app and sign in to your account with this temporary password. You can create a new secure password in user setting section." +
        //                    "<br><br>Sincerely,<br>{2}", existinguser.FirstName, password, "ScanNPass");
        //                EmailSending.SendEmail(txtemail, body, "Admin", "Password");
        //            }
        //            else if(existingcom.Business_Email_address != null)
        //            {
        //                Random generator = new Random();
        //                string password = generator.Next(0, 999999).ToString("D6");
        //                existingcom.Password = password;
        //                Resp.Msg = "New password sent to your email successfully!";
        //                dbConn.SaveChanges();

        //                string body = string.Format("Hi {0},<br><br>We received a request for forget password " +
        //                    "Your account associated with this email address.<br><br>Your temporary password is:<b>{1}</b><br><br>Please open the app and sign in to your account with this temporary password. You can create a new secure password in user setting section." +
        //                    "<br><br>Sincerely,<br>{2}", existingcom.BusinessOwners.FirstOrDefault().FirstName, password, "ScanNPass");
        //                EmailSending.SendEmail(txtemail, body, "Admin", "Password");
        //            }
        //            else
        //            {
        //                Resp.Msg = "Email-Id is not Found!";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Resp.Msg = "Email-Id is not Found!";
        //    }
        //    return Resp;
        //}
        public async Task<ApiResponse> ForgetPasswordMail(string txtemail, int type)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    User existinguser = new User();
                    Company existingcom = new Company();
                    existingcom = dbConn.Companies.Include("BusinessOwners").Where(a => a.Business_Email_address == txtemail || a.Business_contact_number== txtemail).FirstOrDefault();
                    //if (type == 1)
                    //{
                    //    existinguser = dbConn.Users.Where(a => a.Email == txtemail || a.PhoneNo==txtemail).FirstOrDefault();
                    //}
                    //else {
                    //    existingcom = dbConn.Companies.Include("BusinessOwners").Where(a => a.Business_Email_address == txtemail).FirstOrDefault();
                    //}
                    if (existinguser.Email != null && existinguser.Email== txtemail)
                    {
                        Random generator = new Random();
                        string password = generator.Next(0, 999999).ToString("D6");
                        existinguser.Password = password;
                        Resp.Msg = "New password sent to your email successfully!";
                        dbConn.SaveChanges();

                        string body = string.Format("Hi {0},<br><br>We received a request for forget password " +
                            "Your account associated with this email address.<br><br>Your temporary password is:<b>{1}</b><br><br>Please open the app and sign in to your account with this temporary password. You can create a new secure password in user setting section." +
                            "<br><br>Sincerely,<br>{2}", existinguser.FirstName, password, "ScanNPass");
                        await EmailSending.SendEmail(txtemail, body, "Admin", "Password",1);
                    }
                    else if (existinguser.PhoneNo != null && existinguser.PhoneNo== txtemail)
                    {
                        Random generator = new Random();
                        string password = generator.Next(0, 999999).ToString("D6");
                        existinguser.Password = password;
                        Resp.Msg = "New password sent to your number successfully!";
                        dbConn.SaveChanges();
                        var ccode = dbConn.CountryNews.Where(a => a.id == existinguser.CountryId).FirstOrDefault().phonecode;
                        string PhoneReciever = "";
                        if (txtemail.Length == 10)
                        {
                            PhoneReciever = "233" + txtemail.Substring(1);
                        }
                        else if (txtemail.Length == 9) { PhoneReciever = "233" + txtemail; }
                        var Text = "Hi, We received a request for forget password " + "Your account associated with this number.Your temporary password is:" + password + " You can create a new secure password in user setting section.";
                        //var smssendd = CommonSMSCallF.SendHubtelSMS(Text, PhoneReciever);
                        //var smssendd = CommonSMSCallF.SendSMSnew(Text, PhoneReciever);
                        var smssendd = CommonSMSCallF.SendSMSHubtel(Text, PhoneReciever);


                        //CommonSMSModal commonSMSModal = new CommonSMSModal()
                        //{
                        //    PhoneReciever = txtemail,// "+918287622372", //
                        //    Text = "Hi, We received a request for forget password " +
                        //    "Your account associated with this number.Your temporary password is:" + password + " You can create a new secure password in user setting section.",
                        //    PhoneSender = "",
                        //    Countrycode = "+" + ccode
                        //};
                       
                        //Thread threadSMS = new Thread(() => CommonSMSCallF.sendSMS(commonSMSModal));
                        //threadSMS.Start();
                    }
                    else if (existingcom.Business_Email_address != null && existingcom.Business_Email_address== txtemail)
                    {
                        Random generator = new Random();
                        string password = generator.Next(0, 999999).ToString("D6");
                        existingcom.Password = password;
                        Resp.Msg = "New password sent to your email successfully!";
                        dbConn.SaveChanges();

                        string body = string.Format("Hi {0},<br><br>We received a request for forget password " +
                            "Your account associated with this email address.<br><br>Your temporary password is:<b>{1}</b><br><br>Please open the app and sign in to your account with this temporary password. You can create a new secure password in user setting section." +
                            "<br><br>Sincerely,<br>{2}", existingcom.BusinessOwners.FirstOrDefault().FirstName, password, "ScanNPass");
                        await EmailSending.SendEmail(txtemail, body, "Admin", "Password",1);
                    }
                    else if(existingcom.Business_contact_number != null  && existingcom.Business_contact_number == txtemail)
                    {
                        Random generator = new Random();
                        string password = generator.Next(0, 999999).ToString("D6");
                        existinguser.Password = password;
                        Resp.Msg = "New password sent to your number successfully!";
                        dbConn.SaveChanges();
                        var ccode = dbConn.CountryNews.Where(a => a.id == existingcom.Country_Id).FirstOrDefault().phonecode;
                        string PhoneReciever = "";
                        if (txtemail.Length == 10 && ccode==233 || ccode == 245)
                        {
                            PhoneReciever = "00"+ ccode + txtemail.Substring(1);
                        }                        
                        else if (txtemail.Length == 9)
                        {
                            PhoneReciever = "00"+ ccode + txtemail;
                        }
                        else if(txtemail.Length == 10)
                        {
                            PhoneReciever = "00"+ ccode + txtemail;
                        }
                        var Text = "Hi, We received a request for forget password " + "Your account associated with this number.Your temporary password is:" + password + " You can create a new secure password in user setting section.";
                        //var smssendd = CommonSMSCallF.SendHubtelSMS(Text, PhoneReciever);
                        //var smssendd = CommonSMSCallF.SendSMSnew(Text, PhoneReciever);
                        //var smssendd = CommonSMSCallF.SendSMSHubtel(Text, PhoneReciever);
                        var smssendd = CommonSMSCallF.SendSMSCM(Text, PhoneReciever);
                    }
                    else
                    {
                        Resp.Msg = "Email-Id is not Found!";
                    }
                }
            }
            catch (Exception ex)
            {
                Resp.Msg = "Something went wrong!";
            }
            return Resp;
        }

    }
}