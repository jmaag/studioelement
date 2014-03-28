using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SquareHook.Membership.Controllers
{
    public class HomeController : ApplicationController
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Front()
        {
            return View();
        }
    }
}
