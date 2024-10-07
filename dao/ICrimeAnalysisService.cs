using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CrimeAnalysisReportingSystem2.entity;

namespace CrimeAnalysisReportingSystem2.dao
{
    public interface ICrimeAnalysisService
    {
        int CreateIncident(Incident incident, List<Victim> victims, List<Suspect> suspects);
        bool UpdateIncidentStatus(int incidentId, string status);
        List<Incident> GetIncidentsInDateRange(DateTime startDate, DateTime endDate);
        List<Incident> SearchIncidents(string incidentType);

        Case CreateCase(string caseDescription, List<Incident> incidents);
        DetailedCaseReport GetCaseDetails(int caseId);
        bool UpdateCaseDetails(Case caseObj, List<Incident> newIncidents = null);
        Case GetBasicCaseDetails(int caseId);

        List<DetailedCaseReport> GetAllCases();

        Incident GetIncidentDetails(int incidentId);
        Officer GetOfficerDetails(int officerID);

        Report GenerateIncidentReport(Incident incident);

       
        bool AddSuspectsToIncident(int incidentID, List<Suspect> suspects);


        DetailedIncidentReport GetDetailedIncidentReport(int incidentID);


        bool UpdateReportDetails(int incidentID, string newReportDetails, string newStatus);

        Report GetBasicReportDetails(int incidentID);
        }
}

