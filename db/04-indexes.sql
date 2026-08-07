USE [SupportDesk];

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'UX_Users_Email')
    CREATE UNIQUE NONCLUSTERED INDEX UX_Users_Email ON dbo.Users (Email);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Tickets') AND name = N'IX_Tickets_Status_Priority_CreatedAt')
    CREATE NONCLUSTERED INDEX IX_Tickets_Status_Priority_CreatedAt
    ON dbo.Tickets (Status, Priority, CreatedAt DESC)
    INCLUDE (Id, Title, UpdatedAt, CreatedByUserId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Comments') AND name = N'IX_Comments_TicketId_CreatedAt')
    CREATE NONCLUSTERED INDEX IX_Comments_TicketId_CreatedAt
    ON dbo.Comments (TicketId, CreatedAt ASC)
    INCLUDE (Id, Text, CreatedByUserId);
