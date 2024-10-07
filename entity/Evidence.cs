using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimeAnalysisReportingSystem2.entity
{
    public class Evidence
    {
        public int EvidenceID { get; set; }
        public string Description { get; set; }
        public string LocationFound { get; set; }
        public int IncidentID { get; set; }

        public Evidence() { }

        public Evidence(string description, string locationFound, int incidentId)
        {
            Description = description;
            LocationFound = locationFound;
            IncidentID = incidentId;
        }
    }
}

