# ADR mini 02: EF Core frente a Dapper

- **Contexto:** CRUD transaccional con consultas relacionales y auditoría.
- **Decisión:** EF Core como acceso principal; Dapper solo para consultas medidas.
- **Positivo:** productividad, tracking controlado, migraciones y parametrización.
- **Negativo:** abstracción y riesgo de consultas subóptimas.
- **Mitigación:** proyecciones, `AsNoTracking`, revisión de SQL/planes y benchmarks.
