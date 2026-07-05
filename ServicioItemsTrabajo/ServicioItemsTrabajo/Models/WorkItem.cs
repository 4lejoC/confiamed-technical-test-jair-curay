namespace ServicioItemsTrabajo.Models
{
    //Se trabajará con una sola tabla para este microservicio; la distribución se calcula a partir de los ítems
    public class WorkItem
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

    public enum WorkItemRelevance
    {
        Low = 0,
        High = 1
    }

    public enum WorkItemStatus
    {
        Pending = 0,
        Completed = 1
    }
}
