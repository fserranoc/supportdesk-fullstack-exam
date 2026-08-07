# Caso logístico: sprint de dos semanas

## Objetivo

Entregar un flujo vertical seguro y observable que permita crear y consultar incidencias, con identidad corporativa simulada por contrato, persistencia y base de auditoría.

## Historias

| ID | Historia | Estimación |
|---|---|---:|
| HU-01 | Como operador, quiero crear una incidencia para registrar una entrega fallida. | 5 |
| HU-02 | Como operador, quiero consultar incidencias autorizadas para conocer su avance. | 5 |
| HU-03 | Como supervisor, quiero filtrar por estado y prioridad para priorizar trabajo. | 3 |
| HU-04 | Como supervisor, quiero asignar responsable para establecer ownership. | 5 |
| HU-05 | Como supervisor, quiero cambiar estado con reglas para mantener un flujo válido. | 5 |
| HU-06 | Como auditor, quiero ver el historial inmutable para reconstruir decisiones. | 5 |
| HU-07 | Como equipo de operación, quiero health checks y logs correlacionados para diagnosticar fallos. | 3 |
| HU-08 | Como arquitecto, quiero un spike del contrato Java para elegir la integración autorizada. | 3 |

## Criterios de aceptación

### HU-01

- Dado un operador autenticado, cuando envía datos válidos, se crea la incidencia y se devuelve 201.
- Estado, actor y fechas se asignan en servidor.
- Datos inválidos retornan 400 por campo y no persisten cambios.
- La creación genera un evento de auditoría en la misma transacción.

### HU-05

- Solo un supervisor autorizado puede ejecutar la operación.
- Una transición permitida actualiza estado, actor, UTC y auditoría.
- Saltos o retrocesos inválidos retornan 409 sin cambios parciales.

## Dependencias

Proveedor OIDC/sandbox, catálogo de estados, acceso técnico al legado y ambiente SQL Server. HU-08 precede cualquier integración real.

## Riesgos

- Contrato legado inexistente o inestable: spike temprano y adaptador anticorrupción.
- Roles/transiciones sin definición: sesión con negocio y tablas de decisión.
- Alcance superior a capacidad: preservar flujo vertical y mover extras al backlog.
- Ambiente OIDC tardío: validador simulado detrás de la misma interfaz, sin presentarlo como seguridad real.

## Definición de terminado

Código revisado, compilación/lint correctos, pruebas críticas verdes, validación y autorización aplicadas, auditoría presente, OpenAPI/documentación actualizada, sin secretos y desplegable en ambiente de desarrollo.
