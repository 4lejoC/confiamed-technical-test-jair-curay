# ServicioManejoUsuarios

Microservicio encargado de exponer usuarios disponibles para consulta desde otros servicios.

## Responsabilidad

- listar usuarios registrados
- consultar un usuario por `username`
- validar si un usuario existe

## Tecnologías

- ASP.NET Core Web API
- .NET 8
- Entity Framework Core
- SQLite
- Swagger

## Base de datos

Este servicio utiliza SQLite con una sola tabla de usuarios.

Archivo principal:

- `users.db`

## Datos semilla

Al iniciar, si la base está vacía, el servicio inserta usuarios de prueba como:

- `jcuray`
- `mlopez`
- `agarcia`
- `rperez`

## Endpoints principales

- `GET /api/users`
- `GET /api/users/{username}`
- `GET /api/users/exists/{username}`

## Cómo ejecutar

1. Abrir el proyecto en Visual Studio.
2. Ejecutar el servicio.
3. Abrir Swagger.

## Uso dentro de la solución

Este microservicio es consumido por `ServicioItemsTrabajo`, que consulta la lista de usuarios activos antes de distribuir nuevos ítems de trabajo.
