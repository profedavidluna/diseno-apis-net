# Proyecto Final 3 — Sistema de Gestión de Inversiones y Portafolios

## Contexto empresarial

**InverFlex S.A.** es una casa de valores que quiere digitalizar completamente la gestión de portafolios de inversión para sus clientes (personas naturales e institucionales). Actualmente, la operación se gestiona manualmente por asesores financieros; el nuevo sistema debe exponer APIs que permitan a los clientes consultar su portafolio, ejecutar órdenes de compra/venta de instrumentos financieros (acciones, ETFs, bonos — todos simulados) y recibir alertas de mercado en tiempo real.

Adicionalmente, **asesores financieros** internos necesitan un perfil con permisos elevados para ver portafolios de sus clientes asignados, aprobar órdenes de alto valor y generar reportes. La regulación exige que toda orden de inversión quede registrada con trazabilidad completa y que los datos de rendimiento del portafolio no se recalculen en cada consulta (alta frecuencia de acceso).

El sistema debe estar diseñado para escalar horizontalmente: múltiples instancias del API detrás del gateway deben funcionar correctamente sin estados en memoria local.

---

## Requerimientos funcionales

1. **Gestión de portafolios**: Consulta del portafolio del cliente (posiciones, valor actual, rendimiento acumulado); el valor actual se obtiene de un servicio de precios de mercado (mock).
2. **Órdenes de inversión**: Creación, consulta y cancelación de órdenes de compra/venta; las órdenes de alto valor (> USD 50.000) requieren aprobación del asesor antes de ejecutarse.
3. **Catálogo de instrumentos**: Listado de instrumentos disponibles (acciones, ETFs, bonos) con precio actual, sector y datos básicos; paginado y filtrable.
4. **Alertas de precio**: El cliente configura alertas (ej. "notificarme si el precio de X sube 5 %"); cuando se dispara la condición, se publica un evento y se notifica al cliente.
5. **Reportes del asesor**: El asesor puede descargar un reporte (JSON o CSV) de rendimiento de todos sus clientes asignados, filtrado por período.
6. **Historial de órdenes**: Listado de órdenes ejecutadas y canceladas, paginado y con filtros por instrumento, estado y rango de fechas.

---

## Requerimientos no funcionales y restricciones técnicas

| Área | Restricción |
|---|---|
| Autenticación | JWT con claims `role` (`cliente`, `asesor`, `admin`) y `userId`; el token no debe contener información sensible de saldo |
| Autorización | Un cliente solo puede ver y operar su propio portafolio; un asesor puede ver los portafolios de sus clientes asignados (relación persistida en BD); admin accede a todo |
| Versionado | `/api/v1/...` para la versión inicial; se debe preparar la infraestructura para `v2` (aunque no tenga cambios aún); los headers de respuesta deben incluir `Deprecated: true` en endpoints marcados para deprecación futura |
| Caché | El valor del portafolio y los precios del catálogo de instrumentos deben cachearse en Redis; TTL de 60 segundos para precios y 5 minutos para el portafolio calculado |
| Gateway | Ocelot gestiona las rutas hacia los servicios internos; aplica autenticación centralizada y un límite de 200 req/min por usuario |
| Eventos | La evaluación de alertas de precio se desencadena mediante un evento periódico; las notificaciones se procesan de forma asíncrona |
| Sin estado local | No se permite usar `IMemoryCache` en el API; todo caché debe ir a Redis para garantizar coherencia entre instancias |
| Documentación | Swagger/OpenAPI con tres grupos de endpoints según rol (cliente, asesor, admin); descripción completa de códigos de error y esquemas |
| Pruebas | xUnit + Moq; pruebas unitarias de la lógica de aprobación de órdenes (al menos 5 casos) y de la evaluación de alertas (al menos 4 casos); pruebas de integración para autenticación y autorización por rol |
| Despliegue | GitHub Actions: compilación → pruebas → análisis de cobertura → despliegue a Azure Container Apps o App Service con slot de staging |

