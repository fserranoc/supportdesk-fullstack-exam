# ADR-001: arquitectura en capas

- **Estado:** aceptada.
- **Contexto:** el examen requiere separación clara sin microservicios.
- **Decisión:** Api → Application; Infrastructure implementa puertos; Domain no depende de frameworks.
- **Consecuencias positivas:** reglas testeables, persistencia reemplazable y controllers delgados.
- **Trade-offs:** más proyectos y mapeos que una API CRUD directa.
