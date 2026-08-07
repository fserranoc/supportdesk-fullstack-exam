USE [SupportDesk];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL OR OBJECT_ID(N'dbo.Tickets', N'U') IS NULL
BEGIN
    THROW 50001, 'Ejecute primero 01-create-schema.sql.', 1;
END;

BEGIN TRANSACTION;

DECLARE @TestUsers TABLE
(
    Id UNIQUEIDENTIFIER NOT NULL,
    Email NVARCHAR(254) NOT NULL,
    DisplayName NVARCHAR(120) NOT NULL
);

INSERT @TestUsers (Id, Email, DisplayName)
VALUES
    ('30000000-0000-0000-0000-000000000001', N'ana.torres@example.test', N'Ana Torres'),
    ('30000000-0000-0000-0000-000000000002', N'bruno.silva@example.test', N'Bruno Silva'),
    ('30000000-0000-0000-0000-000000000003', N'carla.munoz@example.test', N'Carla Muñoz'),
    ('30000000-0000-0000-0000-000000000004', N'diego.rojas@example.test', N'Diego Rojas'),
    ('30000000-0000-0000-0000-000000000005', N'elena.soto@example.test', N'Elena Soto');

INSERT dbo.Users (Id, Email, DisplayName)
SELECT source.Id, source.Email, source.DisplayName
FROM @TestUsers AS source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Users AS existing
    WHERE existing.Id = source.Id OR existing.Email = source.Email
);

DECLARE @Now DATETIMEOFFSET(7) = SYSUTCDATETIME();

;WITH Numbers AS
(
    SELECT 1 AS Number
    UNION ALL
    SELECT Number + 1
    FROM Numbers
    WHERE Number < 50
),
TestTickets AS
(
    SELECT
        Number,
        CONVERT(NVARCHAR(120), N'Ticket de prueba ' + RIGHT('000' + CONVERT(VARCHAR(3), Number), 3)) AS Title,
        CONVERT(NVARCHAR(2000), N'Incidente ficticio generado para validar filtros, paginación, estados y prioridades. Caso número ' + CONVERT(VARCHAR(3), Number) + N'.') AS Description,
        CASE Number % 4
            WHEN 0 THEN 'Critical'
            WHEN 1 THEN 'Low'
            WHEN 2 THEN 'Medium'
            ELSE 'High'
        END AS Priority,
        CASE Number % 4
            WHEN 0 THEN 'Closed'
            WHEN 1 THEN 'Open'
            WHEN 2 THEN 'InProgress'
            ELSE 'Resolved'
        END AS Status,
        DATEADD(HOUR, -Number * 12, @Now) AS CreatedAt,
        DATEADD(HOUR, -Number * 12 + (Number % 10), @Now) AS UpdatedAt,
        CASE Number % 5
            WHEN 0 THEN N'elena.soto@example.test'
            WHEN 1 THEN N'ana.torres@example.test'
            WHEN 2 THEN N'bruno.silva@example.test'
            WHEN 3 THEN N'carla.munoz@example.test'
            ELSE N'diego.rojas@example.test'
        END AS CreatedByEmail
    FROM Numbers
)
INSERT dbo.Tickets
(
    Id,
    Title,
    Description,
    Priority,
    Status,
    CreatedAt,
    UpdatedAt,
    CreatedByUserId
)
SELECT
    NEWID(),
    source.Title,
    source.Description,
    source.Priority,
    source.Status,
    source.CreatedAt,
    source.UpdatedAt,
    creator.Id
FROM TestTickets AS source
INNER JOIN dbo.Users AS creator ON creator.Email = source.CreatedByEmail
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Tickets AS existing
    WHERE existing.Title = source.Title
);

DECLARE @InsertedTickets INT = @@ROWCOUNT;

COMMIT TRANSACTION;

SELECT
    @InsertedTickets AS InsertedTickets,
    COUNT_BIG(t.Id) AS TotalLoadTestTickets
FROM dbo.Tickets AS t
WHERE t.Title LIKE N'Ticket de prueba %';
GO
