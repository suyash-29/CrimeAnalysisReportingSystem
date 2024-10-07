using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CrimeAnalysisReportingSystem2.entity
{
    public class Suspect
    {
        public int SuspectID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string ContactInfo { get; set; }

        public Suspect() { }

        public Suspect(string firstName, string lastName, DateTime dateOfBirth, string gender, string contactInfo)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            ContactInfo = contactInfo;
        }
    }
}

