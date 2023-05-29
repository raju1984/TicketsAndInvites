using EventManager1.DBCon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace EventManager1.Models
{
    public class CommonDbLogic
    {
        public static List<Dropdownlist> GetCountry()
        {
            List<Dropdownlist> result = new List<Dropdownlist>();
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                try
                {
                    result = (from r in dbConn.Countries
                              select new Dropdownlist
                              {
                                 Id=r.Id,
                                 Text=r.Name
                              }).ToList();
                  
                }
                catch(Exception ex)
                {

                }
                return result;
            }
        }

        public static CompanyProfileModel GetCompanyProfile(int id=0, int comId=0)
        {
            CompanyProfileModel result = new CompanyProfileModel();
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                try
                {
                    result = (from r in dbConn.Companies
                              join e in dbConn.Events on r.Id equals e.Company_Id
                              where e.Id == id && e.Company_Id==comId
                              select new CompanyProfileModel
                              {
                                  website = r.website,
                                  Image = r.Logo
                              }).FirstOrDefault();

                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }

        public static List<NotificationModel> GetNotification(int? type)
        {
            List<NotificationModel> result = new List<NotificationModel>();
            int userid = 0;
            if (type == 1) { userid = ManageSession.CompanySession.Id; }
            else { userid = ManageSession.UserSession.Id; }
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                try
                {
                    result = (from r in dbConn.Notifications where r.GeneratedFor == type && r.ReceiverId == userid
                              orderby r.Id descending
                              select new NotificationModel
                              {
                                  Id = r.Id,
                                  MsgTitle = r.MsgTitle,
                                  Text = r.Text
                              }).ToList();

                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }
        public static List<NotificationModel> GetMessage(int Id,int messagesender = 0 )
        {
            List<NotificationModel> result = new List<NotificationModel>();            
            using (EventmanagerEntities dbConn = new EventmanagerEntities())
            {
                try
                {
                    if (Id > 0 ) {

                        var users = dbConn.NotificationMessages.FirstOrDefault(x => x.Id == Id);
                        string name = "";
                        if (users.UserId != null)
                        {
                            var user = dbConn.Users.FirstOrDefault(x => x.Id == users.UserId); name = user.FirstName;
                        }
                        else { var comp = dbConn.Companies.FirstOrDefault(x => x.Id == users.CompanyId).Name_of_business; name = comp; }

                        result = (from r in dbConn.NotificationMessages                             
                              where r.Id == Id || r.MsgId == Id                              
                              select new NotificationModel
                              {
                                  Id = r.Id,
                                  MsgTitle = r.MsgTitle,
                                  Text = r.MsgText,
                                  CreatedDate= r.CreatedDate,
                                  GeneratedBy = r.ReplyedBy,
                                  name = name
                              }).ToList();

                    }

                    else if (messagesender == 2)
                    {                       
                        var list   = (from re in dbConn.NotificationMessages                                      
                                      where re.MsgId == null
                                      select new NotificationModel
                                      {
                                          Id = re.Id,                                           
                                          MsgTitle = re.MsgTitle,                                          
                                          CreatedDate = re.CreatedDate,                                          
                                      }).ToList();
                        result = new List<NotificationModel>();
                      foreach (var i in list)
                        {
                            var rq  = dbConn.NotificationMessages.Where(x => x.MsgId == i.Id || x.Id== i.Id).Max(p => p.Id);
                            var r = dbConn.NotificationMessages.FirstOrDefault(x => x.Id == rq);
                            if (r.ReplyedBy == 0)
                            {
                                string name = "";
                                if (r.UserId != null)
                                {
                                    var user = dbConn.Users.FirstOrDefault(x => x.Id == r.UserId); name = user.FirstName  ; }
                                else { var comp = dbConn.Companies.FirstOrDefault(x => x.Id == r.CompanyId).Name_of_business; name = comp; }

                                result.Add(new NotificationModel {
                                    Id = i.Id,
                                    MsgTitle = i.MsgTitle,
                                    CreatedDate = i.CreatedDate,
                                    name = name
                                } );

                            }
                        }
                    }
                    else {
                        if(messagesender == 0) { 
                        result = (from r in dbConn.NotificationMessages
                                  where r.MsgId ==null && r.UserId == ManageSession.UserSession.Id
                        select new NotificationModel
                                  {
                                      Id = r.Id,
                                      MsgTitle = r.MsgTitle,                                      
                                      CreatedDate = r.CreatedDate,                                      
                                  }).ToList();
                        }
                        else
                        {
                            result = (from r in dbConn.NotificationMessages
                                      where r.MsgId == null && r.CompanyId == ManageSession.CompanySession.Id
                            select new NotificationModel
                            {
                                Id = r.Id,
                                MsgTitle = r.MsgTitle,
                                CreatedDate = r.CreatedDate,
                            }).ToList();
                        }
                    }


                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }
        public static ApiResponse AddMessage(string message,  int messagesender, int replyedby, string title = "", int msgid = 0)
        {
            // messagesender : 0-user/1-orgaizer  , replyedby : 0-user/orgaizer  and 1- Admin

            ApiResponse Resp = new ApiResponse();
            Resp.Code = (int)ApiResponseCode.fail;
            try
            {
                NotificationMessage nt = new NotificationMessage();
                if (message != null)
                {
                    using (EventmanagerEntities dbConn = new EventmanagerEntities())
                    {
                        if (msgid > 0)
                        {
                            var msg = dbConn.NotificationMessages.Where(x => x.Id == msgid).FirstOrDefault();
                            if (msg != null)
                            {
                                nt.MsgId = msgid;
                                nt.MsgText = message;
                                if (messagesender == 0)
                                {
                                    nt.UserId = ManageSession.UserSession.Id;
                                }
                                else if(messagesender == 1) { nt.CompanyId = ManageSession.CompanySession.Id; }
                                nt.ReplyedBy = replyedby;
                                nt.CreatedDate = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            nt.MsgTitle = title;
                            nt.MsgText = message;
                            if (messagesender == 0)
                            {
                                nt.UserId = ManageSession.UserSession.Id;
                                
                            }
                            else { nt.CompanyId = ManageSession.CompanySession.Id; }
                            nt.ReplyedBy = replyedby;
                            nt.CreatedDate = DateTime.UtcNow;
                        }

                        dbConn.NotificationMessages.Add(nt);
                        if (dbConn.SaveChanges() > 0)
                        {
                            Resp.Code = (int)ApiResponseCode.ok;
                        }
                        else { Resp.Code = (int)ApiResponseCode.fail; Resp.Msg = "Server Error!"; }

                    }
                }
                else
                {
                    Resp.Code = (int)ApiResponseCode.fail;
                    Resp.Msg = "Notification Message is Required!";
                }
                Resp.Msg = "Notification sent successfully";
                return Resp;
            }
            catch (Exception ex)
            {
                Resp.Msg = ex.Message;
            }
            return Resp;
        }
        public static List<WithdrawModel> Getwithdrawaldetails(int eventId)
        {
            List<WithdrawModel> result = new List<WithdrawModel>();
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var etickets = dbConn.EventTickets.Where(x => x.Event_Id == eventId);
                    var adminfee = dbConn.PaymentSetups.FirstOrDefault().Adminfee;
                    foreach (var i in etickets)
                    {

                        var tic = dbConn.TickeUserMaps.Include(y=>y.Payment).Where(x => x.TicketId == i.Id && (x.Status == 2 || x.Status == 1) && x.Qty > 0).ToList();
                        var amt = tic.Select(x=>x.Payment).Sum(y => y.Amount);
                        var qty = tic.Sum(y => y.Qty);
                        var totalAmount = amt ;                        
                        if (amt > 0)
                        {
                            decimal? WithdrawAmount = totalAmount - (totalAmount * adminfee / 100);
                            result.Add(new WithdrawModel { TotalAmount = amt, Commition = totalAmount * adminfee / 100, qty = qty, Totalqty = i.Quantity, TicketType = dbConn.TicketTypes.FirstOrDefault(x => x.id == i.Ticket_Type).TicketTypes, AmountCreadited = WithdrawAmount });
                            totalAmount = 0;
                        }
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static WithdrawalModel Getwithdrawdetail(int reqId)
        {
            WithdrawalModel result = new WithdrawalModel();            
            try
            {
                using (EventmanagerEntities dbConn = new EventmanagerEntities())
                {
                    var compId = dbConn.WithdrawReqs.FirstOrDefault(s => s.Id == reqId).CompanyId;
                    var res = (from e in dbConn.WithdrawReqs where e.Id == reqId select new WithdrawModels {
                        Id = e.Id, Amount = e.RequestAmount, ReqDate = e.RequestDate
                    }).ToList();
                   var bank =from e in dbConn.BankDetalMappings
                                join bk in dbConn.BankDetails on e.BankDetail_Id equals bk.Id                                
                                 where e.Company_Id == compId
                                select new BankDetaill
                                {
                                     mobile_money_UniqueId = e.Mobile_Money_Unique,
                                     Account_Number = e.AccountNumber,
                                     Account_Holder_Name = e.AccountHolder,
                                     Bank_Registration_Number = e.BankRegNo,
                                     BankName = bk.BankName
                                 };
                    result.Withdraw = res;
                    result.bank = bank.FirstOrDefault();


                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
    public class NotificationModel
    {
        public int Id { get; set; }
        public Nullable<int> ReceiverId { get; set; }
        public Nullable<int> GeneratedBy { get; set; }
        public Nullable<int> GeneratedFor { get; set; }
        public string Text { get; set; }
        public string name { get; set; }
        public string MsgTitle { get; set; }
        public Nullable<bool> Isseen { get; set; }
        public Nullable<int> MsgType { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
    public class WithdrawalModel
    {        
        public decimal? walletBalance { get; set; }
        public decimal? Availablebalance { get; set; }
        public decimal? PendingtotalReq { get; set; }        
        public List<WithdrawModels> Withdraw { get; set; }
        public BankDetaill bank { get; set; }
        public ApiResponse Resp { get; set; }
    }
    public class BankDetaill
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public int? MobileAccountId { get; set; }
        public string Account_Holder_Name { get; set; }
        public string Account_Number { get; set; }
        public string Bank_Registration_Number { get; set; }
        public string mobile_money_UniqueId { get; set; }
    }
    public class WithdrawModels
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string TransactionId { get; set; }
        public DateTime? ReqDate { get; set; }
        public DateTime? TransDate { get; set; }
        public decimal? Amount { get; set; }
        public int? Status { get; set; }
    }
    public class WithdrawModel
    {          
        public int Id { get; set; }
        public string EventName { get; set; }
        public string TransactionID { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? Status { get; set; }
        public string TicketType  { get; set; }
        public int? qty { get; set; }
        public int? Totalqty { get; set; }
        public decimal? Commition { get; set; }
        public decimal? AmountCreadited { get; set; }
        public decimal? walletBalance { get; set; }
    }

    public class CompanyProfileModel
    {
        public string Image { get; set; }
        public string website { get; set; }
    }


}

