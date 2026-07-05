namespace ServicioItemsTrabajo.DTOs
{
    public class UserWorkloadSummaryDto //DTO que resume la carga de trabajo actual por usuario
    {
        public string Username { get; set; } = string.Empty;
        public int TotalAssignedCount { get; set; }
        public int PendingCount { get; set; }
        public int CompletedCount { get; set; }
        public int HighRelevancePendingCount { get; set; }
        public bool IsSaturated { get; set; }
    }
}
