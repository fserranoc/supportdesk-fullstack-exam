# ADR-004: identidad simulada con X-User

- **Estado:** aceptada exclusivamente para entorno local de examen.
- **Contexto:** no existe proveedor de identidad ni credenciales.
- **Decisión:** leer `X-User` mediante `ICurrentUserService`, con fallback ficticio de desarrollo.
- **Consecuencias positivas:** casos de uso independientes de HTTP y migración sencilla a claims.
- **Consecuencias negativas:** el header es manipulable y no proporciona autenticación ni autorización reales.
