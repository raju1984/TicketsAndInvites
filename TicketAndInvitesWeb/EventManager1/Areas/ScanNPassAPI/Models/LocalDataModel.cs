using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManager1.Areas.ScanNPassAPI.Models
{
    public class LocalDataModel
    {
        public int Event_Id{ get; set; }
        public string Event_Name { get; set; }
        public string City_Name { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public string Event_Venue { get; set; }
        public int Ticket_Id { get; set; }
        public string Ticket_User_name { get; set; }
        public string ticket_Barcode { get; set; }
        public string ticket_Area { get; set; }
        public string Ticket_Name { get; set; }
        public string Gate_No { get; set; }
        public int Quantity { get; set; }
        public Boolean IsCheckIn { get; set; }
        public string Email { get; set; }
        public string Ticket_buyer_name { get; set; }

        public string Table_No { get; set; }
        public string Sheet_No { get; set; }
        public string Ticket_type { get; set; }
        public string TicketColor { get; set; }
        public decimal Ticket_price { get; set; }
        public int SheetNo { get; set; }
        public DateTime? Update_Date { get; set; }
    }

    public class SavedateModal
    {
        public int Event_Id { get; set; }
        public int ticket_Id { get; set; }
        public DateTime update_date { get; set; }

    }

}