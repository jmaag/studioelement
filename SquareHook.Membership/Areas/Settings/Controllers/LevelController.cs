using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SquareHook.Membership.Models;

namespace SquareHook.Membership.Areas.Settings.Controllers
{
    [Authorize]
    public class LevelController : SquareHook.Membership.Controllers.ApplicationController
    {
        //
        // GET: /Settings/Level/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetLevels(int take, int page, string search)
        {
            int total = 0;

            if(page < 1) { page = 1; }
            if(search == null) { search = ""; }
            else { search = search.ToLower(); }

            var levels = (from l in Context.sh_levels
                          where l.Name.ToLower().Contains(search)
                          orderby l.LevelID
                          select new { l.LevelID, l.Name, l.Order });
            total = levels.Count();

            var results = levels.Skip((page - 1) * take).Take(take).ToList();

            return Json(new
            {
                total = total,
                results = results
            });
        }

        public JsonResult New(sh_level model)
        {
            bool success = true;

            try
            {
                Context.sh_levels.InsertOnSubmit(model);
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
                var level = (from l in Context.sh_levels where l.LevelID == id select l).First();
                Context.sh_levels.DeleteOnSubmit(level);
                Context.SubmitChanges();
            }
            catch { success = false; }

            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }
    }
}
