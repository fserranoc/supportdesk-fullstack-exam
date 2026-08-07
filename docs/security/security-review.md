# Revisión de seguridad

## Alcance

Revisión estática y pruebas automatizadas del repositorio local. No incluye pentest, infraestructura cloud ni servicios externos.

| Riesgo | Severidad | Evidencia | Impacto | Recomendación | Estado |
|---|---|---|---|---|---|
| `X-User` manipulable | Alta | Identidad proviene de header | Suplantación | Reemplazar por OIDC/JWT y claims validados | Aceptado solo para examen |
| .NET 6 fuera de soporte | Alta | Target `net6.0` solicitado | Vulnerabilidades sin parches | Migrar a LTS vigente antes de producción | Aceptado por requisito |
| Sin autorización por recurso | Alta | Lectura para cualquier usuario stub | Acceso indebido | Definir roles, scopes y propiedad | Pendiente |
| Sin rate limiting | Media | No existe middleware de cuota | Abuso/agotamiento | Añadir límites por identidad/IP | Pendiente |
| Swagger solo por ambiente | Baja | Se habilita solo en Development | Descubrimiento de API | Proteger o deshabilitar en público | Mitigado |
| Búsqueda `%q%` | Baja | `LIKE` con comodín | Degradación, no inyección | Medir y evaluar Full-Text | Aceptado |
| React Router 6.30.4 | Media | `pnpm audit`: 3 advisories moderadas de redirect/SSR | Redirect/XSS si se construyen destinos no confiables o se usa SSR | No aceptar URLs externas como destino; SPA no usa SSR; actualizar cuando exista rama sin hallazgos altos | Mitigación temporal |

No se encontraron secretos reales, SQL concatenado ni renderizado de HTML arbitrario. `pnpm audit --audit-level high` finalizó correctamente con 0 altas/críticas y 3 moderadas documentadas. La aplicación no se declara “100 % segura”.
