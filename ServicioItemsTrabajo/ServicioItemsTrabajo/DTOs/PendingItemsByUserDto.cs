namespace ServicioItemsTrabajo.DTOs
{
    public class PendingItemsByUserDto //DTO que agrupa los pendientes ordenados por usuario
    {
        public string Username { get; set; } = string.Empty;
        public List<WorkItemResponseDto> PendingItems { get; set; } = new();
    }
}
