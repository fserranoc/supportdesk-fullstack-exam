# ADR mini 01: monolito modular

- **Contexto:** 10.000 incidencias/día, equipo y límites aún desconocidos.
- **Decisión:** monolito modular stateless, no microservicios iniciales.
- **Positivo:** menor costo operativo, transacciones simples y entrega rápida.
- **Positivo:** límites internos permiten extraer módulos medidos.
- **Negativo:** despliegue conjunto y riesgo de erosión modular.
- **Revisión:** separar solo ante escalado, resiliencia o ownership independientes demostrados.
