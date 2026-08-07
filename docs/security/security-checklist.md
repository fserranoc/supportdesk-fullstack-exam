# Checklist de seguridad

- [x] DTO de entrada/salida; entidades no expuestas como requests.
- [x] Validación de longitud, enums, GUID, paginación y columnas de orden.
- [x] EF Core/SQL parametrizado; no hay concatenación de input en SQL.
- [x] React muestra texto codificado y no usa `dangerouslySetInnerHTML`.
- [x] Problem Details sin stack trace y con trace ID.
- [x] CORS configurable mediante allowlist.
- [x] Correlation ID aceptado/generado y devuelto.
- [x] Secretos y `.env` excluidos de Git.
- [x] Logs de creación, transición y errores sin cuerpos completos.
- [x] Dependencias fijadas mediante lockfile/versiones explícitas.
- [x] Pruebas de 400, 404 y 409.
- [ ] OIDC/JWT: pendiente; `X-User` es solo stub local.
- [ ] Rate limiting: pendiente para fase productiva.
- [ ] CSP/HSTS completos: pendientes de topología de hosting.
- [ ] SAST, auditoría de paquetes y gitleaks: configurados conceptualmente en CI; requieren ejecución remota.
- [ ] DAST no destructivo: pendiente de ambiente autorizado.
- [ ] Backups cifrados/restauración ensayada: pendiente de infraestructura real.
