using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SquareHook.Membership.Controllers
{
    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {
            var client = new SquareHook.Membership.Data.Models.Client();

            return View(client);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
