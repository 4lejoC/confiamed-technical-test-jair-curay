using ServicioItemsTrabajo.Models;

namespace ServicioItemsTrabajo.DTOs
{
    public class WorkItemResponseDto //DTO que sirve para devolver un ítem con su información de asignación
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public WorkItemRelevance Relevance { get; set; }
        public DateTime DueDate { get; set; }
        public WorkItemStatus Status { get; set; }
        public string AssignedUsername { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int PendingOrder { get; set; }
    }
}
