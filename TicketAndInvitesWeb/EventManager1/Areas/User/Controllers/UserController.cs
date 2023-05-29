using EventManager1.Areas.User.Models;
using EventManager1.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static EventManager1.MvcApplication;


namespace EventManager1.Areas.User.Controllers
{
    [UserSessionExpire]
    public class UserController : Controller
    {
        common cdb = new common();
        HandleEvent hdb = new HandleEvent();
        // GET: User/User
        public ActionResult Dashboard()
        {
            List<Event_> ct = new List<Event_>();
            try
            {
                List<TicketTypemodel> tType = cdb.GetticketTypes();
                var act = hdb.GetUserTickets(ManageSession.UserSession.Id);
                ct = act.ToList();
            }
            catch (Exception ex) { RedirectToAction("Login", "Account", new { area = "" }); }
            return View(ct);
        }
        public ActionResult Tickets(int id, int tmap = 0)
        {

            if (id > 0)
            {
                Event_ ev = new Event_();
                List<Event_> ct = new List<Event_>();
                try
                {
                    List<TicketTypemodel> tType = cdb.GetticketTypes();
                    ct = hdb.GetUserTickets(ManageSession.UserSession.Id);
                    TempData["id"] = id;
                    TempData.Keep("id");
                    ev = ct.Where(m => m.Id == tmap).FirstOrDefault();

                    return View(ev);
                }
                catch (Exception ex) { RedirectToAction("Login", "Account", new { area = "" }); }
            }

            return RedirectToAction("Dashboard", "User", new { area = "User" }); ;

        }
        public ActionResult Ticketdownload()
        {
            return View();
        }
        [HttpPost]
        [ActionName("Tickets")]
        public ActionResult Ticket()
        {
            Event_ ev = new Event_();
            int id = 0; int userid = 0;
            if (TempData["id"] != null) { id = Convert.ToInt32(TempData["id"]); userid = ManageSession.UserSession.Id; }
            else
            {
                if (Request.QueryString["id"] != null && Request.QueryString["user"] != null)
                {
                    id = Convert.ToInt32(Request.QueryString["id"]);
                    userid = Convert.ToInt32(Request.QueryString["user"]);
                }
            }

            if (id > 0)
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
                    chunk = new Chunk("Event Name: " + ev.EventName + "\nName: " + ev.UserName + "\nTicket Name: " + ev.Tickets.FirstOrDefault().TicketName + "\nVenue: " + ev.Venue + "\nStart Date: " + Convert.ToDateTime(ev.StartDate).ToString("dd/MM/yy HH:mmtt") + "" +
                        "\nGate No: " + ev.Tickets.FirstOrDefault().GateNo + " , Seats :" + ev.Tickets.FirstOrDefault().Quantity + "\nColor Area :" + ev.Tickets.FirstOrDefault().ColorArea, FontFactory.GetFont("Arial", 15, Font.NORMAL));
                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.AddElement(chunk);
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    Image image1 = Image.GetInstance(Server.MapPath(String.Format("https://www.ticketsandinvites.com/Content/BarCode/" + ev.Tickets.FirstOrDefault().Barcode + ".jpg")));
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
            if (Request.QueryString["id"] != null && Request.QueryString["user"] != null)
            {
                return RedirectToAction("Ticketdownload");
            }
            else { return View(ev); }
        }


        //[HttpPost]
        //[ValidateInput(false)]
        //public FileResult Export(string GridHtml)
        //{
        //    using (MemoryStream stream = new System.IO.MemoryStream())
        //    {
        //        StringReader sr = new StringReader(GridHtml);
        //        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
        //        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
        //        pdfDoc.Open();
        //        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
        //        pdfDoc.Close();
        //        return File(stream.ToArray(), "application/pdf", "Ticket.pdf");
        //    }
        //}
        //[HttpPost]
        //[ValidateInput(false)]
        //public FileResult exportPDF(string gridHTML)
        //{
        //    using (MemoryStream stream = new System.IO.MemoryStream())
        //    {
        //        StringReader sr = new StringReader(gridHTML);
        //        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
        //        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
        //        pdfDoc.Open();
        //        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
        //        pdfDoc.Close();
        //        return File(stream.ToArray(), "application/pdf", "Grid.pdf");
        //    }
        //    //StringReader reader = null;
        //    //Document pdfDoc = null;
        //    //PdfWriter write = null;
        //    //try
        //    //{
        //    //    using (MemoryStream memorystream = new MemoryStream())
        //    //    {
        //    //        reader = new StringReader(gridHTML);
        //    //        pdfDoc = new Document(PageSize.A4, 5f, 5f, 5f, 5f);
        //    //        write = PdfWriter.GetInstance(pdfDoc, memorystream);
        //    //        pdfDoc.Open();
        //    //        XMLWorkerHelper.GetInstance().ParseXHtml(write, pdfDoc, reader);
        //    //        pdfDoc.Close();
        //    //        return File(memorystream.ToArray(), "application/pdf", Convert.ToString(DateTime.Now + ".pdf"));
        //    //    }

