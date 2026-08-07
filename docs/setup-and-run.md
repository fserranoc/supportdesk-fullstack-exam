# Instalación, ejecución y operación local

## Requisitos

- .NET SDK 6.0.x.
- Node.js 20.19 o superior con npm.
- SQL Server 2019 o superior, o LocalDB.
- pnpm 11.9 es una alternativa si se desea usar el lockfile incluido.

## Base de datos

Ejecutar sobre la misma instancia configurada en la API:

1. `db/01-create-schema.sql`.
2. `db/02-seed-sample-data.sql`.
3. `db/04-indexes.sql`.
4. Opcionalmente, `db/05-seed-load-test-data.sql` para agregar 50 tickets ficticios.

`db/03-required-queries.sql` contiene consultas demostrativas y no modifica datos. Los scripts de seed son idempotentes. Antes de un ambiente compartido usaría migraciones versionadas, backup y aprobación para cualquier cambio destructivo.

## Backend

```powershell
dotnet restore .\backend\SupportDesk.sln
dotnet build .\backend\SupportDesk.sln --configuration Release --no-restore
dotnet test .\backend\SupportDesk.sln --configuration Release --no-build
dotnet run --project .\backend\src\SupportDesk.Api
```

La API escucha en `http://localhost:5080`; Swagger está disponible en `/swagger` durante Development. La configuración predeterminada usa LocalDB. Para otra instancia, definir `ConnectionStrings__SupportDesk` como variable de entorno sin guardar secretos en el repositorio.

## Frontend

```powershell
Set-Location frontend
Copy-Item .env.example .env.local
npm install
npm run dev
npm run lint
npm test
npm run build
```

El frontend escucha en `http://localhost:5173`. `VITE_API_BASE_URL` apunta a la API y `VITE_CURRENT_USER` configura únicamente el stub local. Como alternativa reproducible se puede ejecutar `pnpm install --frozen-lockfile` y los mismos scripts con pnpm.

## Troubleshooting y runbook

- **API no inicia:** validar SDK, cadena de conexión y que el esquema se haya creado.
- **Frontend no conecta:** revisar `frontend/.env.local`, API activa, puerto 5080, CORS y consola de red.
- **SQL Server no disponible:** comprobar servicio, instancia, credenciales locales y conectividad; no reintentar indefinidamente.
- **Puerto ocupado:** cambiar launch settings, `frontend/.env.local` y CORS de forma consistente.
- **Error funcional:** usar el `X-Correlation-ID` de la respuesta para buscar el evento en logs.
- **Migración pendiente:** detener el despliegue, respaldar y aplicar la versión aprobada; no modificar una migración compartida.
- **Rollback:** desplegar el artefacto anterior compatible y usar un script compensatorio probado si cambió el modelo.
- **Restauración:** restaurar primero en una instancia aislada, validar integridad y después coordinar el cambio.

## Alcance local

No incluí Docker Compose porque no es obligatorio y no pude validarlo en el entorno disponible. Antes de producción ejecutaría los scripts y una suite de integración contra una instancia SQL Server equivalente al ambiente objetivo.
