# Caso logístico: requerimientos, supuestos y preguntas

## Requerimientos que identifiqué

1. El operador debe crear incidencias de entrega con datos mínimos validados y fecha UTC.
2. Los usuarios autorizados deben consultar, buscar, filtrar y paginar incidencias dentro de su ámbito.
3. Los supervisores deben asignar y reasignar responsables, conservando auditoría.
4. Los cambios de estado deben respetar transiciones configuradas y rechazar saltos inválidos.
5. Toda operación sensible debe generar un evento append-only con actor, fecha, cambio y correlation ID.
6. El sistema debe consultar datos del portal Java en modo de solo lectura mediante un adaptador anticorrupción.
7. El frontend y los terceros deben autenticarse con OIDC/OAuth 2.0; la API debe validar JWT, roles y scopes mínimos.
8. La solución debe absorber picos de 10.000 incidencias diarias mediante paginación, índices, una API stateless y trabajo asíncrono cuando corresponda.
9. Deben existir logs estructurados, métricas, trazas, health checks y alertas para API, base de datos e integración.
10. Deben definirse RPO, RTO, backups, restauración y degradación controlada cuando el legado no esté disponible.

## Supuestos de trabajo

- Considero que el portal Java es ficticio y que no existe código, árbol de paquetes ni contrato que pueda inspeccionar; por eso no invento su estructura.
- Supongo que la integración con el legado será estrictamente de lectura y que su mecanismo real se definirá mediante un spike técnico.
- Supongo que los 10.000 registros corresponden al pico diario total y no a solicitudes concurrentes por segundo.
- Asumo que la empresa dispone o seleccionará un proveedor corporativo compatible con OIDC/OAuth 2.0.
- Asumo que SQL Server es una tecnología aceptada y que se podrá operar con backups y alta disponibilidad.
- Para el primer sprint asumo un catálogo inicial de estados, pero no considero definitivas sus transiciones hasta validarlas con negocio.

## Preguntas abiertas

- ¿Cuál es el proveedor de identidad y qué claims o scopes entrega?
- ¿Cuáles son los roles exactos y el alcance de datos por sucursal o equipo?
- ¿Cuáles son todos los estados, transiciones y facultades para reabrir o cancelar?
- ¿Cuál es el SLA y cómo se define una incidencia atrasada por prioridad?
- ¿El portal Java expondrá API, réplica, archivos batch u otro contrato estable?
- ¿Qué frescura máxima admite la vista de datos legados?
- ¿Cuánto tiempo debe conservarse la auditoría y qué regulación aplica?
- ¿Cuántos consumidores externos existen y qué cuotas necesitan?
- ¿Se requieren notificaciones y por qué canales?
- ¿Cuáles son los objetivos concretos de RPO, RTO y disponibilidad?
