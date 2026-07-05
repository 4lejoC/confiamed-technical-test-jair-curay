using ServicioItemsTrabajo.Models;

namespace ServicioItemsTrabajo.DTOs
{
    /// <summary>
    /// DTO que sirve para registrar un ítem y disparar la asignación automática.
    /// </summary>
    public class CreateWorkItemRequestDto
    {
        /// <summary>
        /// Título corto del ítem de trabajo.
        /// </summary>
        /// <example>Revisión médica urgente</example>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Descripción opcional del trabajo a realizar.
        /// </summary>
        /// <example>Validar expediente antes del cierre</example>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Relevancia del ítem. Valores permitidos: High o Low.
        /// </summary>
        /// <example>High</example>
        public WorkItemRelevance Relevance { get; set; }

        /// <summary>
        /// Fecha de entrega del ítem en formato de solo fecha.
        /// </summary>
        /// <example>2026-07-08</example>
        public DateOnly DueDate { get; set; }
    }
}
