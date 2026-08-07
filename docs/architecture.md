# Arquitectura de SupportDesk

## Contexto

```mermaid
flowchart LR
  U["Usuario interno"] -->|"Navegador"| W["SupportDesk React"]
  W -->|"REST JSON + X-User"| A["API ASP.NET Core"]
  A -->|"EF Core"| D[("SQL Server")]
```

## Contenedores y capas

```mermaid
flowchart TB
  R["React / Redux / Axios"] --> C["Controllers + middleware"]
  C --> AP["Application: casos de uso, DTO y puertos"]
  AP --> DM["Domain: entidades e invariantes"]
  I["Infrastructure: EF Core y repositorios"] --> AP
  I --> DM
  I --> DB[("SQL Server")]
```

Domain no referencia frameworks ni capas exteriores. Api compone dependencias; Infrastructure implementa los puertos definidos en Application.

## Modelo de datos

```mermaid
erDiagram
  USERS ||--o{ TICKETS : crea
  USERS ||--o{ COMMENTS : escribe
  TICKETS ||--o{ COMMENTS : contiene
  USERS { uuid Id PK string Email UK string DisplayName }
  TICKETS { uuid Id PK string Title string Description string Priority string Status datetime CreatedAt datetime UpdatedAt uuid CreatedByUserId FK }
  COMMENTS { uuid Id PK uuid TicketId FK string Text datetime CreatedAt uuid CreatedByUserId FK }
```

## Flujo de creación

```mermaid
sequenceDiagram
  participant UI as React
  participant API as TicketsController
  participant APP as TicketService
  participant DB as SQL Server
  UI->>API: POST /api/tickets + X-User
  API->>APP: CreateTicketRequest
  APP->>APP: resolver usuario, validar y crear Domain
  APP->>DB: INSERT usuario/ticket transaccional
  DB-->>APP: confirmado
  APP-->>API: TicketDetailResponse
  API-->>UI: 201 + Location
```

## Flujo de estado

El controller entrega el estado solicitado a Application. Domain calcula el único siguiente estado permitido; un salto produce `BusinessConflictException`, traducido a 409. Una transición válida actualiza `UpdatedAt` UTC, persiste y se registra con ticket/estado sin body sensible.

## Operación

La API acepta/genera `X-Correlation-ID` y lo devuelve. En producción se propone consolidar logs/métricas/trazas, usar health checks de DB, alertar por latencia/errores y proteger Swagger. Backups, restauración y rollback deben ensayarse según el hosting real.
