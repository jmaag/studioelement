using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SquareHook.Membership.Models;
using SquareHook.Membership.Areas.Settings.Models;

namespace SquareHook.Membership.Areas.Settings.Controllers
{
    [Authorize]
    public class CertificationController : SquareHook.Membership.Controllers.ApplicationController
    {
        //
        // GET: /Settings/Level/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCertifications(int take, int page, string search)
        {
            int total = 0;

            if (page < 1) { page = 1; }
            if (search == null) { search = ""; }
            else { search = search.ToLower(); }

            var providers = (from l in Context.sh_certifications
                             where l.Title.ToLower().Contains(search) || l.Abbreviation.ToLower().Contains(search)
                             orderby l.ProviderID
                             select new { 
                                 l.Title, 
                                 l.Abbreviation,
                                 l.CertificationID, 
                                 l.CareerTrack, 
                                 ProviderName = l.sh_provider.Name, 
                                 LevelName = l.sh_level.Name });
            total = providers.Count();

            var results = providers.Skip((page - 1) * take).Take(take).ToList();

            return Json(new
            {
                total = total,
                results = results
            });
        }

        public ActionResult New()
        {
            return View(getModel());
        }

        [HttpPost]
        public ActionResult New(sh_certification model)
        {
            if (ModelState.IsValid)
            {
                Context.sh_certifications.InsertOnSubmit(model);
                Context.SubmitChanges();

                return RedirectToAction("Edit", new { id = model.CertificationID });
            }
            
            return View(getModel());
        }

        public ActionResult Edit(int id)
        {
            var cert = (from c in Context.sh_certifications where c.CertificationID == id select c).First();
            var model = getModel();
            model.Abbreviation = cert.Abbreviation;
            model.CareerTrack = cert.CareerTrack;
            model.CertificationID = cert.CertificationID;
            model.Details = cert.Details;
            model.LevelID = cert.LevelID;
            model.ProviderID = cert.ProviderID;
            model.Title = cert.Title;
            model.Url = cert.Url;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CertificationModel model)
        {
            if (ModelState.IsValid)
            {
                var cert = (from c in Context.sh_certifications where c.CertificationID == model.CertificationID select c).First();
                cert.Abbreviation = model.Abbreviation;
                cert.CareerTrack = model.CareerTrack;
                cert.Details = model.Details;
                cert.Title = model.Title;
                cert.Url = model.Url;
                cert.LevelID = model.LevelID;
                cert.ProviderID = model.ProviderID;

                Context.SubmitChanges();

                return RedirectToAction("Index");
            }

            model.Levels = (from l in Context.sh_levels
                            orderby l.LevelID
                            select new SelectListItem
                            {
                                Text = l.Name,
                                Value = l.LevelID.ToString()
                            }).ToList();
            model.Providers = (from l in Context.sh_providers
                               orderby l.Name
                               select new SelectListItem
                               {
                                   Text = l.Name,
                                   Value = l.ProviderID.ToString()
                               }).ToList();

            return View(model);
        }

        public JsonResult Delete(int id)
        {
            bool success = true;

            try
            {
                var level = (from l in Context.sh_certifications where l.CertificationID == id select l).First();
                Context.sh_certifications.DeleteOnSubmit(level);
                Context.SubmitChanges();
            }
            catch { success = false; }

            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }

        private CertificationModel getModel()
        {
            CertificationModel model = new CertificationModel();
            model.Levels = (from l in Context.sh_levels
                            orderby l.LevelID
                            select new SelectListItem
                            {
                                Text = l.Name,
                                Value = l.LevelID.ToString()
                            }).ToList();
            model.Providers = (from l in Context.sh_providers
                               orderby l.Name
                               select new SelectListItem
                               {
                                   Text = l.Name,
                                   Value = l.ProviderID.ToString()
                               }).ToList();
            return model;
        }
    }
}
