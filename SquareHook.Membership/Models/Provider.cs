using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SquareHook.Membership.Models
{
    public class Provider
    {
        public string Name { get; set; }
        public int ProviderID { get; set; }
        public int CertificationCount { get; set; }
        public bool Show { get; set; }
    }
}