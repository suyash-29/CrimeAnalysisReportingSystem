using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimeAnalysisReportingSystem2.entity
{
    public class Report
    {
        public int ReportID { get; set; }
        public int IncidentID { get; set; }
        public int ReportingOfficer { get; set; }
        public DateTime ReportDate { get; set; }
        public string ReportDetails { get; set; }
        public string Status { get; set; }

        public Report() { }

        public Report(int incidentId, int reportingOfficerID, DateTime reportDate, string reportDetails, string status)
        {
            IncidentID = incidentId;
            ReportingOfficer = reportingOfficerID;
            ReportDate = reportDate;
            ReportDetails = reportDetails;
            Status = status;
        }
    }
}

