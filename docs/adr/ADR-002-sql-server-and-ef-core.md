# ADR-002: SQL Server y EF Core 6

- **Estado:** aceptada.
- **Contexto:** modelo relacional y requisito explícito de T-SQL.
- **Decisión:** SQL Server 2019+ y EF Core 6 con configuraciones por entidad.
- **Consecuencias positivas:** consistencia, FK, consultas parametrizadas y productividad.
- **Trade-offs:** pruebas deben respetar diferencias del proveedor; consultas críticas requieren medición.
