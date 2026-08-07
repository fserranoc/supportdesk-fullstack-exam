# Ejemplos de API

Base local sugerida: `http://localhost:5080/api`. Todos los datos son ficticios.

```http
GET /api/tickets?status=Open&priority=High&q=portal&page=1&pageSize=20&sortBy=createdAt&sortDirection=desc
X-User: user@example.test
```

```http
POST /api/tickets
X-User: user@example.test
Content-Type: application/json

{"title":"Error al acceder al portal","description":"El usuario recibe un error al iniciar sesión en el portal corporativo.","priority":"High"}
```

```http
PUT /api/tickets/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
X-User: user@example.test
Content-Type: application/json

{"title":"Error recurrente al acceder al portal","description":"El error continúa después de limpiar la caché del navegador.","priority":"Critical"}
```

```http
PATCH /api/tickets/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/status
X-User: agent@example.test
Content-Type: application/json

{"status":"InProgress"}
```

```http
POST /api/tickets/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/comments
X-User: agent@example.test
Content-Type: application/json

{"text":"Se solicitó información adicional para reproducir el problema."}
```

También existen `GET /api/tickets/{id}` y `GET /api/tickets/{id}/comments`. Respuestas inválidas siguen Problem Details con 400, 404, 409 o 500 y `traceId`.
