# API Gateway con Ocelot

Este proyecto implementa un API Gateway utilizando [Ocelot](https://github.com/ThreeMammals/Ocelot) para .NET 8. Actúa como punto de entrada único hacia los microservicios de la Librería API.

## Características

### 🚦 API Gateway con Ocelot
- Enrutamiento de peticiones desde el gateway hacia `LibreriaAPI` (puerto 5000)
- Rutas disponibles bajo el prefijo `/gateway/`:
  - `/gateway/libros` → `GET`, `POST`
  - `/gateway/libros/{id}` → `GET`, `PUT`, `DELETE`
  - `/gateway/autores` → `GET`, `POST`
  - `/gateway/autores/{id}` → `GET`, `PUT`, `DELETE`
  - `/gateway/categorias` → `GET`, `POST`
  - `/gateway/categorias/{id}` → `GET`, `PUT`, `DELETE`

### 💾 Estrategias de Caché
- **Memory Cache (CacheManager)**: Caché en proceso para respuestas GET, usando `Ocelot.Cache.CacheManager` con un diccionario en memoria.
  - `GET /gateway/libros` y `GET /gateway/libros/{id}`: TTL de 30 segundos
  - `GET /gateway/autores` y `GET /gateway/autores/{id}`: TTL de 60 segundos
  - `GET /gateway/categorias` y `GET /gateway/categorias/{id}`: TTL de 120 segundos
- **Redis Distributed Cache**: Caché distribuida para escenarios multi-instancia. Se configura en `appsettings.json` con `Redis:ConnectionString`. Si Redis no está disponible, el Memory Cache sigue funcionando.

### 🚧 Rate Limiting y Control de Tráfico
- Rate limiting habilitado por ruta usando las opciones nativas de Ocelot.
- **Lectura (GET)**: máximo **30 peticiones por minuto** por cliente.
- **Escritura (POST/PUT/DELETE)**: máximo **10 peticiones por minuto** por cliente.
- Al superar el límite, se devuelve `HTTP 429 Too Many Requests` con el mensaje configurado.
- Cabecera de identificación de cliente: `X-ClientId`.
- Las cabeceras de rate limit (`X-RateLimit-*`) se incluyen en las respuestas.

## Configuración

### appsettings.json
```json
{
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "ApiGateway:"
  }
}
```

### ocelot.json
El archivo `ocelot.json` contiene todas las rutas y la configuración global del gateway.

## Ejecución

### Prerequisitos
1. Tener `LibreriaAPI` corriendo en `http://localhost:5000`
2. (Opcional) Tener Redis corriendo en `localhost:6379`

### Iniciar el gateway
```bash
cd ApiGateway
dotnet run
```

El gateway estará disponible en `http://localhost:5001`.

## Arquitectura

```
Cliente HTTP
     │
     ▼
API Gateway (puerto 5001) ←── Ocelot + CacheManager + RateLimit
     │
     ▼
LibreriaAPI (puerto 5000) ←── CQRS + MediatR + EF Core InMemory
```
