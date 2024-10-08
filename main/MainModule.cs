using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CrimeAnalysisReportingSystem2.dao;
using CrimeAnalysisReportingSystem2.entity;
using CrimeAnalysisReportingSystem2.exception;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Data;
using Spectre.Console;



namespace CrimeAnalysisReportingSystem2.main
{
    public class MainModule
    {

        public static void Main(string[] args)
        {
            ICrimeAnalysisService crimeService = new CrimeAnalysisServiceImpl();
            bool exit = false;

            while (!exit)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new Panel("[bold yellow]Welcome to the Crime Analysis and Reporting System[/]")
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1),
                    Header = new PanelHeader("[bold cyan]Main Menu[/]", Justify.Center)
                });

                AnsiConsole.MarkupLine("[bold green]1.[/] Access [bold]Incidents[/] Menu");
                AnsiConsole.MarkupLine("[bold green]2.[/] Access [bold]Cases[/] Menu");
                AnsiConsole.MarkupLine("[bold green]3.[/] Access [bold]Reports[/] Menu");
                AnsiConsole.MarkupLine("[bold red]4.[/] Exit the Application");

                int mainChoice = GetUserInputChoice();

                switch (mainChoice)
                {
                    case 1:
                        AccessIncidentsMenu(crimeService);
                        break;
                    case 2:
                        AccessCasesMenu(crimeService);
                        break;
                    case 3:
                        AccessReportsMenu(crimeService);
                        break;
                    case 4:
                        AnsiConsole.MarkupLine("[bold yellow]Thank you for using the Crime Analysis and Reporting System. Goodbye![/]");
                        exit = true;
                        break;
                    default:
                        AnsiConsole.MarkupLine("[bold red]Invalid selection. Please choose a valid option.[/]");
                        break;
                }
            }
        }

        private static void AccessIncidentsMenu(ICrimeAnalysisService crimeService)
        {
            bool incidentsMenu = true;
            while (incidentsMenu)
            {
                AnsiConsole.Write(new Panel("[bold blue]Incidents Menu[/]")
                {
                    Border = BoxBorder.Double,
                    Padding = new Padding(1/2, 1/2),
                    Header = new PanelHeader("Options", Justify.Center)
                });

                AnsiConsole.MarkupLine("[bold green]1.[/] Create a New Incident");
                AnsiConsole.MarkupLine("[bold green]2.[/] Add Suspects to an Incident");

                AnsiConsole.MarkupLine("[bold green]3.[/] Update the Status of an Incident");
                AnsiConsole.MarkupLine("[bold green]4.[/] Retrieve Incidents Within a Date Range");
                AnsiConsole.MarkupLine("[bold green]5.[/] Search for Specific Incident Type");
                AnsiConsole.MarkupLine("[bold red]6.[/] Return to Main Menu");
                

                int incidentChoice = GetUserInputChoice();

                switch (incidentChoice)
                {
                    case 1:
                        CreateIncident(crimeService);
                        break;
                    case 2:
                        AddSuspectsToIncident(crimeService);
                        
                        break;
                    case 3:
                        UpdateIncidentStatus(crimeService);
                        
                        break;
                    case 4:
                        GetIncidentsInDateRange(crimeService);
                       
                        break;
                    case 5:
                        SearchIncidents(crimeService);
                        break;
                    case 6:
                        incidentsMenu = false; 
                        break;
                    default:
                        AnsiConsole.MarkupLine("[bold red]Invalid selection. Please choose a valid option.[/]");
                        break;
                }
            }
        }

        private static void AccessCasesMenu(ICrimeAnalysisService crimeService)
        {
            bool casesMenu = true;
            while (casesMenu)
            {
                AnsiConsole.Write(new Panel("[bold blue]Cases Menu[/]")
                {
                    Border = BoxBorder.Double,
                    Padding = new Padding(1/2, 1/2),
                    Header = new PanelHeader("Options", Justify.Center)
                });

                AnsiConsole.MarkupLine("[bold green]1.[/] Create a New Case");
                AnsiConsole.MarkupLine("[bold green]2.[/] Retrieve Case Details");
                AnsiConsole.MarkupLine("[bold green]3.[/] Update Case Information");
                AnsiConsole.MarkupLine("[bold green]4.[/] View All Cases");
                AnsiConsole.MarkupLine("[bold red]5.[/] Return to Main Menu");

                int caseChoice = GetUserInputChoice();

                switch (caseChoice)
                {
                    case 1:
                        CreateCase(crimeService);
                        break;
                    case 2:
                        GetCaseDetails(crimeService);
                        break;
                    case 3:
                        UpdateCaseDetails(crimeService);
                        break;
                    case 4:
                        GetAllCases(crimeService);
                        break;
                    case 5:
                        casesMenu = false; 
                        break;
                    default:
                        AnsiConsole.MarkupLine("[bold red]Invalid selection. Please choose a valid option.[/]");
                        break;
                }
            }
        }

        private static void AccessReportsMenu(ICrimeAnalysisService crimeService)
        {
            bool reportsMenu = true;
            while (reportsMenu)
            {
                AnsiConsole.Write(new Panel("[bold blue]Reports Menu[/]")
                {
                    Border = BoxBorder.Double,
                    Padding = new Padding(1/2, 1/2),
                    Header = new PanelHeader("Options", Justify.Center)
                });

                AnsiConsole.MarkupLine("[bold green]1.[/] File an Incident Report");
                AnsiConsole.MarkupLine("[bold green]2.[/] Display Incident Report");
                AnsiConsole.MarkupLine("[bold green]3.[/] Update Incident Report");
                AnsiConsole.MarkupLine("[bold red]4.[/] Return to Main Menu");

                int reportChoice = GetUserInputChoice();

                switch (reportChoice)
                {
                    case 1:
                        GenerateIncidentReport(crimeService);
                        break;
                    case 2:
                        DisplayIncidentReport(crimeService);
                        break;
                    case 3:
                        UpdateReportDetails(crimeService);
                        break;
                    case 4:
                        reportsMenu = false; 
                        break;
                    default:
                        AnsiConsole.MarkupLine("[bold red]Invalid selection. Please choose a valid option.[/]");
                        break;
                }
            }
        }

        private static int GetUserInputChoice()
        {
            AnsiConsole.Markup("[bold yellow]Your choice: [/]");
            return int.Parse(Console.ReadLine());
        }
    