        //    //}
        //    //finally
        //    //{
        //    //    reader.Dispose();
        //    //    pdfDoc.Dispose();
        //    //    write.Dispose();
        //    //}
        //}
        public ActionResult CreateEvent()
        {
            int id = Convert.ToInt32(Request.QueryString["id"]);
            List<Country> ct = cdb.GetCountry();
            ViewBag.country = ct;
            ViewBag.compName = ManageSession.UserSession.FirstName;
            Event_ ev = new Event_();
            if (id > 0)
            {
                ev = cdb.GetEvents(id);
                ViewBag.id = id;
                //var x = ev.Multimedia;
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CreateEvent(Event_ Register)
        {
            var isValidModel = true;
            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            result.Msg = "Invalid Model";
            Register.User_Id = ManageSession.UserSession.Id;
            if (isValidModel)
            {
                HandleEvent hdb = new HandleEvent();
                result = hdb.EventRegister(Register);
                return new JsonResult { Data = new { result } };
            }
            return new JsonResult { Data = new { result } };
        }

        public ActionResult ManageEvents()
        {
            return View();
        }
        public ActionResult Notification()
        {
            List<NotificationModel> result = new List<NotificationModel>();
            try { result = CommonDbLogic.GetNotification(2); }
            catch { }
            return View(result);
        }

        public ActionResult Message(int mid = 0)
        {
            List<NotificationModel> result = new List<NotificationModel>();

            try
            {
                result = CommonDbLogic.GetMessage(0, 0);

                if (mid > 0)
                {
                    List<NotificationModel> results = CommonDbLogic.GetMessage(mid);
                    ViewBag.msglist = results;
                    //return PartialView("_allmessage", results);
                }
            }
            catch { }

            return View(result);
        }
        [HttpPost]
        public ActionResult Message(string txtmessagetitle, string txtmessage, int MsgId = 0)
        {
            ApiResponse result = new ApiResponse();
            List<NotificationModel> results = new List<NotificationModel>();
            try
            {
                results = CommonDbLogic.GetMessage(0);
                if (MsgId > 0)
                {
                    result = CommonDbLogic.AddMessage(txtmessage, 0, 0, "", MsgId);
                    List<NotificationModel> resultss = CommonDbLogic.GetMessage(MsgId);
                    ViewBag.msglist = resultss;
                    return RedirectToAction("Message", new { mid = MsgId });
                    //return View(new { mid = MsgId }, results);
                    //return RedirectToAction("Message?mid="+MsgId, results);                    
                }
                else
                {
                    result = CommonDbLogic.AddMessage(txtmessage, 0, 0, txtmessagetitle);

                }


            }
            catch (Exception ex) { result.Msg = ex.ToString(); }
            ViewData["Error"] = result;
            return RedirectToAction("Message");
            //return View(results);
        }
        public ActionResult Settings()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Settings(FormCollection collection)
        {
            string email = collection["txtemail"];
            string firstname = collection["txtfirsname"];
            string lastname = collection["txtlastname"];
            ViewData["Error"] = UserDbOperation.UpdateDetail(email, firstname, lastname, ManageSession.UserSession.Id);
            return View();
        }
        //[HttpPost]
        //public ActionResult Security(FormCollection collection)
        //{
        //    string current = collection["txtcurretpassword"];
        //    string newpass = collection["txtnewpassword"];
        //    ViewData["Error"] = UserDbOperation.UpdatePassword(newpass, current, ManageSession.UserSession.Id);
        //    return View("Settings");
           
        //}
        [HttpPost]
        public JsonResult Security(string currentpassword, string newpassword)
        {
            int msg = 0;
            if (currentpassword != "" && newpassword != "")
            {
                ViewData["Error"] = UserDbOperation.UpdatePassword(newpassword, currentpassword, ManageSession.UserSession.Id);
                if (ViewData["Error"] != null)
                {
                    var error = ViewData["Error"] as ApiResponse;
                    msg = error.Code;
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Logout()
        {
            HandleLiveEvent hdb = new HandleLiveEvent();
            //hdb.DeleteLoggedForLiveStreaming(ManageSession.UserSession.Id);
            ManageSession.UserSession = null;
            HttpCookie userInfo = new HttpCookie("userInfo");
            userInfo["UserName"] = "";
            userInfo.Domain = "stream233.com";
            userInfo.Expires.Add(new TimeSpan(0, 0, 0));
            Response.Cookies.Add(userInfo);
            
            Response.Cookies["CheckVideoSession"].Value = "";
            Response.Cookies["VideoId"].Value = "";
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                string name = ManageSession.UserSession.FirstName.ToString().Trim();
                try
                {

                    string[] imgpath = new string[3];
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        imgpath[i] = "../../ImageEvent/" + name + "/" + fname.Replace(" ", "");
                        string path = Server.MapPath("~/ImageEvent/" + name); //Session["name"]
                        if (Directory.Exists(path))
                        { }
                        else { path = Directory.CreateDirectory(path).ToString(); }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/ImageEvent/"), path, fname.Replace(" ", ""));

                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json(imgpath);
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
        [HttpPost]
        public ActionResult geteventbyid(int id)
        {
            Event_ ev = new Event_();
            ApiResponse result = new ApiResponse();
            result.Code = (int)ApiResponseCode.fail;
            try
            {
                if (id > 0)
                {
                    ev = cdb.GetEvents(id);
                }
                return Json(ev);
            }
            catch (Exception ex) { return new JsonResult { Data = new { result } }; }
        }
        public ActionResult GetNotification()
        {
            try
            {
                List<NotificationModel> result = CommonDbLogic.GetNotification(2);

                return PartialView("_Notification", result);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_Notification");
        }

    }
}