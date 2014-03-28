using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SquareHook.Membership.Models;

namespace SquareHook.Membership.Areas.Settings.Controllers
{
    [Authorize]
    public class ProviderController : SquareHook.Membership.Controllers.ApplicationController
    {
        //
        // GET: /Settings/Level/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProviders(int take, int page, string search)
        {
            int total = 0;

            if (page < 1) { page = 1; }
            if (search == null) { search = ""; }
            else { search = search.ToLower(); }

            var providers = (from l in Context.sh_providers
                             where l.Name.ToLower().Contains(search)
                             orderby l.ProviderID
                             select new { l.Name, l.ProviderID });
            total = providers.Count();

            var results = providers.Skip((page - 1) * take).Take(take).ToList();

            return Json(new
            {
                total = total,
                results = results
            });
        }

        public JsonResult New(sh_provider model)
        {
            bool success = true;

            try
            {
                Context.sh_providers.InsertOnSubmit(model);
                Context.SubmitChanges();
            }
            catch { success = false; }

            return Json(new { success = success, results = model });
        }

        public JsonResult Delete(int id)
        {
            bool success = true;

            try
            {
                var level = (from l in Context.sh_providers where l.ProviderID == id select l).First();
                Context.sh_providers.DeleteOnSubmit(level);
                Context.SubmitChanges();
            }
            catch { success = false; }

            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }
    }
}