//Incidents
       public static void CreateIncident(ICrimeAnalysisService crimeService)
        {
            try
            {
                AnsiConsole.Markup("[bold cyan]Enter Incident Type:[/] ");
                string type = Console.ReadLine();
                DateTime date;
                string input;
                do
                {
                    AnsiConsole.Markup("[cyan]Enter Incident Date[/][bold yellow] (yyyy-mm-dd):[/] ");
                    input = Console.ReadLine(); 

                    if (!DateTime.TryParse(input, out date))
                    {
                        AnsiConsole.MarkupLine("[bold red]Invalid input[/] Please enter the Incident Date in the format [bold red]'yyyy-mm-dd'[/]");
                    }
                } while (!DateTime.TryParse(input, out date));
                AnsiConsole.Markup("[cyan]Enter Location:[/] ");
                string location = Console.ReadLine();
                AnsiConsole.Markup("[cyan]Enter Description:[/] ");
                string description = Console.ReadLine();
                string[] validStatuses = { "open", "closed", "under investigation" };
                string status;

                do
                {
                    AnsiConsole.Markup("[cyan]Enter Status (open, closed, under investigation):[/] ");
                    status = Console.ReadLine();
                    if (!validStatuses.Contains(status?.ToLower()))
                    {
                        AnsiConsole.MarkupLine("[bold red]Invalid input[/]");
                    }
                } while (!validStatuses.Contains(status?.ToLower()));

                Incident incident = new Incident(0, type, date, location, description, status);

                // taking input formultiple victims
                List<Victim> victims = new List<Victim>();
                AnsiConsole.Markup("[cyan]Enter number of victims:[/]");
                int victimCount = int.Parse(Console.ReadLine());
                for (int i = 0; i < victimCount; i++)
                {
                    AnsiConsole.MarkupLine($"[cyan]Enter details for Victim [/][bold yellow]{i + 1}[/]");
                    AnsiConsole.Markup("[cyan]First Name:[/] ");
                    string firstName = Console.ReadLine();
                    AnsiConsole.Markup("[cyan]Last Name:[/] ");
                    string lastName = Console.ReadLine();

                    DateTime dob;
                    string InputValue;
                    do
                    {
                        AnsiConsole.Markup("[cyan]Enter Date of Birth [/][bold yellow](yyyy-mm-dd):[/] ");
                        InputValue = Console.ReadLine();
                        if (!DateTime.TryParse(InputValue, out dob))
                        {
                            AnsiConsole.MarkupLine("[red bold]Invalid input.[/] Please enter the Incident Date in the format [bold red]'yyyy-mm-dd'.[/]");
                        }
                    } while (!DateTime.TryParse(InputValue, out dob));

                    AnsiConsole.Markup("[cyan]Gender:[/] ");
                    string gender = Console.ReadLine();
                    AnsiConsole.Markup("[cyan]Contact Info:[/] ");
                    string contactInfo = Console.ReadLine();

                    victims.Add(new Victim( firstName, lastName, dob, gender, contactInfo));
                }

                // taking multiple suspects 
                List<Suspect> suspects = new List<Suspect>();
                AnsiConsole.Markup("[cyan]Enter number of suspects :[/]");
                int suspectCount = int.Parse(Console.ReadLine());
                for (int i = 0; i < suspectCount; i++)
                {
                    AnsiConsole.MarkupLine($"[cyan]Enter details for Suspect [/][bold yellow]{i + 1}:[/]");
                    AnsiConsole.Markup("[cyan]First Name:[/] ");
                    string firstName = Console.ReadLine();
                    AnsiConsole.Markup("[cyan]Last Name:[/] ");
                    string lastName = Console.ReadLine();

                    DateTime dob;
                    string InputValue;
                    do
                    {
                        AnsiConsole.Markup("[cyan]Enter Date of Birth [/][bold yellow](yyyy-mm-dd):[/] ");
                        InputValue = Console.ReadLine();
                        if (!DateTime.TryParse(InputValue, out dob))
                        {
                            AnsiConsole.MarkupLine("[bold red]Invalid input.[/] Please enter the Incident Date in the format[bold red] 'yyyy-mm-dd'.[/]");
                        }
                    } while (!DateTime.TryParse(InputValue, out dob));


                    AnsiConsole.Markup("[cyan]Gender:[/] ");
                    string gender = Console.ReadLine();
                    AnsiConsole.Markup("[cyan]Contact Info:[/] ");
                    string contactInfo = Console.ReadLine();

                    suspects.Add(new Suspect( firstName, lastName, dob, gender, contactInfo));
                }

                int newIncidentId = crimeService.CreateIncident(incident, victims, suspects);
                AnsiConsole.MarkupLine($"[bold green]Incident Created Successfully with [/][bold yellow]Incident ID: {newIncidentId} [/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red]Error:[/] " + ex.Message);
            }
        }


        public static void UpdateIncidentStatus(ICrimeAnalysisService crimeService)
        {
            try
            {
                AnsiConsole.Markup("[cyan]Enter Incident ID:[/] ");
                int incidentId = int.Parse(Console.ReadLine());
                string[] validStatuses = { "open", "closed", "under investigation" };
                string status;

                do
                {
                    AnsiConsole.Markup("[cyan]Enter Status (open, closed, under investigation): [/]");
                    status = Console.ReadLine();
                    if (!validStatuses.Contains(status?.ToLower()))
                    {
                        AnsiConsole.MarkupLine("[bold red]Invalid input[/]");
                    }
                } while (!validStatuses.Contains(status?.ToLower()));
                crimeService.UpdateIncidentStatus(incidentId, status);
                AnsiConsole.MarkupLine("[bold green]Incident Status Updated Successfully.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red]Error:[/] " + ex.Message);
            }
        }

        public static void AddSuspectsToIncident(ICrimeAnalysisService crimeService)
        {
            AnsiConsole.Markup("[cyan]Enter Incident ID to add suspects to:[/]");
            int incidentID = Convert.ToInt32(Console.ReadLine());

            List<Suspect> suspects = new List<Suspect>();

            AnsiConsole.Markup("[cyan]Enter the number of suspects to add:[/]");
            int numSuspects = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < numSuspects; i++)
            {
                AnsiConsole.MarkupLine($"[cyan]Enter details for Suspect[/][yellow bold] {i + 1}[/]");

                AnsiConsole.Markup("[cyan]First Name:[/] ");
                string firstName = Console.ReadLine();

                AnsiConsole.Markup("[cyan]Last Name:[/] ");
                string lastName = Console.ReadLine();

                DateTime dob;
                while (true)  
                {
                    AnsiConsole.Markup("[cyan]Date of Birth[/][bold yellow] (yyyy-mm-dd):[/] ");
                    if (DateTime.TryParse(Console.ReadLine(), out dob)) break;
                    AnsiConsole.MarkupLine("[Bold red]Invalid date format.[/] Please try again.");
                }

                AnsiConsole.Markup("[cyan]Gender:[/] ");
                string gender = Console.ReadLine();

                AnsiConsole.Markup("[cyan]Contact Info:[/] ");
                string contactInfo = Console.ReadLine();

                Suspect newSuspect = new Suspect
                {
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dob,
                    Gender = gender,
                    ContactInfo = contactInfo
                };

                suspects.Add(newSuspect);
            }

            bool success = crimeService.AddSuspectsToIncident(incidentID, suspects);

            if (success)
            {
                AnsiConsole.MarkupLine("[bold green]Suspects successfully added to the incident.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red ]Failed to add suspects to the incident.[/]");
            }
        }

        public static void GetIncidentsInDateRange(ICrimeAnalysisService crimeService)
        {
            try
            {
                AnsiConsole.MarkupLine("[bold cyan]Enter Date Range to Retrieve Incidents[/]\n");
                DateTime startDate;
                string input;
                do
                {
                    AnsiConsole.Markup("[cyan]Enter Start Date[/][bold yellow] (yyyy-mm-dd)[/] ");
                    input = Console.ReadLine();

                    if (!DateTime.TryParse(input, out startDate))
                    {
                        AnsiConsole.MarkupLine("[bold red]Invalid input.[/] Please enter the Start Date in the format [bold red] 'yyyy-mm-dd'.[/]");
                    }
                } while (!DateTime.TryParse(input, out startDate));

                
                DateTime endDate;
                string input1;
                do
                {
                    AnsiConsole.Markup("[cyan]Enter End Date[/] [bold yellow](yyyy-mm-dd):[/] ");
                    input1 = Console.ReadLine();

                    if (!DateTime.TryParse(input1, out endDate))
                    {
                        AnsiConsole.MarkupLine("[bold red]Invalid input.[/] Please enter the End Date in the format [bold red]'yyyy-mm-dd'[/].");
                    }
                } while (!DateTime.TryParse(input1, out endDate));


                List<Incident> incidents = crimeService.GetIncidentsInDateRange(startDate, endDate);
                AnsiConsole.MarkupLine("[bold green]Incidents in Date Range:[/]");
                
                var table = new Table()
                       .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("[bold cyan]ID[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Type[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Date[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Description[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Status[/]").Centered());

                foreach (Incident incident in incidents)
                {
                    table.AddRow(
                        incident.IncidentID.ToString(),
                        incident.IncidentType,
                        incident.IncidentDate.ToString("yyyy/MM/dd"),
                        incident.Description,
                        incident.Status
                    );
                }

                AnsiConsole.Write(table);
            }
            catch (Exception ex)
            {
                throw;
                AnsiConsole.MarkupLine("[bold red]Error:[/] " + ex.Message);
                
            }
        }

        public static void SearchIncidents(ICrimeAnalysisService crimeService)
        {
            try
            {
                AnsiConsole.Markup("[cyan]Enter Incident Type to Search:[/] ");
                string type = Console.ReadLine();
                List<Incident> incidents = crimeService.SearchIncidents(type);

                AnsiConsole.MarkupLine("[bold green]Search Results:[/]");
               
                var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn(new TableColumn("[bold cyan]ID[/]").Centered())
            .AddColumn(new TableColumn("[bold cyan]Type[/]").Centered())
            .AddColumn(new TableColumn("[bold cyan]Date[/]").Centered())
            .AddColumn(new TableColumn("[bold cyan]Description[/]").Centered())
            .AddColumn(new TableColumn("[bold cyan]Location[/]").Centered())
            .AddColumn(new TableColumn("[bold cyan]Status[/]").Centered());

                if (incidents.Count > 0)
                {
                    foreach (Incident incident in incidents)
                    {
                        table.AddRow(
                            incident.IncidentID.ToString(),
                            incident.IncidentType,
                            incident.IncidentDate.ToString("yyyy/MM/dd"),
                            incident.Description,
                            incident.Location,
                            incident.Status
                        );
                    }

                    AnsiConsole.Write(table);
                }
                else
                {
                    AnsiConsole.WriteLine("[bold red]No incidents found for the specified type.[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine("[bold red]Error:[/] " + ex.Message);
            }
        }

        public static void GenerateIncidentReport(ICrimeAnalysisService crimeService)
        {
            while (true)
            {
                try
                {
                    AnsiConsole.Markup("[cyan]Enter Incident ID for report generation:[/]");
                    int incidentId = int.Parse(Console.ReadLine());

                    Incident incident = crimeService.GetIncidentDetails(incidentId);

                    if (incident != null)
                    {
                        Report report = crimeService.GenerateIncidentReport(incident);

                        if (report != null)
                        {
                            AnsiConsole.Markup($"[green]Report Generated with ID:[/][yellow] {report.ReportID}[/]");
                            break;  
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[yellow]If you wish to make changes to an incident report , please choose option 3[/] ");
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"Incident with ID {incidentId} not found. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine("[bold red]Error:[/] " + ex.Message);
                }
            }
        }


        // case 
        public static void CreateCase(ICrimeAnalysisService crimeService)
        {
            AnsiConsole.Markup("[cyan]Enter Case Description:[/]");
            string description = Console.ReadLine();

            AnsiConsole.Markup("[cyan]Enter number of incidents to associate:[/]");
            int incidentCount = int.Parse(Console.ReadLine());

            List<Incident> incidents = new List<Incident>();
            for (int i = 0; i < incidentCount; i++)
            {
                AnsiConsole.Markup($"[cyan]Enter Incident {i + 1} ID:[/]");
                int incidentId = int.Parse(Console.ReadLine());
                Incident incident = crimeService.GetIncidentDetails(incidentId);  
                if (incident != null)
                {
                    incidents.Add(incident);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[blue]Incident with ID {incidentId} not found.[/]");
                }
            }

            if (incidents.Count > 0)
            {
                Case newCase = crimeService.CreateCase(description, incidents);
                AnsiConsole.MarkupLine($"[bold green]New case created with ID:[/][bold yellow] {newCase.CaseID}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]No valid incidents provided. Case not created.[/]");
            }
        }

        public static void UpdateCaseDetails(ICrimeAnalysisService crimeService)
        {
            AnsiConsole.Markup("[cyan]Enter Case ID:[/]");
            int caseId = int.Parse(Console.ReadLine());

            Case caseDetails = crimeService.GetBasicCaseDetails(caseId);

            AnsiConsole.Markup("[cyan]Enter new description:[/]");
            caseDetails.CaseDescription = Console.ReadLine();

            AnsiConsole.Markup("[cyan]Do you want to add any incidents to this case?[/] [yellow](yes/no):[/]");
            string addIncidentsResponse = Console.ReadLine().ToLower();

            if (addIncidentsResponse == "yes")
            {
                List<Incident> newIncidents = new List<Incident>();

                AnsiConsole.Markup("[cyan]How many incidents do you want to add?[/]");
                int numberOfIncidents = int.Parse(Console.ReadLine());

                for (int i = 0; i < numberOfIncidents; i++)
                {
                    AnsiConsole.MarkupLine($"[bold cyan]Enter details for Incident[/][yellow bold] {i + 1}[/]:");

                    AnsiConsole.Markup("[cyan]Enter Incident ID:[/]");
                    int incidentID = int.Parse(Console.ReadLine());

                    Incident incident = crimeService.GetIncidentDetails(incidentID);
                    if (incident != null)
                    {
                        newIncidents.Add(incident);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[bold red]Incident not found.[/]");
                    }
                }

                if (crimeService.UpdateCaseDetails(caseDetails, newIncidents))
                {
                    AnsiConsole.MarkupLine("[bold green]Case and incidents updated successfully.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red bold]Failed to update the case or incidents.[/]");
                }
            }
            else
            {
                if (crimeService.UpdateCaseDetails(caseDetails))
                {
                    AnsiConsole.MarkupLine("[green bold]Case updated successfully.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold red]Failed to update the case.[/]");
                }
            }
        }




        public static void GetAllCases(ICrimeAnalysisService crimeService)
        {
            List<DetailedCaseReport> caseReports = crimeService.GetAllCases();

            AnsiConsole.MarkupLine("[bold green]All Cases:[/]");

            var table = new Table()
              .Border(TableBorder.Rounded)
              .AddColumn(new TableColumn("[bold cyan]Case ID[/]").Centered())
              .AddColumn(new TableColumn("[bold cyan]Creation Date[/]").Centered())
              .AddColumn(new TableColumn("[bold cyan]Description[/]").Centered())
              .AddColumn(new TableColumn("[bold cyan]Incidents Count[/]").Centered());

            if (caseReports.Count > 0)
            {
                foreach (DetailedCaseReport caseReport in caseReports)
                {
                    table.AddRow(
                        caseReport.CaseInfo.CaseID.ToString(),
                        caseReport.CaseInfo.CreationDate.ToString("yyyy/MM/dd"),
                        caseReport.CaseInfo.CaseDescription,
                        caseReport.DetailedIncidents.Count.ToString()
                    );
                }

                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.MarkupLine("[bold yellow]No cases found.[/]");
            }
        }


        public static void GetCaseDetails(ICrimeAnalysisService crimeService)
    {
        AnsiConsole.Markup("Enter Case ID to view detailed case information:");
        int caseId = Convert.ToInt32(Console.ReadLine());

        DetailedCaseReport caseDetails = crimeService.GetCaseDetails(caseId);

        var caseTable = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn(new TableColumn("[bold cyan]Case ID[/]").Centered())
            .AddColumn(new TableColumn("[bold cyan]Creation Date[/]").Centered())
            .AddColumn(new TableColumn("[bold cyan]Description[/]").Centered());

        caseTable.AddRow(caseDetails.CaseInfo.CaseID.ToString(),
                         caseDetails.CaseInfo.CreationDate.ToString("yyyy/MM/dd"),
                         caseDetails.CaseInfo.CaseDescription);

        AnsiConsole.Write(caseTable);

            AnsiConsole.MarkupLine("\n[bold yellow]Incidents in this case:[/]");

        foreach (var incidentReport in caseDetails.DetailedIncidents)
        {
            var incidentTable = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("[bold cyan]Incident ID[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Type[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Date[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Location[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Status[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Report Date[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Report Status[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Description[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Officer Name[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Contact Info[/]").Centered());

                incidentTable.AddRow(
                incidentReport.Incident.IncidentID.ToString(),
                incidentReport.Incident.IncidentType,
                incidentReport.Incident.IncidentDate.ToString("yyyy/MM/dd"),
                incidentReport.Incident.Location,
                incidentReport.Incident.Status,
                incidentReport.Report.ReportDate.ToString("yyyy/MM/dd"),
                incidentReport.Report.Status,
                incidentReport.Report.ReportDetails,
                $"{incidentReport.ReportingOfficer.FirstName} {incidentReport.ReportingOfficer.LastName}",
                incidentReport.ReportingOfficer.ContactInfo
            );

            AnsiConsole.Write(incidentTable);


                var suspectTable = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn(new TableColumn("[bold cyan]Suspect ID[/]").Centered())
                    .AddColumn(new TableColumn("[bold cyan]Name[/]").Centered());
                    

                foreach (var suspect in incidentReport.Suspects)
            {
                suspectTable.AddRow(suspect.SuspectID.ToString(), $"{suspect.FirstName} {suspect.LastName}");
            }

            AnsiConsole.Write(suspectTable);

            var victimTable = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("[bold cyan]Victim ID[/]").Centered())
                .AddColumn(new TableColumn("[bold cyan]Name[/]").Centered());

            foreach (var victim in incidentReport.Victims)
            {
                victimTable.AddRow(victim.VictimID.ToString(), $"{victim.FirstName} {victim.LastName}");
            }

            AnsiConsole.Write(victimTable);
        }
    }


        public static void UpdateReportDetails(ICrimeAnalysisService crimeService)
        {
            AnsiConsole.Markup("[cyan]Enter Incident ID to update report details:[/]");
            int incidentId;

            while (!int.TryParse(Console.ReadLine(), out incidentId))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid input.[/] Please enter a valid Incident ID");
            }

            Report existingReport = crimeService.GetBasicReportDetails(incidentId);

            if (existingReport == null)
            {
                AnsiConsole.MarkupLine("[yellow]No report found for the specified Incident ID[/]");
                return;
            }

            AnsiConsole.MarkupLine("[bold yellow]Existing Report Details:[/]");
            AnsiConsole.MarkupLine($"[cyan]Report ID:[/] {existingReport.ReportID}");
            AnsiConsole.MarkupLine($"[cyan]Report Date:[/] {existingReport.ReportDate.ToString("yyyy-MM-dd")}");
            AnsiConsole.MarkupLine($"[cyan]Current Report Details:[/] {existingReport.ReportDetails}");
            AnsiConsole.MarkupLine($"[cyan]Current Status:[/] {existingReport.Status}");

            AnsiConsole.MarkupLine("[cyan]Enter new report details:[/]");
            string newReportDetails = Console.ReadLine();

            AnsiConsole.MarkupLine("[cyan]Enter new status:[/]");
            string newStatus = Console.ReadLine();

            bool isUpdated = crimeService.UpdateReportDetails(incidentId, newReportDetails, newStatus);

            if (isUpdated)
            {
                AnsiConsole.MarkupLine("[bold green]Report updated successfully.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]Failed to update the report.[/]");
            }
        }



        public static void DisplayIncidentReport(ICrimeAnalysisService crimeService)
        {
            AnsiConsole.Markup("[cyan]Enter Incident ID to view the detailed report:[/]");
            int incidentID = Convert.ToInt32(Console.ReadLine());

            DetailedIncidentReport report = crimeService.GetDetailedIncidentReport(incidentID);

            AnsiConsole.MarkupLine("[bold yellow]Incident Details:[/]");
            AnsiConsole.MarkupLine($"[cyan]Type:[/] {report.Incident.IncidentType}");
            AnsiConsole.MarkupLine($"[cyan]Date:[/] {report.Incident.IncidentDate.ToString("yyyy-MM-dd")}");
            AnsiConsole.MarkupLine($"[cyan]Location:[/] {report.Incident.Location}");
            AnsiConsole.MarkupLine($"[cyan]Description:[/] {report.Incident.Description}");

            AnsiConsole.MarkupLine("\n[bold yellow]Report Details:[/]");
            AnsiConsole.MarkupLine($"[cyan]Report Date:[/] {report.Report.ReportDate.ToString("yyyy-MM-dd")}");
            AnsiConsole.MarkupLine($"[cyan]Report Description:[/] {report.Report.ReportDetails}");
            AnsiConsole.MarkupLine($"[cyan]Report Status:[/] {report.Report.Status}");

            AnsiConsole.MarkupLine("\n[bold yellow]Reporting Officer:[/]");
            AnsiConsole.MarkupLine($"[cyan]Officer Name:[/] {report.ReportingOfficer.FirstName} {report.ReportingOfficer.LastName}");
            AnsiConsole.MarkupLine($"[cyan]Contact Info:[/] {report.ReportingOfficer.ContactInfo}");

            AnsiConsole.MarkupLine("\n[yellow]Suspects:[/]");
            foreach (var suspect in report.Suspects)
            {
                AnsiConsole.MarkupLine($"[cyan]ID:[/] {suspect.SuspectID}, [cyan]Name:[/] {suspect.FirstName} {suspect.LastName}");
            }

            AnsiConsole.MarkupLine("\n[bold yellow]Victims:[/]");
            foreach (var victim in report.Victims)
            {
                AnsiConsole.MarkupLine($"[cyan]ID:[/] {victim.VictimID},[cyan] Name:[/] {victim.FirstName} {victim.LastName}");
            }

            
        }





    }
}

