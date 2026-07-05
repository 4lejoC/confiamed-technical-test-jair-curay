using Microsoft.AspNetCore.Mvc;
using ServicioItemsTrabajo.DTOs;
using ServicioItemsTrabajo.Services;

namespace ServicioItemsTrabajo.Controllers
{
    /// <summary>
    /// Expone operaciones para consultar, asignar y completar ítems de trabajo.
    /// </summary>
    [ApiController]
    [Route("api/work-items")]
    public class WorkItemsController : ControllerBase
    {
        private readonly IWorkItemService _workItemService;

        public WorkItemsController(IWorkItemService workItemService)
        {
            _workItemService = workItemService;
        }

        /// <summary>
        /// Obtiene la lista completa de ítems de trabajo.
        /// </summary>
        /// <returns>Una lista con todos los ítems registrados y su estado actual.</returns>
        [HttpGet]
        public async Task<ActionResult<List<WorkItemResponseDto>>> GetAll() //Devuelve todos los ítems
        {
            List<WorkItemResponseDto> workItems = await _workItemService.GetAllAsync();
            return Ok(workItems);
        }

        /// <summary>
        /// Busca un ítem de trabajo por su identificador.
        /// </summary>
        /// <param name="id">Identificador único del ítem.</param>
        /// <returns>El ítem encontrado o un 404 si no existe.</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<WorkItemResponseDto>> GetById(int id) //Devuelve un ítem por id o 404
        {
            WorkItemResponseDto? workItem = await _workItemService.GetByIdAsync(id);

            if (workItem == null)
            {
                return NotFound();
            }

            return Ok(workItem);
        }

        /// <summary>
        /// Obtiene el resumen de carga de trabajo por usuario.
        /// </summary>
        /// <returns>La cantidad de asignados, pendientes, completados y saturación por usuario.</returns>
        [HttpGet("summary")]
        public async Task<ActionResult<List<UserWorkloadSummaryDto>>> GetSummary() //Devuelve el resumen de carga por usuario
        {
            List<UserWorkloadSummaryDto> summary = await _workItemService.GetSummaryAsync();
            return Ok(summary);
        }

        /// <summary>
        /// Obtiene los ítems pendientes agrupados y ordenados por usuario.
        /// </summary>
        /// <returns>Una lista de usuarios con sus pendientes ordenados.</returns>
        [HttpGet("pending-by-user")]
        public async Task<ActionResult<List<PendingItemsByUserDto>>> GetPendingByUser() //Devuelve los pendientes agrupados por usuario
        {
            List<PendingItemsByUserDto> pendingItems = await _workItemService.GetPendingByUserAsync();
            return Ok(pendingItems);
        }

        /// <summary>
        /// Registra un nuevo ítem y lo asigna automáticamente según las reglas de distribución.
        /// </summary>
        /// <param name="request">Datos mínimos requeridos para crear el ítem.</param>
        /// <returns>El ítem creado con el usuario asignado.</returns>
        [HttpPost]
        public async Task<ActionResult<WorkItemResponseDto>> CreateAndAssign(CreateWorkItemRequestDto request) //Registra un ítem y lo asigna según reglas
        {
            try
            {
                WorkItemResponseDto workItem = await _workItemService.CreateAndAssignAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = workItem.Id }, workItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Marca un ítem de trabajo como completado.
        /// </summary>
        /// <param name="id">Identificador del ítem que se desea completar.</param>
        /// <returns>El ítem actualizado con estado completado.</returns>
        [HttpPatch("{id:int}/complete")]
        public async Task<ActionResult<WorkItemResponseDto>> Complete(int id) //Marca un ítem como completado
        {
            WorkItemResponseDto? workItem = await _workItemService.CompleteAsync(id);

            if (workItem == null)
            {
                return NotFound();
            }

            return Ok(workItem);
        }
    }
}
