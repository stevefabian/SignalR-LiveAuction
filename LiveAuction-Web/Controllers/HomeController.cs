using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveAuction.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Auction()
        {
            return View();
        }

        public ActionResult Admin()
        {
            ViewBag.Message = "SignalR Auction Administration";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "SignalR Auction Reference Application";

            return View();
        }
    }
}