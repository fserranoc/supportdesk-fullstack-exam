# Explicación de mi solución — SupportDesk

## Alcance que implementé

Para este ejercicio construí un flujo completo de gestión de tickets: API, interfaz web, modelo SQL, pruebas y documentación. Mi objetivo no fue resolver solamente el CRUD, sino dejar separadas las responsabilidades y mostrar cómo evolucionaría la solución si pasara de examen técnico a producto.

Implementé los siete endpoints solicitados. Desde la interfaz puedo listar tickets con filtros, búsqueda y paginación; crear uno nuevo; consultar su detalle; agregar comentarios y avanzar su estado desde `Open` hasta `Closed`. La API también permite actualizar título, descripción y prioridad mediante `PUT`.

## Versión de .NET

El documento original tiene una contradicción: las instrucciones generales piden una API .NET 6, mientras el título del ejercicio backend menciona .NET 8. Para esta entrega mantuve .NET 6 porque fue la versión confirmada durante la preparación del proyecto.

Soy consciente de que .NET 6 ya no tiene soporte. Si esta solución fuera a producción, una de mis primeras tareas sería migrarla a una versión LTS vigente, ejecutar las pruebas de regresión y revisar cambios de paquetes y hosting.

## Cómo organicé el backend

Dividí el backend en cuatro proyectos:

- `SupportDesk.Domain`: contiene Ticket, Comment, User, enums e invariantes.
- `SupportDesk.Application`: contiene casos de uso, DTO, validaciones de consulta y puertos.
- `SupportDesk.Infrastructure`: implementa EF Core, SQL Server y repositorios.
- `SupportDesk.Api`: contiene controllers, middleware, configuración HTTP y composición de dependencias.

Mantuve los controllers delgados. Por ejemplo, una transición de estado no se decide en el controller: Domain calcula el único estado siguiente permitido y produce un conflicto si se intenta saltar o retroceder. Application coordina repositorios, usuario actual y reloj.

Usé request DTO específicos para no aceptar entidades completas en POST o PUT. Las longitudes, identificadores, prioridades, comentarios y transiciones se validan en servidor. Los textos se normalizan con `Trim`; no intento modificar contenido legítimo y la interfaz no renderiza HTML arbitrario.

Centralicé el manejo de excepciones en un middleware. Las validaciones responden 400, los recursos inexistentes 404, los conflictos de negocio 409 y los errores inesperados 500. En estos últimos no devuelvo detalles internos. Todas las respuestas de error incluyen un identificador de seguimiento.

Encapsulé el header `X-User` mediante `ICurrentUserService`. Lo usé porque el ejercicio permite un stub de identidad, pero no lo considero autenticación: cualquier cliente puede manipularlo. En un sistema real lo reemplazaría por OIDC/OAuth 2.0, validación JWT y autorización por rol, scope y recurso.

Documenté los siete endpoints en Swagger con summaries y códigos de respuesta. También agregué ejemplos HTTP y una colección Postman.

## Cómo organicé el frontend

Elegí React con componentes funcionales y hooks. Usé TypeScript strict, Redux Toolkit para el listado compartido y Axios como cliente HTTP.

Organicé el código en `core/api`, `features/tickets`, `components` y `app`. El cliente Axios agrega `X-User` y un `X-Correlation-ID` nuevo a cada solicitud. También transforma los errores del backend en mensajes en español y conserva el identificador de seguimiento para soporte.

Los filtros y la página quedan reflejados en la URL. Esto permite conservar el contexto al abrir un detalle y volver al listado. Apliqué un debounce de 300 ms para no disparar una solicitud por cada tecla.

En la UI consideré los estados de carga, vacío, error, éxito y reintento. Agregué un spinner accesible, prevención de doble envío, mensajes de validación y comportamiento específico para tickets cerrados. No uso `alert()` ni dejo una pantalla en blanco frente a un error.

Para responsive utilicé CSS Grid y Flexbox. En escritorio muestro una tabla; en móvil cada fila se convierte en tarjeta. También incluí labels, foco visible, un enlace para saltar al contenido y etiquetas textuales que evitan depender solamente del color.

## Base de datos

Elegí SQL Server y entregué scripts T-SQL explícitos:

