using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimeAnalysisReportingSystem2.entity
{
    public class Agency
    {
        public int AgencyID { get; set; }
        public string AgencyName { get; set; }
        public string Jurisdiction { get; set; }
        public string ContactInfo { get; set; }

        public Agency() { }

        public Agency(string agencyName, string jurisdiction, string contactInfo)
        {
            AgencyName = agencyName;
            Jurisdiction = jurisdiction;
            ContactInfo = contactInfo;
        }
    }
}

