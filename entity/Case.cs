using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimeAnalysisReportingSystem2.entity
{
    public class Case
    {
        public int CaseID { get; set; }
        public string CaseDescription { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Incident> Incidents { get; set; }



        public Case()
        {
        }
        public Case(string caseDescription, List<Incident> incidents)
        {
            CaseDescription = caseDescription;
            Incidents = incidents;
            CreationDate = DateTime.Now;

        }
        
    }
}
