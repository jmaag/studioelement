using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SquareHook.Membership.Areas.Settings.Models
{
    public class CertificationModel : SquareHook.Membership.Models.sh_certification
    {
        public List<SelectListItem> Providers { get; set; }
        public List<SelectListItem> Levels { get; set; }
    }
}