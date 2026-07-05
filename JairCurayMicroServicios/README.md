# JairCurayMicroServicios

Backend desarrollado como solución de prueba técnica con arquitectura de microservicios en C# y .NET 8.

## Estructura de la solución

La solución contiene dos microservicios:

- `ServicioManejoUsuarios`
- `ServicioItemsTrabajo`

Cada microservicio tiene una arquitectura simple por capas:

- `Controllers`
- `Services`
- `Repositories`
- `Data`
- `Models`
- `DTOs`

## Objetivo funcional

El sistema permite gestionar ítems de trabajo y su distribución entre usuarios.

Se implementaron dos responsabilidades principales:

- un microservicio para consultar usuarios disponibles
- un microservicio para registrar, distribuir y completar ítems de trabajo

## Tecnologías utilizadas

- C#
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger / OpenAPI

## Microservicios

### 1. ServicioManejoUsuarios

Responsabilidad:

- exponer usuarios disponibles para consulta
- validar si un usuario existe

Este servicio representa la parte del sistema donde existen los usuarios.

### 2. ServicioItemsTrabajo

Responsabilidad:

- registrar ítems de trabajo
- asignarlos automáticamente según reglas de negocio
- marcar ítems como completados
- mostrar carga de trabajo y pendientes por usuario

Este servicio consulta al microservicio de usuarios para obtener la lista de usuarios activos.

## Reglas de asignación implementadas

- si la fecha de entrega vence en menos de 3 días, el ítem se asigna al usuario con menor carga actual
- si el ítem es de relevancia alta y no vence pronto, se asigna al usuario con menor cantidad de pendientes
- si un usuario tiene más de 3 ítems pendientes de relevancia alta, se considera saturado y no participa en la distribución
- después de cada asignación o completado, se reordena la lista de pendientes del usuario

## Persistencia

Cada microservicio usa su propia base SQLite:

- `users.db`
- `workitems.db`

También pueden aparecer archivos auxiliares como:

- `*.db-wal`
- `*.db-shm`

Son archivos normales de SQLite cuando trabaja en modo WAL.

## Cómo ejecutar la solución

1. Abrir la solución `JairCurayMicroServicios.slnx`.
2. Restaurar paquetes NuGet si Visual Studio lo solicita.
3. Configurar la solución con `Multiple startup projects`.
4. Marcar `ServicioManejoUsuarios` como `Start`.
5. Marcar `ServicioItemsTrabajo` como `Start`.
6. Ejecutar la solución.

También se pueden ejecutar por separado, pero el orden importa porque `ServicioItemsTrabajo` consulta al microservicio de usuarios por HTTP.

## Configuración de puertos

Antes de probar la integración, se debe verificar que `ServicioItemsTrabajo` apunte al `localhost` y puerto reales con los que está corriendo `ServicioManejoUsuarios`.

La configuración relevante está en:

- `ServicioItemsTrabajo/appsettings.json`

Propiedad:

- `UserManagementService:BaseUrl`

El valor correcto se puede tomar desde la URL que abre Swagger al ejecutar `ServicioManejoUsuarios`.

## Swagger

Cada microservicio expone Swagger para pruebas manuales.

### ServicioManejoUsuarios

Endpoints principales:

- `GET /api/users`
- `GET /api/users/{username}`
- `GET /api/users/exists/{username}`

### ServicioItemsTrabajo

Endpoints principales:

- `GET /api/work-items`
- `GET /api/work-items/{id}`
- `GET /api/work-items/summary`
- `GET /api/work-items/pending-by-user`
- `POST /api/work-items`
- `PATCH /api/work-items/{id}/complete`

## Pruebas manuales realizadas

Se validó manualmente:

- consulta de usuarios
- consulta de existencia de usuarios
- creación de ítems
- asignación automática
- resumen por usuario
- consulta de pendientes por usuario
- marcado de ítems como completados

## Notas de diseño

- Se utilizó una arquitectura simple por capas para mantener claridad y rapidez de implementación.
- El microservicio de ítems consume al microservicio de usuarios para respetar el enfoque de microservicios del enunciado.
- Se añadieron datos semilla para facilitar la demostración funcional desde el arranque.
