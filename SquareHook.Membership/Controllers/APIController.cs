using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SquareHook.Membership.Models;

namespace SquareHook.Membership.Controllers
{
    [AllowCrossJsonAttribute]
    public class APIController : ApplicationController
    {
        //
        // GET: /API/

        public JsonResult Start()
        {
            try
            {
                var careers = (from c in Context.sh_careers orderby c.DisplayOrder select new { 
                    c.CareerID,
                    c.AverageSalary, 
                    c.Demand, 
                    c.Name,
                    c.Level1Instructions, 
                    c.Level2Instructions, 
                    c.Level3Instructions, 
                    c.Level4Instructions, 
                    c.Level5Instructions 
                }).ToList();
                var providers = (from p in Context.sh_providers
                                 orderby p.Name select new Provider { 
                                         Name = p.Name, 
                                         ProviderID = p.ProviderID, 
                                         CertificationCount = p.sh_certifications.Count,
                                         Show = true
                                     }).ToList();

                for (int i = 0; i < providers.Count; i++)
                {
                    providers[i].Show = i < 10;
                }
                return Json(new { success = true, careers = careers, providers = providers });
            }
            catch { }
            return Json(new { success = false });
        }

        public JsonResult CareerDetails(int id)
        {
            try
            {
                // initialize model
                CareerDetails model = new Models.CareerDetails();
                model.Levels = new List<LevelDetails>();

                // retrieve certifications
                var certs = (from c in Context.sh_certifications where c.sh_career_certifications.Any(car => car.CareerID == id) 
                             orderby c.LevelID select c).ToList();

                foreach (var cert in certs)
                {
                    var levelCheck = (from l in model.Levels where l.LevelID == cert.LevelID select l);
                    LevelDetails level = new LevelDetails() { LevelID = cert.LevelID, Name = cert.sh_level.Name };

                    if (levelCheck.Count() > 0)
                    {
                        level = levelCheck.First();
                    }
                    else
                    {
                        model.Levels.Add(level);
                    }

                    // check for no certs
                    if (level.Certifications == null) { level.Certifications = new List<CertificationDetails>(); }
                    level.Certifications.Add(new CertificationDetails {
                        Abbreviation = cert.Abbreviation, 
                        CareerTrack = cert.CareerTrack, 
                        CertificationID = cert.CertificationID, 
                        Details = cert.Details, 
                        LevelID = cert.LevelID, 
                        ProviderID = cert.ProviderID, 
                        Title = cert.Title, 
                        Url = cert.Url, 
                        Provider = cert.sh_provider.Name
                    });
                }

                return Json(new { success = true, results = model });
            }
            catch {}

            return Json(new { success = false });
        }

        public JsonResult ViewAll()
        {
            try
            {
                // initialize model
                CareerDetails model = new Models.CareerDetails();
                model.Levels = new List<LevelDetails>();

                // retrieve certifications
                var certs = (from c in Context.sh_certifications
                             orderby c.LevelID
                             select c).ToList();

                foreach (var cert in certs)
                {
                    var levelCheck = (from l in model.Levels where l.LevelID == cert.LevelID select l);
                    LevelDetails level = new LevelDetails() { LevelID = cert.LevelID, Name = cert.sh_level.Name };

                    if (levelCheck.Count() > 0)
                    {
                        level = levelCheck.First();
                    }
                    else
                    {
                        model.Levels.Add(level);
                    }

                    // check for no certs
                    if (level.Certifications == null) { level.Certifications = new List<CertificationDetails>(); }
                    level.Certifications.Add(new CertificationDetails
                    {
                        Abbreviation = cert.Abbreviation,
                        CareerTrack = cert.CareerTrack,
                        CertificationID = cert.CertificationID,
                        Details = cert.Details,
                        LevelID = cert.LevelID,
                        ProviderID = cert.ProviderID,
                        Title = cert.Title,
                        Url = cert.Url,
                        Provider = cert.sh_provider.Name
                    });
                }

                return Json(new { success = true, results = model });
            }
            catch { }

            return Json(new { success = false });
        }

    }
}
