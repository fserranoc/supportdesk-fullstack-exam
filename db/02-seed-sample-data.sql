USE [SupportDesk];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Tickets', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Comments', N'U') IS NULL
BEGIN
    THROW 50001, 'Ejecute primero 01-create-schema.sql.', 1;
END;

BEGIN TRANSACTION;

DECLARE @Now DATETIMEOFFSET(7) = SYSUTCDATETIME();

DECLARE @SampleUsers TABLE
(
    Id UNIQUEIDENTIFIER NOT NULL,
    Email NVARCHAR(254) NOT NULL,
    DisplayName NVARCHAR(120) NOT NULL
);

INSERT @SampleUsers (Id, Email, DisplayName)
VALUES
    ('30000000-0000-0000-0000-000000000001', N'ana.torres@example.test', N'Ana Torres'),
    ('30000000-0000-0000-0000-000000000002', N'bruno.silva@example.test', N'Bruno Silva'),
    ('30000000-0000-0000-0000-000000000003', N'carla.munoz@example.test', N'Carla Muñoz');

INSERT dbo.Users (Id, Email, DisplayName)
SELECT source.Id, source.Email, source.DisplayName
FROM @SampleUsers AS source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Users AS existing
    WHERE existing.Id = source.Id OR existing.Email = source.Email
);

DECLARE @SampleTickets TABLE
(
    Id UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(120) NOT NULL,
    Description NVARCHAR(2000) NOT NULL,
    Priority VARCHAR(16) NOT NULL,
    Status VARCHAR(16) NOT NULL,
    CreatedAt DATETIMEOFFSET(7) NOT NULL,
    UpdatedAt DATETIMEOFFSET(7) NOT NULL,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL
);

INSERT @SampleTickets
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
VALUES
    (
        '40000000-0000-0000-0000-000000000001',
        N'Error al ingresar al portal',
        N'El usuario recibe un mensaje de acceso denegado después de iniciar sesión.',
        'High',
        'Open',
        DATEADD(DAY, -2, @Now),
        DATEADD(DAY, -2, @Now),
        '30000000-0000-0000-0000-000000000001'
    ),
    (
        '40000000-0000-0000-0000-000000000002',
        N'Impresora de bodega sin conexión',
        N'La impresora de etiquetas no responde desde dos estaciones de trabajo.',
        'Critical',
        'InProgress',
        DATEADD(DAY, -8, @Now),
        DATEADD(DAY, -7, @Now),
        '30000000-0000-0000-0000-000000000002'
    ),
    (
        '40000000-0000-0000-0000-000000000003',
        N'Solicitud de acceso a reportes',
        N'El equipo comercial necesita permisos de lectura para consultar reportes mensuales.',
        'Medium',
        'Resolved',
        DATEADD(DAY, -15, @Now),
        DATEADD(DAY, -3, @Now),
        '30000000-0000-0000-0000-000000000003'
    ),
    (
        '40000000-0000-0000-0000-000000000004',
        N'Actualización de firma corporativa',
        N'Se requiere actualizar la firma predeterminada del correo de un usuario.',
        'Low',
        'Closed',
        DATEADD(DAY, -30, @Now),
        DATEADD(DAY, -20, @Now),
        '30000000-0000-0000-0000-000000000001'
    );

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
    source.Id,
    source.Title,
    source.Description,
    source.Priority,
    source.Status,
    source.CreatedAt,
    source.UpdatedAt,
    source.CreatedByUserId
FROM @SampleTickets AS source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Tickets AS existing
    WHERE existing.Id = source.Id
);

DECLARE @SampleComments TABLE
(
    Id UNIQUEIDENTIFIER NOT NULL,
    TicketId UNIQUEIDENTIFIER NOT NULL,
    Text NVARCHAR(1000) NOT NULL,
    CreatedAt DATETIMEOFFSET(7) NOT NULL,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL
);

INSERT @SampleComments (Id, TicketId, Text, CreatedAt, CreatedByUserId)
VALUES
    (
        '50000000-0000-0000-0000-000000000001',
        '40000000-0000-0000-0000-000000000001',
        N'Se solicitó al usuario confirmar el grupo de acceso asignado.',
        DATEADD(HOUR, -36, @Now),
        '30000000-0000-0000-0000-000000000002'
    ),
    (
        '50000000-0000-0000-0000-000000000002',
        '40000000-0000-0000-0000-000000000002',
        N'Se está revisando la conectividad y la cola de impresión.',
        DATEADD(DAY, -6, @Now),
        '30000000-0000-0000-0000-000000000003'
    );

INSERT dbo.Comments (Id, TicketId, Text, CreatedAt, CreatedByUserId)
SELECT source.Id, source.TicketId, source.Text, source.CreatedAt, source.CreatedByUserId
FROM @SampleComments AS source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Comments AS existing
    WHERE existing.Id = source.Id
);

COMMIT TRANSACTION;
GO
