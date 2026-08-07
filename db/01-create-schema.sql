USE [master];
GO

IF DB_ID(N'SupportDesk') IS NULL
BEGIN
    CREATE DATABASE [SupportDesk];
END;
GO

USE [SupportDesk];
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id UNIQUEIDENTIFIER NOT NULL,
        Email NVARCHAR(254) NOT NULL,
        DisplayName NVARCHAR(120) NOT NULL,
        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT CK_Users_Email_NotBlank CHECK (LEN(LTRIM(RTRIM(Email))) > 0),
        CONSTRAINT CK_Users_DisplayName_NotBlank CHECK (LEN(LTRIM(RTRIM(DisplayName))) > 0)
    );
END;

IF OBJECT_ID(N'dbo.Tickets', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tickets
    (
        Id UNIQUEIDENTIFIER NOT NULL,
        Title NVARCHAR(120) NOT NULL,
        Description NVARCHAR(2000) NOT NULL,
        Priority VARCHAR(16) NOT NULL,
        Status VARCHAR(16) NOT NULL,
        CreatedAt DATETIMEOFFSET(7) NOT NULL,
        UpdatedAt DATETIMEOFFSET(7) NOT NULL,
        CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT PK_Tickets PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Tickets_Users_CreatedByUserId FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id),
        CONSTRAINT CK_Tickets_Title_Length CHECK (LEN(LTRIM(RTRIM(Title))) BETWEEN 5 AND 120),
        CONSTRAINT CK_Tickets_Description_Length CHECK (LEN(LTRIM(RTRIM(Description))) BETWEEN 10 AND 2000),
        CONSTRAINT CK_Tickets_Priority CHECK (Priority IN ('Low', 'Medium', 'High', 'Critical')),
        CONSTRAINT CK_Tickets_Status CHECK (Status IN ('Open', 'InProgress', 'Resolved', 'Closed')),
        CONSTRAINT CK_Tickets_Dates CHECK (UpdatedAt >= CreatedAt)
    );
END;

IF OBJECT_ID(N'dbo.Comments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Comments
    (
        Id UNIQUEIDENTIFIER NOT NULL,
        TicketId UNIQUEIDENTIFIER NOT NULL,
        Text NVARCHAR(1000) NOT NULL,
        CreatedAt DATETIMEOFFSET(7) NOT NULL,
        CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT PK_Comments PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Comments_Tickets_TicketId FOREIGN KEY (TicketId) REFERENCES dbo.Tickets(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Comments_Users_CreatedByUserId FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id),
        CONSTRAINT CK_Comments_Text_Length CHECK (LEN(LTRIM(RTRIM(Text))) BETWEEN 2 AND 1000)
    );
END;

COMMIT TRANSACTION;
