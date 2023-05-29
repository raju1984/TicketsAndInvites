using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManager1.Areas.Organizer.Models
{
    public class SubcriptionViewModel
    {
        public List<SubcriptionView> Subcription { get; set; }
        public List<Dropdownlist> Events { get; set; }
    }
    public class SubcriptionView
    {
        public string eventname { get; set; }
        public int? subcriptiontype { get; set; }
        public string status { get; set; }
    }
    public class TicketsModelView
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string Venuname { get; set; }
        public DateTime? StartDate { get; set; }
        public string TicketType { get; set; }
        public string TicketName { get; set; }
        public string Seat { get; set; }
        public string Table { get; set; }
        public string Colorarea { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public bool? isEnabled { get; set; }
    }
    public class InvitationViewModel
    {
        public List<InvitationDetail> Invitations { get; set; }
        public List<Dropdownlist> Events { get; set; }
    }
    public class InvitationDetail
    {
        public int InvitationId { get; set; }
        public string EventName { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string SeatNumber { get; set; }
        public string TableNumber { get; set; }
        public string ColorCode { get; set; }
        public int? Status { get; set; }
        public int PaymentStatus { get; set; }
        public string Remark { get; set; }
        public int SendInviteType { get; set; }
    }
    public class ExceldataModelView
    {
        public List<InvitationDetail> InvitationDetail { get; set; }
        public List<EmailList> EmailList { get; set; }
        public string EventName { get; set; }
        public DateTime? Datetime { get; set; }
        public string Venue { get; set; }
        public List<Dropdownlist> Events { get; set; }
        public string TotalAmount { get; set; }

    }

    public class OverviewModel
    {
        public string TickeType { get; set; }
        public string TicketName { get; set; }
        public int? SoldTicket { get; set; }
        public int? Quantity { get; set; }
        public decimal? TicketPrice { get; set; }
        public decimal EventRevenue { get; set; }
    }
    public class OverviewEventModel
    {
        public int Id { get; set; }
        public DateTime? Startdate { get; set; }
        public string EventName { get; set; }
        public string Venuename { get; set; }
        public List<OverviewModel> Tickets { get; set; }
        public List<Dropdownlist> Events { get; set; }
    }
    public class OverModels
    {
        public List<OverviewEventModel> Tickets { get; set; }
        public List<Dropdownlist> Events { get; set; }
    }

    public class AcccountTypeViewModel
    {
        public string AccountName { get; set; }
        public int GpId { get; set; }
        public List<ObjectMasterModel> Objects { get; set; }
        public List<ObjectMasterModel> Groupname { get; set; }
        public ApiResponse Resp { get; set; }
    }
    public class ObjectMasterModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Groupname { get; set; }
        public bool Watch { get; set; }
        public bool EC { get; set; }
        public bool Delete { get; set; }
    }
    public class UserManageModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int AccountTypeId { get; set; }
        public string AccountType { get; set; }
    }
    public class UsermangeviewModel
    {
        public List<UserManageModel> UserModel { get; set; }
        public List<Dropdownlist> Accounts { get; set; }
        public ApiResponse Resp { get; set; }
    }
    public class EdituserRoleModel
    {
        public UserManageModel UserManageModel { get; set; }
        public List<Dropdownlist> Accounts { get; set; }
    }

    #region #eventpromation
    public class Salesoverview
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public DateTime? StartDate { get; set; }
        public int? TotalTicket { get; set; }
        public int? BookTotal { get; set; }
        public decimal? EventRevenue { get; set; }
        public decimal? TransactionCost { get; set; }
        public decimal? Payout { get; set; }
    }
    public class BroadcastEmails
    {
        public string Youremail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public List<EmailList> Emails { get; set; }


    }

    public class OffersModel
    {
        public int Id { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public decimal? valus { get; set; }
        public int? OfferType { get; set; }
        public List<Dropdownlist> Events { get; set; }
        public List<Dropdownlist> TicketTypeddl { get; set; }
        public int? EventId { get; set; }
        public int? NoofCoupans { get; set; }
        public List<ExistingOffers> ExistingOffers { get; set; }
        public int OfferPageCategory { get; set; }
        public int TicketType { get; set; }
        public string CoupenCode { get; set; }
    }
    public partial class CouponModel
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public string CoupanCode { get; set; }
        public string Mobile { get; set; }        
    }
    public class BroadcastModel
    {
        public List<Dropdownlist> Events { get; set; }
    }

    public class ExistingOffers
    {
        public int Id { get; set; }
        public DateTime? startdate { get; set; }
        public int? Offertype { get; set; }
        public DateTime? enddate { get; set; }
        public decimal? discount { get; set; }
        public string coupencode { get; set; }
        public string EventName { get; set; }
        public int? Noofcoupons { get; set; }
        public int? OfferPageCategory { get; set; }
        public int? TicketType { get; set; }
        public int? NoOfUsedCoupon { get; set; }
    }
    #endregion

    public class CompnayProfile
    {
        public string Email { get; set; }
        public string CompnayName { get; set; }
        public int CountryId { get; set; }
        public int AddressId { get; set; }
        public string Address { get; set; }
        public string Contact_Number { get; set; }
        public List<Dropdownlist> Country { get; set; }
        public List<Dropdownlist> Banks { get; set; }
        public List<Dropdownlist> MobileWallet { get; set; }
        public BankDetails BankDetails { get; set; }


    }
    public class BankDetails
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public int? MobileAccountId { get; set; }
        public string Account_Holder_Name { get; set; }
        public string Account_Number { get; set; }
        public string Bank_Registration_Number { get; set; }
        public string mobile_money_UniqueId { get; set; }
    }
    public class DashboardOrgModel
    {
        public TopsaleEvent Topsaleevents { get; set; }
        public List<RecentTicket> Recenttickets { get; set; }
    }
    public class TopsaleEvent
    {
        public string EventName { get; set; }
        public string EventNametotal { get; set; }
        public int Total { get; set; }
        public int totalevent { get; set; }
        public double topticketSale { get; set; }
    }
    public class RecentTicket
    {
        public int Id { get; set; } 
        public string UserName { get; set; }
        public string EventName { get; set; }
        public string Total { get; set; }
        public string EventDate { get; set; }
        public string Bookedticket { get; set; }
        public string TotalAmt { get; set; }
        public int PayId { get; set; }
    }


}