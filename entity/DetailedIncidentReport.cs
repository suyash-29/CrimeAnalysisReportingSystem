using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimeAnalysisReportingSystem2.entity
{
    public class DetailedIncidentReport
    {
        public Incident Incident { get; set; }
        public Report Report { get; set; }
        public Officer ReportingOfficer { get; set; }
        public List<Suspect> Suspects { get; set; }
        public List<Victim> Victims { get; set; }


    }

}
