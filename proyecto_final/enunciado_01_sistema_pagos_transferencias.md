# Proyecto Final 1 — Sistema de Pagos y Transferencias Bancarias

## Contexto empresarial

**FinPay S.A.** es un banco digital de reciente creación que necesita construir el núcleo de su plataforma de pagos y transferencias interbancarias. El banco ya cuenta con un core bancario legado que expone datos mediante una base de datos relacional, pero la nueva estrategia tecnológica exige un conjunto de APIs REST modernas que sirvan tanto a la aplicación móvil propia como a terceros autorizados (fintechs, comercios, pasarelas de pago).

La regulación local exige que toda operación financiera sea trazable, auditada y que el acceso esté fuertemente controlado. El equipo de arquitectura ha definido que el sistema debe soportar al menos **10.000 transacciones por minuto** en horas pico y que las consultas de saldo no deben golpear la base de datos transaccional en cada petición.

---

## Requerimientos funcionales

1. **Gestión de cuentas**: Consulta de saldo, historial de movimientos (paginado), datos del titular.
2. **Transferencias**: Transferencia entre cuentas del mismo banco y transferencias interbancarias (simuladas mediante un servicio externo mock).
3. **Pagos**: Pago de servicios públicos y recargas (catálogo de convenios).
4. **Notificaciones**: Cada transacción exitosa o fallida debe generar una notificación al titular (email/SMS simulado).
5. **Auditoría**: Registro inmutable de cada operación con timestamp, IP de origen, usuario y resultado.

---

## Requerimientos no funcionales y restricciones técnicas

| Área | Restricción |
|---|---|
| Autenticación de usuarios finales | JWT con refresh token; expiración de 15 minutos |
| Autenticación de terceros (fintechs) | API Keys con scopes de permisos |
| Versionado de API | Las rutas deben seguir el esquema `/api/v{n}/...`; debe existir al menos `v1` y `v2` con un endpoint deprecado gestionado correctamente |
| Caché | El saldo y el catálogo de convenios deben cachearse en Redis con TTL configurable |
| Gateway | Todas las peticiones externas deben pasar por un API Gateway (Ocelot) que centralice autenticación y rate limiting |
| Eventos | Las transferencias deben publicar eventos de dominio consumidos por el servicio de notificaciones |
| Documentación | Swagger/OpenAPI completo con ejemplos, esquemas de seguridad y descripción de errores |
| Pruebas | Cobertura mínima del 80 % en lógica de negocio (xUnit + Moq); al menos 3 pruebas de integración de los endpoints críticos |
| Despliegue | Pipeline CI/CD en Azure DevOps o GitHub Actions que compile, ejecute pruebas y publique en Azure App Service |

---

## Decisiones de diseño que el equipo debe tomar y documentar

> El equipo no recibe la arquitectura resuelta. Deben analizar el enunciado, discutir y justificar cada decisión. Se evaluará tanto la solución entregada como la calidad de la argumentación.

1. **Estrategia de versionado**: ¿URL path, query string o header? ¿Cómo se manejará la deprecación?
2. **Modelo de autenticación dual**: ¿Cómo convive JWT con API Keys en el mismo gateway? ¿Qué middleware se usa?
3. **Estructura de eventos**: ¿Se usa un bus en memoria, RabbitMQ o Azure Service Bus? ¿Qué patrón (pub/sub, event sourcing ligero)?
4. **Política de caché**: ¿Cache-aside? ¿Write-through? ¿Qué datos se invalidan y cuándo?
5. **Manejo de errores**: ¿Problem Details RFC 7807? ¿Cómo se diferencia un error de negocio de un error técnico?
6. **Separación de responsabilidades**: ¿Cuántos proyectos/microservicios? ¿Clean Architecture, Minimal APIs o controladores tradicionales?
7. **Seguridad**: ¿Cómo se protegen los endpoints de transferencia de ataques de repetición (replay attacks)?

---

## Entregables

| # | Entregable | Descripción |
|---|---|---|
| 1 | Repositorio en GitHub | Código fuente con historial de commits significativo (un commit por funcionalidad mínimo) |
| 2 | Documento de arquitectura | Diagrama C4 nivel 2 (contenedores) + justificación de decisiones |
| 3 | Colección Postman / Swagger UI | Demostración funcional de todos los endpoints |
| 4 | Reporte de pruebas | Resultado de ejecución de xUnit con cobertura |
| 5 | Pipeline CI/CD | Archivo YAML del pipeline funcional (puede apuntar a un ambiente de staging gratuito) |
| 6 | Video demo (5-10 min) | Walkthrough del sistema en funcionamiento y explicación de decisiones clave |

---

## Criterios de evaluación

| Criterio | Peso |
|---|---|
| Correctitud funcional (todos los endpoints operan según lo especificado) | 25 % |
| Calidad de diseño y aplicación de buenas prácticas | 20 % |
| Seguridad (autenticación, autorización, validaciones) | 15 % |
| Pruebas (cobertura, calidad de los casos) | 15 % |
| Documentación (Swagger + documento de arquitectura) | 10 % |
| CI/CD y despliegue | 10 % |
| Presentación y argumentación de decisiones | 5 % |

---

## Notas para el equipo

- El servicio de notificaciones y el servicio de transferencias interbancarias pueden ser **simulados** (mocks o servicios en memoria); lo importante es el diseño del contrato y la integración.
- Se permite el uso de librerías externas siempre que el equipo justifique su elección.
- El sistema debe poder ejecutarse localmente con `docker-compose up` o con instrucciones claras de configuración.
- Cada miembro del equipo debe ser capaz de explicar cualquier parte del código durante la presentación.
