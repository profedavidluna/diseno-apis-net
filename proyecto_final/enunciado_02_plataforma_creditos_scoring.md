# Proyecto Final 2 — Plataforma de Créditos y Scoring Crediticio

## Contexto empresarial

**CreditoYa Corp.** es una fintech especializada en créditos de consumo y microcréditos para personas naturales y pequeñas empresas. Su modelo de negocio se basa en evaluar solicitudes de crédito en tiempo real usando datos propios (historial de pagos) y datos de bureaus externos (simulados). El tiempo máximo permitido entre la solicitud y la respuesta al cliente es de **3 segundos**.

La empresa está creciendo rápidamente y necesita abrir su plataforma a **aliados estratégicos** (tiendas de retail, cooperativas) que integrarán el botón de crédito en sus propios sistemas mediante APIs. Al mismo tiempo, el equipo de cumplimiento exige que ningún dato sensible del cliente (número de cédula, ingresos, score) quede expuesto en logs o respuestas innecesarias, y que todas las decisiones de crédito sean auditables.

---

## Requerimientos funcionales

1. **Solicitud de crédito**: Creación de una solicitud con datos personales, monto solicitado y plazo; respuesta síncrona con aprobación, rechazo o análisis adicional.
2. **Scoring crediticio**: Cálculo del score en base a historial interno + consulta a bureau externo (mock); el score determina la tasa de interés ofrecida.
3. **Gestión de solicitudes**: Consulta del estado de una solicitud, listado de solicitudes por cliente (paginado, filtrado por estado y fecha).
4. **Desembolso**: Marcado de una solicitud aprobada como desembolsada; generación de tabla de amortización.
5. **Alertas de mora**: Cuando un crédito activo supera su fecha de pago sin registrar abono, se publica un evento que dispara una notificación al cliente y al área de cobranza.
6. **Panel de aliados**: Endpoint exclusivo para aliados que muestra estadísticas agregadas (sin datos personales individuales) de las solicitudes originadas desde su canal.

---

## Requerimientos no funcionales y restricciones técnicas

| Área | Restricción |
|---|---|
| Autenticación clientes finales | JWT; el token debe incluir el `clientId` como claim y usarse para autorizar acceso únicamente a los propios recursos del cliente |
| Autenticación aliados | API Keys con scope `partner:read` y `partner:apply`; rotación de keys sin downtime |
| Versionado | `v1` para clientes finales y `v2` para aliados (puede haber endpoints compartidos); la versión se negocia por URL |
| Caché | El score de un cliente calculado en los últimos 10 minutos no debe recalcularse; se almacena en Redis con TTL de 10 min |
| Gateway | API Gateway (Ocelot) diferencia el tráfico de clientes del de aliados y aplica rate limiting diferenciado (aliados: 500 req/min; clientes: 100 req/min) |
| Eventos | El módulo de mora publica un evento `CreditoEnMora` consumido por el servicio de notificaciones y el servicio de cobranza (ambos simulados) |
| Documentación | Swagger con anotaciones de seguridad; los endpoints de aliados deben estar en un grupo separado (`tag`) |
| Pruebas | xUnit + Moq; pruebas unitarias del motor de scoring (al menos 6 casos: aprobado, rechazado, análisis adicional, score en frontera, bureau sin respuesta, cliente nuevo sin historial); 2 pruebas de integración end-to-end de la solicitud |
| Seguridad | Ningún log debe contener número de documento ni score en texto plano; los DTOs de respuesta al aliado deben omitir datos personales del cliente |
| Despliegue | GitHub Actions: build → test → publish → deploy a Azure (App Service o Container Apps) |

---

## Decisiones de diseño que el equipo debe tomar y documentar

> Cada decisión debe quedar registrada en el documento de arquitectura con al menos dos alternativas evaluadas y la justificación de la opción elegida.

1. **Diseño del motor de scoring**: ¿Strategy Pattern para reglas de evaluación? ¿Reglas en base de datos o en código? ¿Cómo se testea de forma aislada?
2. **Timeout y resiliencia al bureau externo**: ¿Circuit Breaker (Polly)? ¿Qué ocurre si el bureau no responde en tiempo? ¿Score por defecto o rechazo automático?
3. **Modelo de autorización granular**: ¿Claims-based? ¿Policy-based con `IAuthorizationHandler`? ¿Cómo se implementa la restricción "solo tus propios recursos"?
4. **Separación de datos sensibles**: ¿DTOs de entrada vs. entidades de dominio? ¿Enmascaramiento en el serializador o en la capa de aplicación?
5. **Diseño del evento de mora**: ¿Qué información lleva el evento? ¿Cómo garantizar que no se dupliquen notificaciones si el job se ejecuta varias veces?
6. **Estrategia de caché del score**: ¿Cache por `clientId`? ¿Cómo invalida la caché si el cliente actualiza sus datos? ¿Qué sucede ante una falla de Redis?
7. **Tabla de amortización**: ¿Se genera y se persiste, o se calcula on-demand? ¿Qué endpoint la expone?

---

## Entregables

| # | Entregable | Descripción |
|---|---|---|
| 1 | Repositorio en GitHub | Código fuente con ramas de feature y PR hacia `main` |
| 2 | Documento de arquitectura | Diagrama de componentes + ADR (Architecture Decision Records) de cada decisión |
| 3 | Swagger UI operativo | Demostración de los dos perfiles (cliente y aliado) con sus respectivos esquemas de seguridad |
| 4 | Reporte de pruebas | Cobertura reportada por Coverlet; capturas del resultado de xUnit |
| 5 | Pipeline CI/CD | Archivo YAML funcional; el pipeline debe fallar si la cobertura cae por debajo del 75 % |
| 6 | Video demo (5-10 min) | Demostración de solicitud → scoring → desembolso → alerta de mora; explicación de decisiones clave |

---

## Criterios de evaluación

| Criterio | Peso |
|---|---|
| Correctitud funcional | 25 % |
| Calidad del diseño (patrones, separación de responsabilidades) | 20 % |
| Seguridad y privacidad de datos sensibles | 15 % |
| Pruebas (variedad de casos, calidad de aserciones) | 15 % |
| Documentación (Swagger + ADR) | 10 % |
| CI/CD con gate de cobertura | 10 % |
| Presentación y argumentación | 5 % |

---

## Notas para el equipo

- El bureau externo y el servicio de cobranza deben implementarse como **interfaces mockeadas**; el equipo elige si usa un HttpClient con `MockHttpMessageHandler` o un servicio en memoria inyectado por DI.
- La tabla de amortización debe calcularse con **sistema francés** (cuota fija); pueden buscar la fórmula estándar.
- Prestar especial atención a los **edge cases** del scoring: cliente sin historial, monto solicitado mayor al máximo permitido, plazo fuera de rango.
- El pipeline CI/CD puede desplegarse a un tier gratuito de Azure siempre que el enlace sea accesible durante la presentación.
