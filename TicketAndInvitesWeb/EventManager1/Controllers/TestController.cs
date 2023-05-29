using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventManager1.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult LiveStream()
        {
            return View();
        }
        public ActionResult LiveHLSStream()
        {
            return View();
        }
        public ActionResult testvideo()
        {
            return View();
        }
    }
}