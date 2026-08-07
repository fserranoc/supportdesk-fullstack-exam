# Notas de rendimiento SQL Server

## Índices seleccionados

- `IX_Tickets_Status_Priority_CreatedAt` respalda los filtros y el orden principal del listado. Incluye las columnas de proyección más frecuentes para reducir lookups. Aumenta el costo de insert y de cambios de estado/prioridad, aceptable para un sistema interno con más lecturas que escrituras.
- `IX_Comments_TicketId_CreatedAt` cubre el listado cronológico y ayuda al conteo por ticket. La clave comienza por la FK, evitando escaneos completos.
- `UX_Users_Email` garantiza la regla de unicidad y acelera la resolución del usuario enviado por `X-User`.

## Cómo medir

1. Poblar un volumen representativo y actualizar estadísticas.
2. Activar `SET STATISTICS IO ON; SET STATISTICS TIME ON;`.
3. Capturar el plan de ejecución real de cada consulta antes de crear los índices.
4. Ejecutar con parámetros selectivos y no selectivos; registrar lecturas lógicas, CPU, duración, scans/seeks y lookups.
5. Crear un índice por vez, repetir con caché caliente y fría cuando sea relevante, y comparar.
6. Revisar periódicamente uso, fragmentación y costo de escritura; eliminar índices sin evidencia de beneficio.

## Antipatrón y corrección

El antipatrón `SELECT * FROM Tickets WHERE CAST(CreatedAt AS DATE) = @Date` devuelve columnas innecesarias y hace no sargable el filtro. Debe proyectar solo las columnas requeridas y usar un rango: `CreatedAt >= @StartUtc AND CreatedAt < @EndUtc`.

La búsqueda `LIKE '%q%'` tiene comodín inicial; un B-tree tradicional no resuelve eficientemente ese patrón. Si las mediciones lo justifican, la evolución recomendada es SQL Server Full-Text Search, manteniendo paginación y límites de longitud.
