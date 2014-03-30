using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SquareHook.Membership.Models
{
    public class CareerDetails
    {
        public int CareerID { get; set; }

        public List<LevelDetails> Levels { get; set; }
    }

    public class LevelDetails
    {
        public int LevelID { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public string Instruction { get; set; }
        public string NameClass
        {
            get
            {
                return Name.ToLower().Replace(' ', '-').Replace('/', '-');
            }
        }

        public List<CertificationDetails> Certifications { get; set; }
    }

    public class CertificationDetails 
    {
        public int CertificationID { get; set; }
        public int ProviderID { get; set; }
        public int LevelID { get; set; }
        public string Title { get; set; }
        public string Abbreviation { get; set; }
        public string CareerTrack { get; set; }
        public string Url { get; set; }
        public string Details { get; set; }
        public string Provider { get; set; }
    }
}