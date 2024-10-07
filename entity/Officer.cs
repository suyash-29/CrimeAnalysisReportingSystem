using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimeAnalysisReportingSystem2.entity
{
    public class Officer
    {
        public int OfficerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BadgeNumber { get; set; }
        public string Rank { get; set; }
        public string ContactInfo { get; set; }
        public int AgencyID { get; set; }

        public Officer() { }

        public Officer(string firstName, string lastName, string badgeNumber, string rank, string contactInfo, int agencyId)
        {
            FirstName = firstName;
            LastName = lastName;
            BadgeNumber = badgeNumber;
            Rank = rank;
            ContactInfo = contactInfo;
            AgencyID = agencyId;
        }

        public static implicit operator int(Officer v)
        {
            throw new NotImplementedException();
        }
    }
}