- `01-create-schema.sql` crea la base `SupportDesk`, tablas, claves, checks y relaciones.
- `02-seed-sample-data.sql` agrega usuarios, cuatro tickets y dos comentarios ficticios.
- `03-required-queries.sql` resuelve las cuatro consultas del examen.
- `04-indexes.sql` agrega los índices propuestos.
- `05-seed-load-test-data.sql` genera 50 tickets ficticios para probar filtros y paginación.

Separé el seed pequeño del seed de carga para que el propósito de cada script sea claro. Ambos pueden repetirse sin duplicar sus datos.

Propuse un índice compuesto para el listado por estado, prioridad y fecha, otro para comentarios por ticket y fecha, y una restricción/índice único para email. En `performance-notes.md` expliqué cómo compararía lecturas lógicas, CPU, duración y planes antes y después. También identifiqué el uso de funciones sobre columnas filtradas y el comodín inicial de `LIKE` como posibles problemas.

No ejecuté estos scripts contra SQL Server durante la verificación final. LocalDB estaba registrado, pero Windows devolvió `Cannot create an automatic instance` al intentar iniciarlo. Por esa razón considero la revisión SQL estática, no una prueba de ejecución. Antes de entregar a un ambiente real probaría DDL, seeds, consultas y rollback sobre una instancia equivalente.

## Caso logístico

Para el escenario de incidencias propuse un monolito modular .NET con frontend React y SQL Server. El volumen de 10.000 incidencias diarias no justifica por sí solo introducir microservicios. Prefiero comenzar con límites modulares y extraer componentes solo si las métricas, la resiliencia o la organización lo exigen.

El portal Java del enunciado es ficticio y no se entregó código ni estructura. Por eso no inventé paquetes, tablas o endpoints. Dejé como primer paso un spike para identificar el contrato real y propuse un adaptador anticorrupción de solo lectura. Dependiendo de lo descubierto, consumiría una API estable o un mecanismo autorizado de réplica o batch, con timeout, circuit breaker, backoff e idempotencia.

Para seguridad propuse OIDC/OAuth 2.0, roles, scopes, TLS, cuotas y auditoría append-only. Para observabilidad consideré logs estructurados, métricas, trazas, health checks y correlation ID. También redacté dos ADR breves y un sprint de dos semanas con ocho historias, criterios de aceptación, estimaciones y riesgos.

## Verificación que ejecuté

El 5 de agosto de 2026 ejecuté las siguientes verificaciones sobre el estado final:

```text
dotnet format backend/SupportDesk.sln --verify-no-changes --no-restore  Correcto
dotnet build backend/SupportDesk.sln -c Release --no-restore          Correcto
Compilación backend                                                    0 advertencias, 0 errores
Pruebas unitarias backend                                              14/14
Pruebas de integración HTTP                                            6/6
Lint frontend                                                          Correcto
Pruebas frontend                                                       3/3
Build frontend                                                         Correcto
```

El build frontend produjo aproximadamente 298,87 kB de JavaScript y 5,27 kB de CSS antes de gzip. En el entorno de validación npm no estaba expuesto globalmente, así que ejecuté los scripts con pnpm y el runtime Node incluido. Los scripts de `package.json` son los mismos y el README incluye los comandos npm solicitados.

## Límites y próximos pasos

No presento `X-User` como seguridad real, EF InMemory como sustituto de SQL Server ni una revisión estática como ejecución SQL. Tampoco incluí migraciones EF, Docker Compose, screenshots ni pruebas E2E porque son opcionales o requieren infraestructura no disponible.

Mis siguientes pasos serían:

1. Ejecutar scripts y pruebas de integración sobre SQL Server real.
2. Migrar .NET a una LTS vigente.
3. Implementar OIDC/JWT y autorización por recurso.
4. Agregar una prueba E2E del flujo crear, comentar y cambiar estado.
5. Incorporar health checks, rate limiting y OpenTelemetry.
6. Medir las consultas con volumen representativo antes de ajustar índices o usar Full-Text Search.

Con estas decisiones busqué entregar una solución clara y comprobable, dejando explícito qué está implementado, qué fue validado y qué requiere trabajo adicional.
