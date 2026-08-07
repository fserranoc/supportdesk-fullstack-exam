# Decisiones backend

- Elegí .NET 6 porque fue la versión confirmada para el ejercicio, aunque en producción migraría a una LTS vigente.
- Separé Domain, Application, Infrastructure y API para mantener reglas y persistencia desacopladas.
- Utilicé DTO específicos para estabilizar el contrato HTTP y evitar overposting.
- Elegí EF Core 6 con SQL Server por su integración, parametrización y soporte relacional.
- Dejé las invariantes y transiciones en Domain; Application valida paginación, búsqueda y orden.
- Centralicé 400, 404, 409 y 500 en un middleware que responde Problem Details sin exponer internals.
- Encapsulé `X-User` detrás de `ICurrentUserService`; lo considero un stub local, no autenticación real.
- Introduje `IClock` para trabajar en UTC y mantener deterministas las pruebas.
- Mantuve los comentarios sin paginación por alcance del MVP; lo revisaría con métricas reales de volumen.
- Usé EF InMemory en pruebas HTTP para aislamiento; agregaría una suite contra SQL Server antes de producción.
