using EventManager1.Areas.Organizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManager1.Models
{
    public class ManageSession
    {
        public static UserSession UserSession
        {
            get
            {
                if (HttpContext.Current.Session[ManageSessionVariable.UserSession] == null)
                    return null;
                else
                    return (UserSession)(HttpContext.Current.Session[ManageSessionVariable.UserSession]);
            }
            set
            {
                HttpContext.Current.Session[ManageSessionVariable.UserSession] = value;
            }
        }

        public static UserSession AdminSession
        {
            get
            {
                if (HttpContext.Current.Session[ManageSessionVariable.AdminSession] == null)
                    return null;
                else
                    return (UserSession)(HttpContext.Current.Session[ManageSessionVariable.AdminSession]);
            }
            set
            {
                HttpContext.Current.Session[ManageSessionVariable.AdminSession] = value;
            }
        }
        public static CompanySession CompanySession
        {
            get
            {
                if (HttpContext.Current.Session[ManageSessionVariable.CompanySession] == null)
                    return null;
                else
                    return (CompanySession)(HttpContext.Current.Session[ManageSessionVariable.CompanySession]);
            }
            set
            {
                HttpContext.Current.Session[ManageSessionVariable.CompanySession] = value;
            }
        }
        public static List<InvitationDetail> ExcelDataSession
        {
            get
            {
                if (HttpContext.Current.Session[ManageSessionVariable.ExcelDataSession] == null)
                    return null;
                else
                    return (List<InvitationDetail>)(HttpContext.Current.Session[ManageSessionVariable.ExcelDataSession]);
            }
            set
            {
                HttpContext.Current.Session[ManageSessionVariable.ExcelDataSession] = value;
            }
        }
        public static List<EmailList> EmailDataSession
        {
            get
            {
                if (HttpContext.Current.Session[ManageSessionVariable.EmailDataSession] == null)
                    return null;
                else
                    return (List<EmailList>)(HttpContext.Current.Session[ManageSessionVariable.EmailDataSession]);
            }
            set
            {
                HttpContext.Current.Session[ManageSessionVariable.EmailDataSession] = value;
            }
        }
        public static TicketCartSession TicketCartSession
        {
            get
            {
                if (HttpContext.Current.Session[ManageSessionVariable.TicketCartSession] == null)
                    return null;
                else
                    return (TicketCartSession)(HttpContext.Current.Session[ManageSessionVariable.TicketCartSession]);
            }
            set
            {
                HttpContext.Current.Session[ManageSessionVariable.TicketCartSession] = value;
            }
        }
        public static SubscribeCartSession SubscribeCartSession
        {
            get
            {
                if (HttpContext.Current.Session[ManageSessionVariable.SubscribeCartSession] == null)
                    return null;
                else
                    return (SubscribeCartSession)(HttpContext.Current.Session[ManageSessionVariable.SubscribeCartSession]);
            }
            set
            {
                HttpContext.Current.Session[ManageSessionVariable.SubscribeCartSession] = value;
            }
        }
    }
    public class EmailList
    {
        public string EmailAddress { get; set; }
        public string Mobile { get; set; } 
        public string Status { get; set; }       
    }
    public class UserSession
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Status { get; set; }
        public int? country { get; set; }
        public string PhoneNo { get; set; }
        public string PhoneCode { get; set; }
        public string Token { get; set; }
        public int Code { get; set; }
    }

    public class ExcelDataSession
    {
        public List<InvitationDetail> Invitation { get; set; }
    }
    public class ManageSessionVariable
    {
        // User Session variables
        public const string UserSession = "UserSession";
        public const string CompanySession = "CompanySession";
        public const string AdminSession = "AdminSession";
        public const string ExcelDataSession = "ExcelDataSession";
        public const string EmailDataSession = "EmailDataSession";
        public const string TicketCartSession = "TicketCartSession";
        public const string SubscribeCartSession = "SubscribeCartSession";
    }
    public class CompanySession
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompName { get; set; }
        public string website { get; set; }
        public int status { get; set; }
        public int CompanyId { get; set; }
    }

    public class TicketCartSession
    {
        public int EventId { get; set; }
        public int? Eventtype { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public List<TickeCart> TickeCarts { get; set; }
    }
    public class TickeCart
    {
        public int TicketId { get; set; }
        public string TicketName { get; set; }
        public decimal? Price { get; set; }
        public int Qnty { get; set; }
        public int OfferId { get; set; }
        public bool TicketAdded { get; set; }
        public int currencyType { get; set; }

        public int TotalTicket { get; set; }
    }
    public class SubscribeCartSession
    {
        public string page { get; set; }
        public string type { get; set; }
        public string total { get; set; }
        public string eventId { get; set; }
        public string EventName { get; set; }
    }
}