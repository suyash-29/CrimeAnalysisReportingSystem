using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimeAnalysisReportingSystem2.entity
{
    public class Incident
    {
        public int IncidentID { get; set; }
        public string IncidentType { get; set; }
        public DateTime IncidentDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public Incident() { }

        public Incident(int incidentID ,string incidentType, DateTime incidentDate, string location, string description, string status)
        {
            IncidentID = incidentID;
            IncidentType = incidentType;
            IncidentDate = incidentDate;
            Location = location;
            Description = description;
            Status = status;
        }
    }
}
