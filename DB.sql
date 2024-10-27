
CREATE SCHEMA schoolMoney;

-- Users table
CREATE TABLE schoolMoney.[Users] (
    UserId NVARCHAR(50) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- Classes table
CREATE TABLE schoolMoney.[Classes] (
    ClassId NVARCHAR(50) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    SchoolName NVARCHAR(255) NOT NULL,
    TreasurerId NVARCHAR(50) NOT NULL,
    FOREIGN KEY (TreasurerId) REFERENCES schoolMoney.[Users](UserId)
);

-- Children table
CREATE TABLE schoolMoney.[Children] (
    ChildId NVARCHAR(50) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    ParentId NVARCHAR(50) NOT NULL,
    ClassId NVARCHAR(50) NOT NULL,
    FOREIGN KEY (ParentId) REFERENCES schoolMoney.[Users](UserId),
    FOREIGN KEY (ClassId) REFERENCES schoolMoney.[Classes](ClassId)
);

-- Fundraisers table
CREATE TABLE schoolMoney.[Fundraisers] (
    FundraiserId NVARCHAR(50) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    GoalAmount DECIMAL(18, 2) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    ClassId NVARCHAR(50) NOT NULL,
    FOREIGN KEY (ClassId) REFERENCES schoolMoney.[Classes](ClassId)
);

-- Transactions table
CREATE TABLE schoolMoney.[Transactions] (
    TransactionId NVARCHAR(50) PRIMARY KEY,
    FundraiserId NVARCHAR(50) NOT NULL,
    UserId NVARCHAR(50) NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    Date DATETIME NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(50) NOT NULL,
    VirtualAccountNumber NVARCHAR(20) NOT NULL,
    FOREIGN KEY (FundraiserId) REFERENCES schoolMoney.[Fundraisers](FundraiserId),
    FOREIGN KEY (UserId) REFERENCES schoolMoney.[Users](UserId)
);

DROP TABLE schoolMoney.[Transactions];
DROP TABLE schoolMoney.[Fundraisers];
DROP TABLE schoolMoney.[Children];
DROP TABLE schoolMoney.[Classes];
DROP TABLE schoolMoney.[Users];
