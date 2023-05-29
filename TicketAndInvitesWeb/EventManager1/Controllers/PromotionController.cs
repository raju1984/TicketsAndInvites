using EventManager1.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;


namespace EventManager1.Controllers
{
    public class PromotionController : Controller
    {
        // GET: Promotion
        [HttpPost]
        public JsonResult Invite(string Id)
        {
            //IndexView obj = new IndexView {EventName="ashok testoing" };
            //Guid id = Guid.NewGuid();
            //string body = EmailSending.RunCompile("/Views/EmailTemplate/", "Invitation.cshtml", id.ToString(), obj);
            //EmailSending.SendEmail("ashoktest@gmail.com", body);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult InvitationTemplate()
        {
            return View();
        }
        public async Task<ActionResult> SendTicket(string EventTickeId, string Status, string request, string address, string rootpath,int type=1)
        {
            rootpath = HostingEnvironment.ApplicationPhysicalPath + "/Views/EmailTemplate/";// Server.MapPath("/Views/EmailTemplate/");
            if (type == 2)
            {
                ViewData["Status"] = "Mobile";
                ViewData["EventTickeId"] = EventTickeId;
                return View();
            }
            if (!string.IsNullOrEmpty(EventTickeId) && !string.IsNullOrEmpty(Status))
            {
                

                if (Status == "1")
                {

                    if (await ModelInviation.SendTicket(Convert.ToInt32(EventTickeId), Convert.ToInt32(Status), request, address, rootpath))
                    {
                        ViewData["Status"] = "Success";
                        bool updateinvit = ModelInviation.UpdateInvitation(Convert.ToInt32(EventTickeId), Convert.ToInt32(Status));
                        return View();
                    }
                }
                else if (Status == "2")
                {
                    ViewData["Status"] = "Tentative";
                }
                else
                {
                    ViewData["Status"] = "Rejected";
                }
                bool updateinvite = ModelInviation.UpdateInvitation(Convert.ToInt32(EventTickeId), Convert.ToInt32(Status));
            }
            else
            {
                ViewData["Status"] = "fail";
            }
            return View();
        }

        public ActionResult Downloadticket()
        {
            common cdb = new common();
            HandleEvent hdb = new HandleEvent();
            Event_ ev = new Event_();
            int id = 0; int userid = 0;

            if (Request.QueryString["id"] != null && Request.QueryString["user"] != null)
            {
                id = Convert.ToInt32(Request.QueryString["id"]);
                userid = Convert.ToInt32(Request.QueryString["user"]);
            }

            if (id > 0 && userid > 0)
            {

                List<Event_> ct = new List<Event_>();
                try
                {
                    List<TicketTypemodel> tType = cdb.GetticketTypes();
                    ct = hdb.GetUserTickets(userid);
                    ev = ct.Where(m => m.Id == id).FirstOrDefault();
                    Document pdfDoc = new Document(PageSize.A4, 50, 50, 50, 50);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    //Top Heading
                    Chunk chunk = new Chunk("Your E-Ticket has been Generated", FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.BLACK));
                    pdfDoc.Add(chunk);

                    //Horizontal Line
                    Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_CENTER, 1)));
                    pdfDoc.Add(line);

                    //Table
                    PdfPTable table = new PdfPTable(2);
                    table.WidthPercentage = 100;
                    //0=Left, 1=Centre, 2=Right
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 20f;
                    table.SpacingAfter = 30f;

                    //Cell no 1
                    PdfPCell cell = new PdfPCell();
                    cell.Border = 0;
                    Image image = Image.GetInstance(Server.MapPath(ev.Multimedia.FirstOrDefault().URL));
                    image.ScaleAbsolute(200, 100);
                    cell.AddElement(image);
                    table.AddCell(cell);
                    chunk = new Chunk("Event Name: " + ev.EventName + "\nName: " + ev.UserName+ "\nVenue: " + ev.Venue + "\nStart Date: " + Convert.ToDateTime(ev.StartDate).ToString("dd/MM/yy hh:mm") + "" +
                        "\nGate No: " + ev.Tickets.FirstOrDefault().GateNo + "\nSeats :" + ev.Tickets.FirstOrDefault().Quantity + "\nColor Area :" + ev.Tickets.FirstOrDefault().ColorArea, FontFactory.GetFont("Arial", 15, Font.NORMAL));
                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.AddElement(chunk);
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    Image image1 = Image.GetInstance(Server.MapPath(String.Format("../../Content/BarCode/" + ev.Tickets.FirstOrDefault().Barcode + ".jpg")));
                    image1.ScaleAbsolute(130, 130);
                    image1.IndentationLeft = 200;
                    cell.AddElement(image1);
                    Paragraph para7 = new Paragraph();

                    para7.Add(image1);
                    para7.IndentationLeft = 175;
                    pdfDoc.Add(para7);

                    table = new PdfPTable(3);
                    table.WidthPercentage = 80;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 20f;
                    table.SpacingAfter = 30f;

                    cell = new PdfPCell();
                    cell.Border = 0;

                    pdfDoc.Add(table);
                    Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_CENTER, 1)));
                    pdfDoc.Add(lines);
                    Paragraph para = new Paragraph();
                    para.Add("REFUND OF EXCHANGES EXCEPT AS NOTED EVENT DATE, TIME AND TICKET PRICE SUBJECT TO CHANGE." +
                        " The following terms apply. The QR Code allows one entry per scan. Unauthorized duplication or sale of this ticket may prevent admittance all holders of this ticket QR Code." +
                        " TICKET IS A REVOCABLE LICENSE Management may, without refund, revoke the license or refuse admission for noncompliance with these terms or for disorderly conduct." +
                        " No photography, video, or other recording or Transmission of any images of event are permitted without venue or artist approval. " +
                        "Ticket not redeemable for cash. Alcohol, drugs, contraband, cameras, recording devices, bundles, and containers are strictly prohibited. " +
                        "Your consent to search on entry and waive related claims. If event is cancelled or rescheduled, no refund required if you are given the right within 12 months of date of event, to attend are scheduled performance of same event or to exchange ticket for a ticket comparable in price and location to similar event as designated by management, except as Provided by law.");
                    pdfDoc.Add(para);

                    //Horizontal Line
                    line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line);
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    Response.Buffer = true;
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=Ticket.pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Write(pdfDoc);
                    Response.End();
                }
                catch (Exception ex) { }
            }
            return RedirectToAction("Index", "Home");
        }

    }
}