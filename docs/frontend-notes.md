# Notas frontend

## Cómo lo ejecuto

Desde `frontend` copio `.env.example` como `.env.local`, ejecuto `npm install` y luego `npm run dev`. Para verificar la entrega utilizo `npm run lint`, `npm test` y `npm run build`. También dejé un lockfile de pnpm como alternativa reproducible en el entorno en que desarrollé.

## Cómo organicé el código

- `core/api`: configuré Axios, headers de identidad/trazabilidad y normalización de errores.
- `features/tickets`: concentré tipos, llamadas HTTP, slice Redux, páginas y pruebas del dominio visual.
- `components`: dejé el spinner, los mensajes de estado y los badges reutilizables.
- `app`: configuré el store y los hooks tipados de Redux.

Usé Redux Toolkit para el listado porque sus filtros, carga y error son estado compartido. Mantuve los formularios y el detalle en estado local para no globalizar información efímera. Reflejé filtros y página en la URL y apliqué un debounce de 300 ms a la búsqueda.

## Errores, accesibilidad y responsive

Transformé errores de red y Problem Details en mensajes en español para 400, 404, 409 y 500. Muestro el correlation ID cuando existe, permito reintentar cargas fallidas y evito dobles envíos. El spinner tiene un estado accesible independiente de su animación.

La interfaz usa labels, foco visible, enlace para saltar al contenido y texto además de color. En escritorio presento una tabla; desde 720 px la convierto en tarjetas y mantengo funcionamiento desde 360 px.

## Tres mejoras futuras

1. Evaluaría RTK Query o TanStack Query si las mediciones justifican cache de servidor más avanzado.
2. Incorporaría actualizaciones optimistas para comentarios y transiciones, con rollback explícito.
3. Agregaría internacionalización y pruebas automáticas E2E, de accesibilidad y de distintos viewports.
