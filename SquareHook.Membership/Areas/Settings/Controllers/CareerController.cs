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
    public class CareerController : SquareHook.Membership.Controllers.ApplicationController
    {
        //
        // GET: /Settings/Level/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCareers(int take, int page, string search)
        {
            int total = 0;

            if (page < 1) { page = 1; }
            if (search == null) { search = ""; }
            else { search = search.ToLower(); }

            var providers = (from l in Context.sh_careers
                             where l.Name.ToLower().Contains(search)
                             orderby l.Name
                             select new
                             {
                                 l.Name,
                                 l.CareerID,
                                 l.DisplayOrder,
                                 l.AverageSalary,
                                 l.Demand
                             });
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
        public ActionResult New(CareerModel model)
        {
            if (ModelState.IsValid)
            {
                var career = new sh_career()
                {
                    AverageSalary = model.AverageSalary,
                    Demand = model.Demand,
                    DisplayOrder = model.DisplayOrder,
                    Level1Instructions = model.Level1Instructions,
                    Level2Instructions = model.Level2Instructions,
                    Level3Instructions = model.Level3Instructions,
                    Level4Instructions = model.Level4Instructions,
                    Level5Instructions = model.Level5Instructions,
                    Name = model.Name
                };

                // create the career
                Context.sh_careers.InsertOnSubmit(career);

                // save to the database
                Context.SubmitChanges();
                
                // create the associated certifications
                if (model.SelectedCerts != null)
                {
                    foreach (var id in model.SelectedCerts)
                    {
                        sh_career_certification assoc = new sh_career_certification() { CareerID = career.CareerID, CertificationID = id };
                        Context.sh_career_certifications.InsertOnSubmit(assoc);
                    }
                }

                // save to the database again
                Context.SubmitChanges();

                return RedirectToAction("Edit", new { id = career.CareerID });
            }

            return View(getModel());
        }

        public ActionResult Edit(int id)
        {
            var career = (from c in Context.sh_careers where c.CareerID == id select c).First();
            var model = getModel();
            model.AverageSalary = career.AverageSalary;
            model.CareerID = career.CareerID;
            model.Demand = career.Demand;
            model.DisplayOrder = career.DisplayOrder;
            model.Level1Instructions = career.Level1Instructions;
            model.Level2Instructions = career.Level2Instructions;
            model.Level3Instructions = career.Level3Instructions;
            model.Level4Instructions = career.Level4Instructions;
            model.Level5Instructions = career.Level5Instructions;
            model.Name = career.Name;

            model.SelectedCerts = new List<int>();
            foreach (var cert in career.sh_career_certifications)
            {
                model.SelectedCerts.Add(cert.CertificationID);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CareerModel model)
        {
            if (ModelState.IsValid)
            {
                // update the career object
                var career = (from c in Context.sh_careers where c.CareerID == model.CareerID select c).First();
                career.AverageSalary = model.AverageSalary;
                career.Demand = model.Demand;
                career.DisplayOrder = model.DisplayOrder;
                career.Level1Instructions = model.Level1Instructions;
                career.Level2Instructions = model.Level2Instructions;
                career.Level3Instructions = model.Level3Instructions;
                career.Level4Instructions = model.Level4Instructions;
                career.Level5Instructions = model.Level5Instructions;
                career.Name = model.Name;

                // clear old associations of certifications and add new ones.
                Context.sh_career_certifications.DeleteAllOnSubmit(career.sh_career_certifications);
                Context.SubmitChanges();

                // create the associated certifications
                foreach (var id in model.SelectedCerts)
                {
                    sh_career_certification assoc = new sh_career_certification() { CareerID = career.CareerID, CertificationID = id };
                    Context.sh_career_certifications.InsertOnSubmit(assoc);
                }

                // save to the database again
                Context.SubmitChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public JsonResult Delete(int id)
        {
            bool success = true;

            try
            {
                var level = (from l in Context.sh_careers where l.CareerID == id select l).First();
                Context.sh_careers.DeleteOnSubmit(level);
                Context.SubmitChanges();
            }
            catch(Exception exp) { 
                success = false; 
            }

            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }

        private CareerModel getModel()
        {
            CareerModel model = new CareerModel();
            model.Certifications = (from l in Context.sh_certifications
                               orderby l.Title
                               select new SelectListItem
                               {
                                   Text = l.Title,
                                   Value = l.CertificationID.ToString()
                               }).ToList();
            return model;
        }
    }
}
