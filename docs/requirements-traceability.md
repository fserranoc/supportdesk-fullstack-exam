# Trazabilidad contra el enunciado original

Usé esta matriz para comprobar que cada parte del documento original tuviera una evidencia concreta en la entrega. Distingo los extras opcionales para no presentarlos como obligatorios.

| Área | Requisito | Cómo lo cubrí | Evidencia |
|---|---|---|---|
| Entrega | Carpetas backend, frontend, db y docs | Mantuve las cuatro raíces solicitadas | `/backend`, `/frontend`, `/db`, `/docs` |
| Entrega | README con requisitos, comandos, endpoints y ejemplo | Incluí ejecución con dotnet/npm, tabla de rutas y request HTTP | `README.md` |
| Backend | Entidades y longitudes | Modelé Ticket, Comment y User con invariantes y configuración EF | `SupportDesk.Domain`, configuraciones de Infrastructure |
| Backend | Siete endpoints | Implementé listado, detalle, creación, actualización, estado y comentarios | `TicketsController.cs` |
| Backend | Filtros, búsqueda, paginación y orden | Validé parámetros en Application y compuse la consulta en Infrastructure | `TicketService.cs`, `TicketRepository.cs` |
| Backend | Arquitectura en cuatro capas | Separé API, Application, Domain e Infrastructure en proyectos | `backend/src` |
| Backend | DTO y overposting | Definí contratos específicos para cada operación | `Application/Tickets/Contracts` |
| Backend | Errores 400/404/409/500 | Centralicé Problem Details y trace ID en middleware | `ExceptionHandlingMiddleware.cs` |
| Backend | Seguridad básica y auth preparada | Validé servidor, parametrización EF y encapsulé X-User | `HeaderCurrentUserService.cs`, `ICurrentUserService` |
| Backend | DI y logging | Registré puertos/adaptadores y logs estructurados en operaciones críticas | `Program.cs`, controller y middleware |
| Backend extra | Tests unitarios | Cubrí creación, límites, comentarios, edición y transiciones | `SupportDesk.UnitTests` |
| Backend extra | Swagger documentado | Agregué summaries y códigos de respuesta en los siete endpoints | XML comments de `TicketsController.cs` |
| Backend extra | Colección Postman | Incluí variables y requests para los siete endpoints | `docs/postman` |
| Backend opcional | Migraciones EF | No las incluí; el ejercicio exige DDL explícito y documenté el trade-off | `docs/entrega-tecnica.md` |
| Frontend | Listado, filtros, búsqueda y paginación | Implementé consulta de servidor y estado Redux | `TicketListPage.tsx`, `ticketsSlice.ts` |
| Frontend | Creación y validación | Implementé formulario controlado con required, longitudes y mensajes | `CreateTicketPage.tsx` |
| Frontend | Detalle, comentarios y estado | Implementé carga conjunta y las operaciones requeridas | `TicketDetailPage.tsx` |
| Frontend | Spinner y errores 400/404/500 | Agregué indicador accesible y mensajes normalizados con trace ID | `LoadingIndicator.tsx`, `client.ts` |
| Frontend | Responsive y accesibilidad | Convertí tabla en tarjetas, incluí labels, foco y skip link | `styles.css`, `App.tsx` |
| Frontend | Organización React | Separé API, features, components y app; usé hooks y TypeScript strict | `frontend/src` |
| Frontend | Notas y tres mejoras | Expliqué ejecución, estructura, decisiones y mejoras | `docs/frontend-notes.md` |
| Frontend opcional | Capturas | No las incluí porque son opcionales y la instancia SQL local no pudo iniciarse | `docs/entrega-tecnica.md` |
| Base de datos | DDL y relaciones | Creé base, tablas, PK, FK, checks y tipos SQL Server | `db/01-create-schema.sql` |
| Base de datos | Cuatro consultas | Usé JOIN, GROUP BY y OFFSET/FETCH según el caso | `db/03-required-queries.sql` |
| Base de datos | Dos índices y validación | Propuse tres índices y expliqué planes, IO y estadísticas | `db/04-indexes.sql`, `db/performance-notes.md` |
| Base de datos | Datos de prueba adicionales | Separé un quinto script idempotente con 50 tickets | `db/05-seed-load-test-data.sql` |
| Caso | 5–10 requerimientos y mínimo 5 preguntas | Redacté 10 requerimientos, 6 supuestos y 10 preguntas | `docs/analysis/requirements-and-open-questions.md` |
| Caso | Propuesta técnica completa | Incluí diagrama, capas, persistencia, auditoría, legado y observabilidad | `docs/analysis/technical-proposal.md` |
| Caso | Interpretación del Java | Documenté que es ficticio y propuse un spike sin inventar estructura | `docs/analysis/technical-proposal.md` |
| Caso | Dos ADR de máximo 10 líneas | Escribí dos decisiones con contexto y consecuencias | `docs/analysis/adr-mini-01.md`, `adr-mini-02.md` |
| Caso | Sprint de dos semanas | Incluí 8 historias, aceptación de 2, puntos y 4 riesgos | `docs/analysis/sprint-plan.md` |

La única validación que quedó pendiente por infraestructura es ejecutar los scripts sobre SQL Server. LocalDB estaba registrado, pero Windows no pudo iniciar su instancia automática; por eso no afirmo que la prueba SQL haya pasado.
