# SupportDesk

SupportDesk es una aplicación responsive para administrar tickets de soporte corporativo. La solución incluye una API ASP.NET Core sobre .NET 6, un cliente React con TypeScript, Redux Toolkit y Axios, persistencia SQL Server, scripts T-SQL, pruebas y documentación técnica en español.

> El enunciado menciona .NET 6 en las instrucciones generales y .NET 8 en el título del backend. Conservé .NET 6 porque fue la versión confirmada para esta entrega. Para un sistema productivo migraría a una versión LTS con soporte vigente.

## Funcionalidad

- Listado paginado con búsqueda, filtros y orden de servidor.
- Creación, detalle y actualización de tickets.
- Flujo `Open → InProgress → Resolved → Closed`, sin saltos ni retrocesos.
- Comentarios cronológicos y tickets cerrados en modo de solo lectura.
- Problem Details para 400, 404, 409 y 500, con correlation ID.
- UI responsive con carga, vacío, error, éxito, reintento y navegación accesible.

## Estructura

```text
backend/   Solución .NET, capas y pruebas
frontend/  Aplicación React, estado, API, componentes y pruebas
db/        DDL, datos ficticios, consultas, índices y notas de rendimiento
docs/      Decisiones, arquitectura, operación y caso de análisis
```

## Requisitos

- .NET SDK 6.0.x.
- Node.js 20.19 o superior.
- npm incluido con Node.js.
- SQL Server 2019 o superior, LocalDB o una instancia compatible para la persistencia real.

## Base de datos

Ejecutar los scripts sobre la misma instancia configurada en la API:

1. `db/01-create-schema.sql`: crea y selecciona la base `SupportDesk` y sus tablas.
2. `db/02-seed-sample-data.sql`: agrega un conjunto pequeño de datos ficticios.
3. `db/04-indexes.sql`: crea los índices propuestos.
4. `db/05-seed-load-test-data.sql`: agrega 50 tickets ficticios para filtros y paginación.

`db/03-required-queries.sql` contiene las cuatro consultas solicitadas y se ejecuta de forma independiente para revisar sus resultados.

## Ejecutar el backend

La configuración predeterminada usa LocalDB. Para otra instancia, definir `ConnectionStrings__SupportDesk` como variable de entorno sin versionar credenciales.

```powershell
dotnet restore .\backend\SupportDesk.sln
dotnet build .\backend\SupportDesk.sln --configuration Release --no-restore
dotnet run --project .\backend\src\SupportDesk.Api
```

La API queda en `http://localhost:5080` y Swagger en `http://localhost:5080/swagger` durante Development.

## Ejecutar el frontend

```powershell
Set-Location frontend
Copy-Item .env.example .env.local
npm install
npm run dev
```

La aplicación queda en `http://localhost:5173`. El repositorio conserva además `pnpm-lock.yaml`; si se utiliza pnpm, los equivalentes son `pnpm install --frozen-lockfile` y `pnpm dev`.

## Endpoints

| Método | Ruta | Propósito |
|---|---|---|
| GET | `/api/tickets` | Listar, buscar, filtrar, ordenar y paginar |
| GET | `/api/tickets/{id}` | Obtener el detalle |
| POST | `/api/tickets` | Crear un ticket |
| PUT | `/api/tickets/{id}` | Actualizar campos principales |
| PATCH | `/api/tickets/{id}/status` | Avanzar el estado |
| POST | `/api/tickets/{id}/comments` | Agregar un comentario |
| GET | `/api/tickets/{id}/comments` | Listar comentarios |

Ejemplo:

```http
POST http://localhost:5080/api/tickets
Content-Type: application/json
X-User: user@example.test

{
  "title": "Error al acceder al portal",
  "description": "El usuario recibe un error al iniciar sesión en el portal corporativo.",
  "priority": "High"
}
```

Hay más ejemplos en `docs/api-examples.md` y una colección importable en `docs/postman/SupportDesk.postman_collection.json`.

## Calidad

```powershell
dotnet test .\backend\SupportDesk.sln --configuration Release
dotnet format .\backend\SupportDesk.sln --verify-no-changes

npm run lint --prefix frontend
npm test --prefix frontend
npm run build --prefix frontend
```

Las pruebas de integración usan EF Core InMemory para aislar los endpoints. Los scripts SQL se mantienen como la definición explícita exigida por la sección de base de datos; antes de producción agregaría pruebas de integración contra SQL Server real.

## Seguridad y limitaciones

- Los request DTO evitan overposting y las reglas se validan en servidor.
- EF Core parametriza consultas y React no renderiza HTML arbitrario.
- `X-User` es exclusivamente un stub manipulable del examen; no reemplaza autenticación.
- CORS está limitado al origen local configurado y los errores 500 ocultan detalles internos.
- No se incluyen secretos, datos personales reales ni credenciales válidas.
- No incluí migraciones EF porque el examen exige DDL SQL explícito; en un producto versionaría ambos mediante una estrategia única.
- Las capturas de UI son opcionales y no forman parte de esta entrega.

La explicación completa, escrita desde la perspectiva del candidato, está en `docs/entrega-tecnica.md`. La comparación requisito por requisito está en `docs/requirements-traceability.md`.
