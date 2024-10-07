using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;


using CrimeAnalysisReportingSystem2.entity;
using CrimeAnalysisReportingSystem2.exception;
using CrimeAnalysisReportingSystem2.util;
using System.Data;
using System.Transactions; 
using System.Data.Common;
using Spectre.Console;

namespace CrimeAnalysisReportingSystem2.dao
{
    public class CrimeAnalysisServiceImpl : ICrimeAnalysisService
    {
        private static SqlConnection connection;

        public CrimeAnalysisServiceImpl()
        {
            connection = DBConnUtil.GetConnection();
        }

        public int CreateIncident(Incident incident, List<Victim> victims, List<Suspect> suspects)
        {
            SqlTransaction transaction = null;

            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();  
                }

                transaction = connection.BeginTransaction();  

                string insertIncident = "INSERT INTO Incidents (IncidentType, IncidentDate, Location, Description, Status) " +
                                        "OUTPUT INSERTED.IncidentID " +  
                                        "VALUES (@IncidentType, @IncidentDate, @Location, @Description, @Status)";
                SqlCommand cmdIncident = new SqlCommand(insertIncident, connection, transaction);
                cmdIncident.Parameters.AddWithValue("@IncidentType", incident.IncidentType);
                cmdIncident.Parameters.AddWithValue("@IncidentDate", incident.IncidentDate);
                cmdIncident.Parameters.AddWithValue("@Location", incident.Location);
                cmdIncident.Parameters.AddWithValue("@Description", incident.Description);
                cmdIncident.Parameters.AddWithValue("@Status", incident.Status);

                object result = cmdIncident.ExecuteScalar();  

                if (result == null || result == DBNull.Value)
                {
                    throw new InvalidOperationException("Failed to retrieve Incident ID. Incident may not have been inserted.");
                }

                int incidentID = Convert.ToInt32(result);  

                foreach (Victim victim in victims)
                {
                    InsertVictim(victim, transaction);  

                    string insertVictimLink = "INSERT INTO IncidentVictims (IncidentID, VictimID) VALUES (@IncidentID, " +
                                              "(SELECT MAX(VictimID) FROM Victims))";  
                    SqlCommand cmdLinkVictim = new SqlCommand(insertVictimLink, connection, transaction);
                    cmdLinkVictim.Parameters.AddWithValue("@IncidentID", incidentID);
                    cmdLinkVictim.ExecuteNonQuery();
                }

                foreach (Suspect suspect in suspects)
                {
                    InsertSuspect(suspect, transaction);  

                    string insertSuspectLink = "INSERT INTO IncidentSuspects (IncidentID, SuspectID) VALUES (@IncidentID, " +
                                               "(SELECT MAX(SuspectID) FROM Suspects))";  
                    SqlCommand cmdLinkSuspect = new SqlCommand(insertSuspectLink, connection, transaction);
                    cmdLinkSuspect.Parameters.AddWithValue("@IncidentID", incidentID);
                    cmdLinkSuspect.ExecuteNonQuery();
                }

