using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CrimeAnalysisReportingSystem2.exception
{
    public class IncidentNotFoundException : Exception
    {
        public IncidentNotFoundException(string message) : base(message) { }
    }
}

