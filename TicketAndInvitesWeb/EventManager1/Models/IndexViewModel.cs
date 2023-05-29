using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventManager1.Models
{
    public class IndexView
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public List<TicketPrice> Price { get; set; }
        public string Location { get; set; }
        public CompanyModel company { get; set; }
        public List<Images> Images { get; set; }
        public List<RsvpDetail> RsvpDetails { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public int? SubCriptionType { get; set; }
        public int? EventType { get; set; }
        public int? EventCatg { get; set; }
        public decimal? longt { get; set; }
        public decimal? lati { get; set; }
        public string address { get; set; }
        public string VenuName { get; set; }
        public bool? IsPopupar { get; set; }
        public string CompanyName { get; set; }
        public string Currency { get; set; }
        public int CompanyId { get; set; }
        public SubscriptionTypes subscripton {get; set; }
        public LoginViewModel login { get; set; }

}
    public class CompanyModel
    {        
        public string EmailId { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int Zip { get; set; }
    }
    public class SubscriptionTypes
    {       
        public int Platinum { get; set; }
        public int Gold { get; set; }
        public int Silver { get; set; }
        public int Bronze { get; set; }
    }
    public class Images
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public int? MulType { get; set; }
        public bool? mainpic { get; set; }
    }

    public class TicketPrice
    {
        public int TicketId { get; set; }
        public decimal? Price { get; set; }
        public string TicketName { get; set; }
        public int?  TicketType { get; set; }
        public int? AvailableQnty { get; set; }
        public bool IsPurchased { get; set; }
        public bool? IsEnable { get; set; }
        public int PaymentType { get; set; }
    }
    public class TicketModelPopup
    {
        public companyDetail Company { get; set; }      
        public string Eventname { get; set; }
        public string CoverImage { get; set; }
        public int EventId { get; set; }
        public DateTime? EventDate { get; set; }
        public List<TicketPrice> Tickest { get; set; }
        public ContactDetails ContactDetail { get; set; }
        public string LivestreamTiccket { get; set; }
        public string VideoTicket { get; set; }
        public LoginphoneModel loginpopup { get; set; }
        public int? CreatedOnWebsite { get; set; }
        public string UserId { get; set; }

        [EmailAddress]
        [Required]
        public string EmailAddress { get; set; }
        [Required]

        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }
        [Required]

        public string MobileNumber { get; set; }

    }
    public class RsvpDetail
    {
        public string Name { get; set; }
        public string Contact { get; set; }
    }
    public class ContactDetails
    {
        public string name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }

    public class Failedpaymentmodel
    {
        public int PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string ResponseId { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public decimal Amount { get; set; }
        public string EventName { get; set; }
        public string PaymentFor { get; set; }
        public string Gateway { get; set; }
        public string PhoneNo { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int PaymentStatus { get; set; }
        public int EventId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class EventTicketIdModel
    {

        public int EventId { get; set; }

        public int PaymentId { get; set; }

        public string Page { get; set; }

        public string TransactionId { get; set; }

        public int Amount { get; set; }
    }
    public class ReissueFailedpaymentmodel
    {
        public int TicketUserMapId { get; set; }
        public int TicketId { get; set; }
        public string TicketName { get; set; }
        public DateTime PaymentDate { get; set; }
        public int PaymentId { get; set; }
        public int EventId { get; set; }
        public string TransactionId { get; set; }
        public string EventName { get; set; }
        public string Gateway { get; set; }
        public int PaymentStatus { get; set; }
        public int Quantity { get; set; }
        public int ActualPayment { get; set; }
        public string OfferCode { get; set; }
        public decimal Amount { get; set; }
        public string PaymentFor { get; set; }
    }

}