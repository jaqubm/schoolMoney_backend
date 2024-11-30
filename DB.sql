CREATE DATABASE [schoolMoney-DB];

USE [schoolMoney-DB];

CREATE SCHEMA schoolMoney;

-- Accounts table
CREATE TABLE schoolMoney.[Account] (
    AccountNumber NVARCHAR(12) PRIMARY KEY,
    Balance DECIMAL(18, 2) NOT NULL DEFAULT 0.0
);

-- Users table
CREATE TABLE schoolMoney.[User] (
    UserId NVARCHAR(50) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL,
    Surname NVARCHAR(100) NOT NULL,
    PasswordHash VARBINARY(MAX) NOT NULL,
    PasswordSalt VARBINARY(MAX) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    AccountNumber NVARCHAR(12),  -- FK to Accounts
    FOREIGN KEY (AccountNumber) REFERENCES schoolMoney.[Account](AccountNumber)
);

-- Classes table
CREATE TABLE schoolMoney.[Class] (
    ClassId NVARCHAR(50) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    SchoolName NVARCHAR(255) NOT NULL,
    TreasurerId NVARCHAR(50) NOT NULL,  -- FK to User
    FOREIGN KEY (TreasurerId) REFERENCES schoolMoney.[User](UserId)
);

-- Children table
CREATE TABLE schoolMoney.[Child] (
    ChildId NVARCHAR(50) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    ParentId NVARCHAR(50) NOT NULL,
    ClassId NVARCHAR(50),
    FOREIGN KEY (ParentId) REFERENCES schoolMoney.[User](UserId),
    FOREIGN KEY (ClassId) REFERENCES schoolMoney.[Class](ClassId)
);

-- Fundraisers table
CREATE TABLE schoolMoney.[Fundraise] (
    FundraiseId NVARCHAR(50) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    ImageIndex INT,
    GoalAmount DECIMAL(18, 2) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    ClassId NVARCHAR(50) NOT NULL,  -- FK to Class
    AccountNumber NVARCHAR(12) NOT NULL,  -- FK to Accounts
    FOREIGN KEY (ClassId) REFERENCES schoolMoney.[Class](ClassId),
    FOREIGN KEY (AccountNumber) REFERENCES schoolMoney.[Account](AccountNumber)
);

-- Transactions table
CREATE TABLE schoolMoney.[Transaction] (
    TransactionId NVARCHAR(50) PRIMARY KEY,
    Amount DECIMAL(18, 2) NOT NULL,
    Date DATETIME NOT NULL DEFAULT GETDATE(),
    Type NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    SourceAccountNumber NVARCHAR(12) NOT NULL,  -- FK to Accounts
    DestinationAccountNumber NVARCHAR(12) NOT NULL,  -- FK to Accounts
    FOREIGN KEY (SourceAccountNumber) REFERENCES schoolMoney.[Account](AccountNumber),
    FOREIGN KEY (DestinationAccountNumber) REFERENCES schoolMoney.[Account](AccountNumber)
);


DROP TABLE schoolMoney.[Transaction];
DROP TABLE schoolMoney.[Fundraise];
DROP TABLE schoolMoney.[Child];
DROP TABLE schoolMoney.[Class];
DROP TABLE schoolMoney.[User];
DROP TABLE schoolMoney.[Account];
