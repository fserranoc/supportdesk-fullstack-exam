/* 1. Listado paginado con filtros y total de comentarios. */
DECLARE @Status VARCHAR(16) = NULL;
DECLARE @Priority VARCHAR(16) = NULL;
DECLARE @Page INT = 1;
DECLARE @PageSize INT = 20;

SELECT
    t.Id,
    t.Title,
    t.Priority,
    t.Status,
    t.CreatedAt,
    t.UpdatedAt,
    u.DisplayName AS CreatedByDisplayName,
    COUNT(c.Id) AS CommentCount
FROM dbo.Tickets AS t
INNER JOIN dbo.Users AS u ON u.Id = t.CreatedByUserId
LEFT JOIN dbo.Comments AS c ON c.TicketId = t.Id
WHERE (@Status IS NULL OR t.Status = @Status)
  AND (@Priority IS NULL OR t.Priority = @Priority)
GROUP BY t.Id, t.Title, t.Priority, t.Status, t.CreatedAt, t.UpdatedAt, u.DisplayName
ORDER BY t.CreatedAt DESC, t.Id ASC
OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

/* 2. Cinco usuarios con más tickets creados durante el último mes calendario móvil. */
DECLARE @Now DATETIMEOFFSET(7) = SYSDATETIMEOFFSET();
SELECT TOP (5)
    u.Id,
    u.Email,
    u.DisplayName,
    COUNT_BIG(t.Id) AS TicketCount
FROM dbo.Users AS u
INNER JOIN dbo.Tickets AS t ON t.CreatedByUserId = u.Id
WHERE t.CreatedAt >= DATEADD(MONTH, -1, @Now)
  AND t.CreatedAt < @Now
GROUP BY u.Id, u.Email, u.DisplayName
ORDER BY TicketCount DESC, u.Id ASC;

/* 3. Búsqueda case-insensitive según la collation de la base. */
DECLARE @Q NVARCHAR(200) = N'portal';
SELECT t.Id, t.Title, t.Description, t.Priority, t.Status, t.CreatedAt
FROM dbo.Tickets AS t
WHERE t.Title LIKE N'%' + @Q + N'%'
   OR t.Description LIKE N'%' + @Q + N'%'
ORDER BY t.CreatedAt DESC, t.Id ASC;

/* 4. Tickets atrasados: más antiguos que X días y aún no cerrados. */
DECLARE @DaysOld INT = 7;
SELECT t.Id, t.Title, t.Priority, t.Status, t.CreatedAt, u.DisplayName AS CreatedByDisplayName
FROM dbo.Tickets AS t
INNER JOIN dbo.Users AS u ON u.Id = t.CreatedByUserId
WHERE t.Status <> 'Closed'
  AND t.CreatedAt < DATEADD(DAY, -@DaysOld, SYSDATETIMEOFFSET())
ORDER BY t.CreatedAt ASC, t.Id ASC;