                transaction.Commit(); 
                return incidentID;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();  
                }
                throw new DatabaseException("Error creating incident: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();  
                }
            }
        }


        public bool UpdateIncidentStatus(int incidentId, string status)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                string updateQuery = "UPDATE Incidents SET Status = @Status WHERE IncidentID = @IncidentID";
                SqlCommand cmd = new SqlCommand(updateQuery, connection);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@IncidentID", incidentId);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error updating incident status: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public bool AddSuspectsToIncident(int incidentID, List<Suspect> suspects)
        {
            SqlTransaction transaction = null;

            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                transaction = connection.BeginTransaction();  

                foreach (Suspect suspect in suspects)
                {
                    InsertSuspect(suspect, transaction);

                    string insertSuspectLink = "INSERT INTO IncidentSuspects (IncidentID, SuspectID) " +
                                               "VALUES (@IncidentID, (SELECT MAX(SuspectID) FROM Suspects))";  // Use MAX for simplicity
                    SqlCommand cmdLinkSuspect = new SqlCommand(insertSuspectLink, connection, transaction);
                    cmdLinkSuspect.Parameters.AddWithValue("@IncidentID", incidentID);
                    cmdLinkSuspect.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new DatabaseException("Error adding suspects to incident: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }



        public List<Incident> GetIncidentsInDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                string selectQuery = "SELECT * FROM Incidents WHERE IncidentDate BETWEEN @StartDate AND @EndDate";
                SqlCommand cmd = new SqlCommand(selectQuery, connection);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                SqlDataReader reader = cmd.ExecuteReader();

                List<Incident> incidents = new List<Incident>();
                while (reader.Read())
                {
                    incidents.Add(new Incident(
                        Convert.ToInt32(reader["IncidentID"]), 
                        reader["IncidentType"].ToString(),
                        Convert.ToDateTime(reader["IncidentDate"]).Date,
                        reader["Location"].ToString(),
                        reader["Description"].ToString(),
                        reader["Status"].ToString()
                    ));
                }
                reader.Close();
                return incidents;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error retrieving incidents: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public List<Incident> SearchIncidents(string incidentType)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                string selectQuery = "SELECT * FROM Incidents WHERE IncidentType LIKE @IncidentType";
                SqlCommand cmd = new SqlCommand(selectQuery, connection);
                cmd.Parameters.AddWithValue("@IncidentType", "%" + incidentType + "%");
                SqlDataReader reader = cmd.ExecuteReader();

                List<Incident> incidents = new List<Incident>();
                while (reader.Read())
                {
                    incidents.Add(new Incident(
                        Convert.ToInt32(reader["IncidentID"]),
                        reader["IncidentType"].ToString(),
                        Convert.ToDateTime(reader["IncidentDate"]).Date,
                        reader["Location"].ToString(),
                        reader["Description"].ToString(),
                        reader["Status"].ToString()
                    ));
                }
                reader.Close();
                return incidents;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error searching incidents: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public Officer GetOfficerDetails(int officerID)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                string query = "SELECT * FROM Officers WHERE OfficerID = @OfficerID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@OfficerID", officerID);

                SqlDataReader reader = cmd.ExecuteReader();
                Officer officer = null;

                if (reader.Read())
                {
                    officer = new Officer
                    {
                        OfficerID = Convert.ToInt32(reader["OfficerID"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        BadgeNumber = reader["BadgeNumber"].ToString(),
                        Rank = reader["Rank"].ToString(),
                        ContactInfo = reader["ContactInformation"].ToString()
                    };
                }
                reader.Close();

                return officer;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error retrieving officer details: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public Case CreateCase(string caseDescription, List<Incident> incidents)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                string insertCase = "INSERT INTO Cases (CaseDescription, CreationDate) VALUES (@CaseDescription, @CreationDate); SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(insertCase, connection);
                cmd.Parameters.AddWithValue("@CaseDescription", caseDescription);
                cmd.Parameters.AddWithValue("@CreationDate", DateTime.Now);
                int caseID = Convert.ToInt32(cmd.ExecuteScalar());

                foreach (Incident incident in incidents)
                {
                    string insertCaseIncident = "INSERT INTO CaseIncidents (CaseID, IncidentID) VALUES (@CaseID, @IncidentID)";
                    SqlCommand cmdIncident = new SqlCommand(insertCaseIncident, connection);
                    cmdIncident.Parameters.AddWithValue("@CaseID", caseID);
                    cmdIncident.Parameters.AddWithValue("@IncidentID", incident.IncidentID);
                    cmdIncident.ExecuteNonQuery();
                }

                return new Case(caseDescription, incidents)
                {
                    CaseID = caseID,
                    CreationDate = DateTime.Now
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error creating case: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        

        private List<Incident> GetIncidentsByCase(int caseId)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                
                string selectIncidents = "SELECT Incidents.* FROM Incidents " +
                                         "JOIN CaseIncidents ON Incidents.IncidentID = CaseIncidents.IncidentID " +
                                         "WHERE CaseIncidents.CaseID = @CaseID";

                SqlCommand cmd = new SqlCommand(selectIncidents, connection);
                cmd.Parameters.AddWithValue("@CaseID", caseId);
                SqlDataReader reader = cmd.ExecuteReader();

                List<Incident> incidents = new List<Incident>();
                while (reader.Read())
                {
                    incidents.Add(new Incident(
                        Convert.ToInt32(reader["IncidentID"]), 
                        reader["IncidentType"].ToString(),
                        Convert.ToDateTime(reader["IncidentDate"]).Date,
                        reader["Location"].ToString(),
                        reader["Description"].ToString(),
                        reader["Status"].ToString()
                    ));
                }
                reader.Close();
                return incidents;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error fetching incidents for the case: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public bool UpdateCaseDetails(Case caseObj, List<Incident> newIncidents = null)
        {
            SqlTransaction transaction = null;
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                transaction = connection.BeginTransaction();

                string updateCase = "UPDATE Cases SET CaseDescription = @CaseDescription WHERE CaseID = @CaseID";
                SqlCommand cmd = new SqlCommand(updateCase, connection, transaction);
                cmd.Parameters.AddWithValue("@CaseDescription", caseObj.CaseDescription);
                cmd.Parameters.AddWithValue("@CaseID", caseObj.CaseID);
                cmd.ExecuteNonQuery();

                if (newIncidents != null && newIncidents.Count > 0)
                {
                    foreach (var incident in newIncidents)
                    {
                        string insertIncident = "INSERT INTO CaseIncidents (CaseID, IncidentID) VALUES (@CaseID, @IncidentID)";
                        SqlCommand incidentCmd = new SqlCommand(insertIncident, connection, transaction);
                        incidentCmd.Parameters.AddWithValue("@CaseID", caseObj.CaseID);
                        incidentCmd.Parameters.AddWithValue("@IncidentID", incident.IncidentID);
                        incidentCmd.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback(); 
                }
                throw new DatabaseException("Error updating case and incidents: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public Case GetBasicCaseDetails(int caseId)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string selectCase = "SELECT * FROM Cases WHERE CaseID = @CaseID";
                SqlCommand cmd = new SqlCommand(selectCase, connection);
                cmd.Parameters.AddWithValue("@CaseID", caseId);
                SqlDataReader reader = cmd.ExecuteReader();

                Case caseObj = null;
                if (reader.Read())
                {
                    caseObj = new Case
                    {
                        CaseID = caseId,
                        CaseDescription = reader["CaseDescription"].ToString(),
                        CreationDate = Convert.ToDateTime(reader["CreationDate"]).Date,
                    };
                }
                reader.Close();
                return caseObj;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error fetching case details: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }







        public Incident GetIncidentDetails(int incidentId)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                string selectIncident = "SELECT * FROM Incidents WHERE IncidentID = @IncidentID";
                SqlCommand cmd = new SqlCommand(selectIncident, connection);
                cmd.Parameters.AddWithValue("@IncidentID", incidentId);

                SqlDataReader reader = cmd.ExecuteReader();
                Incident incident = null;

                if (reader.Read())
                {
                    incident = new Incident
                    {
                        IncidentID = Convert.ToInt32(reader["IncidentID"]),
                        IncidentType = reader["IncidentType"].ToString(),
                        IncidentDate = Convert.ToDateTime(reader["IncidentDate"]).Date,
                        Location = reader["Location"].ToString(),
                        Description = reader["Description"].ToString(),
                        Status = reader["Status"].ToString()
                    };
                }

                reader.Close();

                if (incident == null)
                {
                    AnsiConsole.WriteLine($"[yellow]Incident with ID {incidentId} not found.[/]");
                }

                return incident;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error retrieving incident details: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public Report GenerateIncidentReport(Incident incident)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string checkQuery = "SELECT COUNT(*) FROM Reports WHERE IncidentID = @IncidentID";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@IncidentID", incident.IncidentID);

                    int reportCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (reportCount > 0)
                    {
                        AnsiConsole.MarkupLine($"[yellow]A report for Incident ID {incident.IncidentID} already exists. Please try another incident ID.[/]");
                        return null;  
                    }
                }

                AnsiConsole.Markup("[cyan]Enter Reporting Officer ID:[/]");
                int reportingOfficerID = Convert.ToInt32(Console.ReadLine());

                AnsiConsole.Markup("[cyan]Enter Report Description:[/]");
                string reportDescription = Console.ReadLine();

                AnsiConsole.Markup("[cyan]Enter Report Status:[/]");
                string reportStatus = Console.ReadLine();

                string insertReportQuery = "INSERT INTO Reports (IncidentID, ReportingOfficer, ReportDate, ReportDetails, Status) " +
                                           "VALUES (@IncidentID, @ReportingOfficer, @ReportDate, @ReportDetails, @Status); " +
                                           "SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(insertReportQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@IncidentID", incident.IncidentID);
                    cmd.Parameters.AddWithValue("@ReportingOfficer", reportingOfficerID);
                    cmd.Parameters.AddWithValue("@ReportDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ReportDetails", reportDescription);
                    cmd.Parameters.AddWithValue("@Status", reportStatus);

                    int reportID = Convert.ToInt32(cmd.ExecuteScalar());

                    return new Report
                    {
                        ReportID = reportID,
                        IncidentID = incident.IncidentID,
                        ReportingOfficer = reportingOfficerID,
                        ReportDate = DateTime.Now,
                        ReportDetails = reportDescription,
                        Status = reportStatus
                    };
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error generating incident report: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public bool UpdateReportDetails(int incidentID, string newReportDetails, string newStatus)
        {
            SqlTransaction transaction = null;
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                transaction = connection.BeginTransaction();

                Report report = GetReportDetails(incidentID, transaction);

                if (report == null)
                {
                    throw new Exception("Report not found for this incident.");
                }

                report.ReportDetails = newReportDetails;
                report.Status = newStatus;

                string updateReportQuery = "UPDATE Reports SET ReportDetails = @ReportDetails, Status = @Status WHERE ReportID = @ReportID";
                SqlCommand cmd = new SqlCommand(updateReportQuery, connection, transaction);
                cmd.Parameters.AddWithValue("@ReportDetails", report.ReportDetails);
                cmd.Parameters.AddWithValue("@Status", report.Status);
                cmd.Parameters.AddWithValue("@ReportID", report.ReportID);

                int rowsAffected = cmd.ExecuteNonQuery();

                transaction.Commit();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new DatabaseException("Error updating report details: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }




       

        private bool InsertVictim(Victim victim, SqlTransaction transaction)
        {
            try
            {
                string insertVictim = "INSERT INTO Victims (FirstName, LastName, DateOfBirth, Gender, ContactInfo) " +
                                      "VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @ContactInfo)";
                SqlCommand cmd = new SqlCommand(insertVictim, connection, transaction);
                cmd.Parameters.AddWithValue("@FirstName", victim.FirstName);
                cmd.Parameters.AddWithValue("@LastName", victim.LastName);
                cmd.Parameters.AddWithValue("@DateOfBirth", victim.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", victim.Gender);
                cmd.Parameters.AddWithValue("@ContactInfo", victim.ContactInfo);

                int affectedRows = cmd.ExecuteNonQuery();  
                return affectedRows > 0;  
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error adding victim: " + ex.Message, ex);
            }
        }

        private bool InsertSuspect(Suspect suspect, SqlTransaction transaction)
        {
            try
            {
                string insertSuspect = "INSERT INTO Suspects (FirstName, LastName, DateOfBirth, Gender, ContactInfo) " +
                                       "VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @ContactInfo)";
                SqlCommand cmd = new SqlCommand(insertSuspect, connection, transaction);
                cmd.Parameters.AddWithValue("@FirstName", suspect.FirstName);
                cmd.Parameters.AddWithValue("@LastName", suspect.LastName);
                cmd.Parameters.AddWithValue("@DateOfBirth", suspect.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", suspect.Gender);
                cmd.Parameters.AddWithValue("@ContactInfo", suspect.ContactInfo);

                int affectedRows = cmd.ExecuteNonQuery();  
                return affectedRows > 0;  
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error adding suspect: " + ex.Message, ex);
            }
        }


        public DetailedIncidentReport GetDetailedIncidentReport(int incidentID)
        {
            SqlTransaction transaction = null;

            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                transaction = connection.BeginTransaction();

                Incident incident = GetIncidentDetails(incidentID, transaction);
                Report report = GetReportDetails(incidentID, transaction);
                Officer officer = GetOfficerDetails(report.ReportingOfficer, transaction);
                List<Suspect> suspects = GetSuspectsForIncident(incidentID, transaction);
                List<Victim> victims = GetVictimsForIncident(incidentID, transaction);

                transaction.Commit();

                return new DetailedIncidentReport
                {
                    Incident = incident,
                    Report = report,
                    ReportingOfficer = officer,
                    Suspects = suspects,
                    Victims = victims,
                };
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new DatabaseException("Error retrieving detailed incident report: " + ex.Message, ex);
            }
            finally
            {
                // Ensure the connection is closed after execution
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

        }


        public DetailedCaseReport GetCaseDetails(int caseId)
        {
            SqlTransaction transaction = null;
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                transaction = connection.BeginTransaction();

                string selectCase = "SELECT * FROM Cases WHERE CaseID = @CaseID";
                SqlCommand cmd = new SqlCommand(selectCase, connection , transaction);
                cmd.Parameters.AddWithValue("@CaseID", caseId);
                SqlDataReader reader = cmd.ExecuteReader();

                DetailedCaseReport detailedCaseReport = null;
                if (reader.Read())
                {
                    Case caseObj = new Case
                    {
                        CaseID = caseId,
                        CaseDescription = reader["CaseDescription"].ToString(),
                        CreationDate = Convert.ToDateTime(reader["CreationDate"]).Date,
                    };

                    List<DetailedIncidentReport> detailedIncidents = FetchIncidentsForCase(caseId, transaction);

                    detailedCaseReport = new DetailedCaseReport
                    {
                        CaseInfo = caseObj,
                        DetailedIncidents = detailedIncidents
                    };
                }
                reader.Close();
                transaction.Commit();
                return detailedCaseReport ;   ;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                if (transaction != null)
                {
                    transaction.Rollback(); 
                }
                throw new DatabaseException("Error fetching case details: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public List<DetailedCaseReport> GetAllCases()
        {
            SqlTransaction transaction = null;
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();
                string selectAllCases = "SELECT * FROM Cases";
                SqlCommand cmd = new SqlCommand(selectAllCases, connection , transaction);
                SqlDataReader reader = cmd.ExecuteReader();

                List<DetailedCaseReport> cases = new List<DetailedCaseReport>();
                while (reader.Read())
                {
                    int caseId = Convert.ToInt32(reader["CaseID"]);
                    Case caseObj = new Case
                    {
                        CaseID = caseId,
                        CaseDescription = reader["CaseDescription"].ToString(),
                        CreationDate = Convert.ToDateTime(reader["CreationDate"]).Date,
                    };
                    List<DetailedIncidentReport> detailedIncidents = FetchIncidentsForCase(caseId, transaction);

                    DetailedCaseReport detailedCaseReport = new DetailedCaseReport
                    {
                        CaseInfo = caseObj,
                        DetailedIncidents = detailedIncidents
                    };

                    cases.Add(detailedCaseReport);
                }
                reader.Close();
                transaction.Commit();
                return cases;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                throw new DatabaseException("Error retrieving all cases: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public List<DetailedIncidentReport> FetchIncidentsForCase(int caseId, SqlTransaction transaction)
        {
            List<DetailedIncidentReport> incidentReports = new List<DetailedIncidentReport>();

            string query = "SELECT * FROM Incidents WHERE IncidentID IN " +
                           "(SELECT IncidentID FROM CaseIncidents WHERE CaseID = @CaseID)";
            SqlCommand cmd = new SqlCommand(query, connection, transaction);
            cmd.Parameters.AddWithValue("@CaseID", caseId);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int incidentID = Convert.ToInt32(reader["IncidentID"]);

                Incident incident = GetIncidentDetails(incidentID, transaction);
                Report report = GetReportDetails(incidentID, transaction);
                Officer officer = GetOfficerDetails(report.ReportingOfficer, transaction);
                List<Suspect> suspects = GetSuspectsForIncident(incidentID, transaction);
                List<Victim> victims = GetVictimsForIncident(incidentID, transaction);

                DetailedIncidentReport detailedIncidentReport = new DetailedIncidentReport
                {
                    Incident = incident,
                    Report = report,
                    ReportingOfficer = officer,
                    Suspects = suspects,
                    Victims = victims
                };

                incidentReports.Add(detailedIncidentReport);
            }

            reader.Close();
            return incidentReports;
        }

        public Report GetBasicReportDetails(int incidentID)
        {
            Report report = null;

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string query = "SELECT * FROM Reports WHERE IncidentID = @IncidentID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@IncidentID", incidentID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        report = new Report
                        {
                            ReportID = Convert.ToInt32(reader["ReportID"]),
                            IncidentID = Convert.ToInt32(reader["IncidentID"]),
                            ReportingOfficer = Convert.ToInt32(reader["ReportingOfficer"]),
                            ReportDate = Convert.ToDateTime(reader["ReportDate"]).Date,
                            ReportDetails = reader["ReportDetails"].ToString(),
                            Status = reader["Status"].ToString()
                        };
                    }
                }
            }

            return report;
        }



        // Private method to retrieve incident details
        private Incident GetIncidentDetails(int incidentID, SqlTransaction transaction)
        {
            string query = "SELECT * FROM Incidents WHERE IncidentID = @IncidentID";
            SqlCommand cmd = new SqlCommand(query, connection, transaction);
            cmd.Parameters.AddWithValue("@IncidentID", incidentID);

            SqlDataReader reader = cmd.ExecuteReader();
            Incident incident = null;

            if (reader.Read())
            {
                incident = new Incident
                {
                    IncidentID = Convert.ToInt32(reader["IncidentID"]),
                    IncidentType = reader["IncidentType"].ToString(),
                    IncidentDate = Convert.ToDateTime(reader["IncidentDate"]),
                    Location = reader["Location"].ToString(),
                    Description = reader["Description"].ToString(),
                    Status = reader["Status"].ToString()
                };
            }
            reader.Close();
            return incident;
        }

        // Private method to retrieve report details
        private Report GetReportDetails(int incidentID, SqlTransaction transaction)
        {

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            string query = "SELECT * FROM Reports WHERE IncidentID = @IncidentID";
            SqlCommand cmd = new SqlCommand(query, connection, transaction);
            cmd.Parameters.AddWithValue("@IncidentID", incidentID);

            SqlDataReader reader = cmd.ExecuteReader();
            Report report = null;

            if (reader.Read())
            {
                report = new Report
                {
                    ReportID = Convert.ToInt32(reader["ReportID"]),
                    IncidentID = Convert.ToInt32(reader["IncidentID"]),
                    ReportingOfficer = Convert.ToInt32(reader["ReportingOfficer"]),
                    ReportDate = Convert.ToDateTime(reader["ReportDate"]).Date,
                    ReportDetails = reader["ReportDetails"].ToString(),
                    Status = reader["Status"].ToString()
                };
            }
            reader.Close();
            return report;
           
        }

        // Private method to retrieve officer details
        private Officer GetOfficerDetails(int officerID, SqlTransaction transaction)
        {
            string query = "SELECT * FROM Officers WHERE OfficerID = @OfficerID";
            SqlCommand cmd = new SqlCommand(query, connection, transaction);
            cmd.Parameters.AddWithValue("@OfficerID", officerID);

            SqlDataReader reader = cmd.ExecuteReader();
            Officer officer = null;

            if (reader.Read())
            {
                officer = new Officer
                {
                    OfficerID = Convert.ToInt32(reader["OfficerID"]),
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    BadgeNumber = reader["BadgeNumber"].ToString(),
                    Rank = reader["Rank"].ToString(),
                    ContactInfo = reader["ContactInfo"].ToString()
                };
            }
            reader.Close();
            return officer;
        }

        // Private method to retrieve suspects linked to the incident
        private List<Suspect> GetSuspectsForIncident(int incidentID, SqlTransaction transaction)
        {
            List<Suspect> suspects = new List<Suspect>();
            string query = "SELECT S.SuspectID, S.FirstName, S.LastName FROM Suspects S " +
                           "INNER JOIN IncidentSuspects ISU ON S.SuspectID = ISU.SuspectID " +
                           "WHERE ISU.IncidentID = @IncidentID";
            SqlCommand cmd = new SqlCommand(query, connection, transaction);
            cmd.Parameters.AddWithValue("@IncidentID", incidentID);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                suspects.Add(new Suspect
                {
                    SuspectID = Convert.ToInt32(reader["SuspectID"]),
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString()
                });
            }
            reader.Close();
            return suspects;
        }

        // Private method to retrieve victims linked to the incident
        private List<Victim> GetVictimsForIncident(int incidentID, SqlTransaction transaction)
        {
            List<Victim> victims = new List<Victim>();
            string query = "SELECT V.VictimID, V.FirstName, V.LastName FROM Victims V " +
                           "INNER JOIN IncidentVictims IV ON V.VictimID = IV.VictimID " +
                           "WHERE IV.IncidentID = @IncidentID";
            SqlCommand cmd = new SqlCommand(query, connection, transaction);
            cmd.Parameters.AddWithValue("@IncidentID", incidentID);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                victims.Add(new Victim
                {
                    VictimID = Convert.ToInt32(reader["VictimID"]),
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString()
                });
            }
            reader.Close();
            return victims;
        }

        private int? GetCaseIDForIncident(int incidentID, SqlTransaction transaction)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = "SELECT CaseID FROM IncidentCases WHERE IncidentID = @IncidentID";  // Assuming this table links incidents and cases
                SqlCommand cmd = new SqlCommand(query, connection, transaction);
                cmd.Parameters.AddWithValue("@IncidentID", incidentID);

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return null;  
                }

                return Convert.ToInt32(result);  
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error retrieving CaseID for incident: " + ex.Message, ex);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }




    }
}

