using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventManager1.Areas.ScanNPassAPI.Models
{
    public class CompanyModel
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Name { get; set; }
        public string CompName { get; set; }
        public int  CompanyId { get; set; }
        public int Type { get; set; }

    }
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

       
       

    }
    public class EventModel
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Venue { get; set; }
        public string CityName { get; set; }
        public string ImagePath { get; set; }
    }
    public class EventTickets
    {
        public int Id { get; set; }
        public string TicketName { get; set; }
        public string Name { get; set; }
        public string ColorArea { get; set; }
        public string GateNo { get; set; }
        public int? Quantity { get; set; }
        public string Barcode { get; set; }
        public string EventName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Venue { get; set; }
        public string CityName { get; set; }
        public string ImagePath { get; set; }
        public string barcodeImageP { get; set; }
        public bool IsCheckIn { get; set; }
        public string ticketType { get; set; }
        public DateTime? updatedate { get; set; }
    }

    public class Barcodemodels
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Barcode { get; set; }

    }

    public class LoginSoViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
   


}