using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SquareHook.Membership.Areas.Settings.Models
{
    public class CareerModel : SquareHook.Membership.Models.sh_career
    {
        public List<SelectListItem> Certifications { get; set; }

        public List<int> SelectedCerts { get; set; }
    }
}