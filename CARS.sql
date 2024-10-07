Create database CARS2
use CARS2

-- Victims Table
CREATE TABLE Victims (
    VictimID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    DateOfBirth DATE,
    Gender NVARCHAR(10),
    ContactInfo NVARCHAR(100)
);

-- Suspects Table
CREATE TABLE Suspects (
    SuspectID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    DateOfBirth DATE,
    Gender NVARCHAR(10),
    ContactInfo NVARCHAR(100)
);

-- Incidents Table
CREATE TABLE Incidents (
    IncidentID INT PRIMARY KEY IDENTITY(1,1),
    IncidentType NVARCHAR(50),
    IncidentDate DATE,
    Location NVARCHAR(100),
    Description NVARCHAR(255),
    Status NVARCHAR(50)
);

-- Law Enforcement Agencies Table
CREATE TABLE LawEnforcementAgencies (
    AgencyID INT PRIMARY KEY IDENTITY(1,1),
    AgencyName NVARCHAR(100),
    Jurisdiction NVARCHAR(100),
    ContactInfo NVARCHAR(100)
);

-- Officers Table
CREATE TABLE Officers (
    OfficerID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    BadgeNumber NVARCHAR(50),
    Rank NVARCHAR(50),
    ContactInfo NVARCHAR(100),
    AgencyID INT,
    FOREIGN KEY (AgencyID) REFERENCES LawEnforcementAgencies(AgencyID)
);

-- Evidence Table
CREATE TABLE Evidence (
    EvidenceID INT PRIMARY KEY IDENTITY(1,1),
    Description NVARCHAR(255),
    LocationFound NVARCHAR(100),
    IncidentID INT,
    FOREIGN KEY (IncidentID) REFERENCES Incidents(IncidentID) ON DELETE CASCADE
);

-- Reports Table
CREATE TABLE Reports (
    ReportID INT PRIMARY KEY IDENTITY(1,1),
    IncidentID INT,
    ReportingOfficer INT,
    ReportDate DATE,
    ReportDetails NVARCHAR(255),
    Status NVARCHAR(50),
    FOREIGN KEY (IncidentID) REFERENCES Incidents(IncidentID) ON DELETE CASCADE,
    FOREIGN KEY (ReportingOfficer) REFERENCES Officers(OfficerID)
);

-- Junction Table for Incident and Victims (Many-to-Many)
CREATE TABLE IncidentVictims (
    IncidentID INT,
    VictimID INT,
    PRIMARY KEY (IncidentID, VictimID),
    FOREIGN KEY (IncidentID) REFERENCES Incidents(IncidentID) ON DELETE CASCADE,
    FOREIGN KEY (VictimID) REFERENCES Victims(VictimID) ON DELETE CASCADE
);

-- Junction Table for Incident and Suspects (Many-to-Many)
CREATE TABLE IncidentSuspects (
    IncidentID INT,
    SuspectID INT,
    PRIMARY KEY (IncidentID, SuspectID),
    FOREIGN KEY (IncidentID) REFERENCES Incidents(IncidentID) ON DELETE CASCADE,
    FOREIGN KEY (SuspectID) REFERENCES Suspects(SuspectID) ON DELETE CASCADE
);

CREATE TABLE Cases (
    CaseID INT IDENTITY(1,1) PRIMARY KEY,
    CaseDescription NVARCHAR(255),
    CreationDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE CaseIncidents (
    CaseID INT,
    IncidentID INT,
    PRIMARY KEY (CaseID, IncidentID),
    FOREIGN KEY (CaseID) REFERENCES Cases(CaseID),
    FOREIGN KEY (IncidentID) REFERENCES Incidents(IncidentID)
);


SELECT* FROM Incidents;
SELECT* FROM Cases;
SELECT* FROM CaseIncidents;
SELECT* FROM Reports;
SELECT* FROM Incidents;
SELECT * FROM Victims
SELECT * From Suspects
SELECT * FROM Officers
