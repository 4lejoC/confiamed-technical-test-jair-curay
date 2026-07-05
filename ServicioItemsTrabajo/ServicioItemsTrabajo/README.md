# ServicioItemsTrabajo

Microservicio encargado de registrar, distribuir y completar ítems de trabajo.

## Responsabilidad

- crear ítems de trabajo
- asignarlos automáticamente según reglas del negocio
- consultar carga por usuario
- consultar pendientes por usuario
- marcar ítems como completados

## Tecnologías

- ASP.NET Core Web API
- .NET 8
- Entity Framework Core
- SQLite
- Swagger

## Base de datos

Este servicio utiliza SQLite con una sola tabla de ítems de trabajo.

Archivo principal:

- `workitems.db`

## Integración con el microservicio de usuarios

Este servicio consulta por HTTP al microservicio `ServicioManejoUsuarios` para obtener la lista de usuarios activos.

Configuración relevante en `appsettings.json`:

- `UserManagementService:BaseUrl`

Esta URL debe apuntar al `localhost` y al puerto real con el que esté corriendo `ServicioManejoUsuarios`.

La referencia correcta se puede tomar desde la URL que abre Swagger al ejecutar ese microservicio.

## Reglas de distribución implementadas

- si la fecha de entrega vence en menos de 3 días, se asigna al usuario con menor carga actual
- si el ítem es de relevancia alta y no vence pronto, se asigna al usuario con menor cantidad de pendientes
- si un usuario tiene más de 3 ítems pendientes de relevancia alta, se considera saturado
- tras asignar o completar un ítem, se reordena la lista de pendientes del usuario

## Endpoints principales

- `GET /api/work-items`
- `GET /api/work-items/{id}`
- `GET /api/work-items/summary`
- `GET /api/work-items/pending-by-user`
- `POST /api/work-items`
- `PATCH /api/work-items/{id}/complete`

## Cómo ejecutar

1. Ejecutar primero `ServicioManejoUsuarios`.
2. Ejecutar después `ServicioItemsTrabajo`.
3. Abrir Swagger para probar los endpoints.

## Dirección local del servicio

En el archivo `ServicioItemsTrabajo.http`, la variable `ServicioItemsTrabajo_HostAddress` debe usar el mismo `localhost` y el mismo puerto que muestra Swagger cuando el servicio está corriendo.

Ejemplo:

```txt
@ServicioItemsTrabajo_HostAddress = http://localhost:5062
```

Si Swagger abre con otro puerto, ese valor debe actualizarse para que las pruebas locales funcionen correctamente.

## Datos semilla

El servicio inserta datos de prueba al iniciar si la base está vacía, con el fin de mostrar:

- pendientes
- completados
- orden de trabajo
- resumen por usuario

## Ejemplo de creación de ítem

```json
{
  "title": "Revisión médica urgente",
  "description": "Validar expediente antes del cierre",
  "relevance": "High",
  "dueDate": "2026-07-08"
}
```