---

## Decisiones de diseño que el equipo debe tomar y documentar

> Se evaluará la profundidad del análisis, no solo la implementación. Cada decisión debe tener un ADR (Architecture Decision Record) de máximo una página.

1. **Modelo de roles y permisos**: ¿Role-based (RBAC) puro o attribute-based (ABAC)? ¿Cómo implementar la restricción "asesor solo ve sus clientes asignados" sin duplicar lógica en cada controlador?
2. **Invalidación de caché distribuido**: ¿Cómo se invalida el portafolio en Redis cuando se ejecuta una orden? ¿Cache Tag Helper, patrón de invalidación por evento o TTL corto y aceptar eventual consistency?
3. **Diseño de la lógica de aprobación**: ¿State Machine? ¿Strategy Pattern? ¿Cómo se asegura que una orden no se apruebe dos veces ante condiciones de carrera?
4. **Evaluación de alertas de precio**: ¿Job en background (`IHostedService`)? ¿Evento disparado por el servicio de precios? ¿Cómo se evita la re-notificación si la condición persiste?
5. **Escalabilidad sin estado**: ¿Qué consideraciones adicionales se necesitan al usar JWT + Redis en múltiples instancias? ¿Cómo se maneja la revocación de tokens?
6. **Diseño del reporte del asesor**: ¿Generación sincrónica o asíncrona (job + polling)? ¿Qué formato es más adecuado para datos financieros?
7. **Resiliencia al servicio de precios de mercado**: ¿Qué ocurre si el mock de precios no responde? ¿Se muestra el último precio conocido (de caché) o se retorna error?

---

## Entregables

| # | Entregable | Descripción |
|---|---|---|
| 1 | Repositorio en GitHub | Código fuente con al menos una rama por funcionalidad principal; PRs hacia `main` |
| 2 | Documento de arquitectura | Diagrama de contexto C4 + ADR por cada decisión de diseño |
| 3 | Swagger UI operativo | Los tres grupos de endpoints (cliente / asesor / admin) deben ser demostrables con usuarios de prueba reales |
| 4 | Reporte de pruebas | Cobertura generada con Coverlet; mínimo 75 % en la capa de aplicación |
| 5 | Pipeline CI/CD | YAML funcional que incluya gate de calidad (cobertura y análisis estático básico) |
| 6 | Video demo (5-10 min) | Flujo completo: autenticación → consulta portafolio → orden de alto valor → aprobación asesor → alerta de precio; argumentación de al menos 3 decisiones de diseño |

---

## Criterios de evaluación

| Criterio | Peso |
|---|---|
| Correctitud funcional | 25 % |
| Diseño de la autorización y seguridad por rol | 20 % |
| Calidad del diseño (patrones, ausencia de estado local, caché distribuido) | 15 % |
| Pruebas (casos borde, calidad de aserciones) | 15 % |
| Documentación (Swagger agrupado + ADR) | 10 % |
| CI/CD con gate de calidad | 10 % |
| Presentación y argumentación de decisiones | 5 % |

---

## Notas para el equipo

- El servicio de precios de mercado debe implementarse como una **interfaz con una implementación mock** que genere precios aleatorios dentro de un rango razonable; esto permite testear la lógica de alertas de forma determinista usando Moq.
- La relación asesor-cliente puede gestionarse con una tabla simple en la base de datos; no es necesario un sistema de roles externo (como Azure AD).
- El catálogo de instrumentos puede ser estático (seed de base de datos) con al menos 20 instrumentos de tres categorías distintas.
- Los reportes del asesor no necesitan ser archivos descargables reales; puede retornarse un JSON estructurado con los datos del reporte.
- Prestar atención a las **condiciones de carrera** en la aprobación de órdenes: ¿qué pasa si dos asesores aprueban la misma orden simultáneamente?
- El pipeline puede desplegarse a un tier gratuito de Azure siempre que el enlace sea accesible durante la presentación.
