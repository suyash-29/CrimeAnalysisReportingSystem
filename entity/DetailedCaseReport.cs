using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimeAnalysisReportingSystem2.entity
{
    public class DetailedCaseReport
    {
        public Case CaseInfo { get; set; } // Holds basic case information
        public List<DetailedIncidentReport> DetailedIncidents { get; set; } // Holds detailed incident information
    }

}
