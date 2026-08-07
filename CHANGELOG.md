# Changelog

Todos los cambios relevantes de SupportDesk se documentarán en este archivo.

## [Unreleased]

### Added

- Plan inicial de arquitectura e implementación.
- Convenciones del repositorio y configuración segura de ejemplo.
- Pipeline CI inicial para backend, frontend y escaneo de secretos.
- Solución .NET 6 con separación Api, Application, Domain e Infrastructure.
- Primer flujo vertical de creación y consulta de tickets.
- Entidades y reglas iniciales, persistencia EF Core, Problem Details y correlation ID.
- Pruebas unitarias iniciales para creación y transiciones de estado.
- Backend completo con siete endpoints y pruebas HTTP aisladas.
- Frontend React responsive con Redux Toolkit, Axios, Vitest y TypeScript estricto.
- Scripts SQL Server, ADR, threat model y caso logístico documental.
- React Router fijado en 6.30.4 para evitar vulnerabilidades altas conocidas; tres riesgos moderados quedan documentados.
