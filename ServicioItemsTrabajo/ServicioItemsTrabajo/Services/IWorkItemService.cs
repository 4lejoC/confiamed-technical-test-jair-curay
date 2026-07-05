using ServicioItemsTrabajo.DTOs;

namespace ServicioItemsTrabajo.Services
{
    public interface IWorkItemService
    {
        Task<List<WorkItemResponseDto>> GetAllAsync();
        Task<WorkItemResponseDto?> GetByIdAsync(int id);
        Task<List<UserWorkloadSummaryDto>> GetSummaryAsync();
        Task<List<PendingItemsByUserDto>> GetPendingByUserAsync();
        Task<WorkItemResponseDto> CreateAndAssignAsync(CreateWorkItemRequestDto request);
        Task<WorkItemResponseDto?> CompleteAsync(int id);
    }
}
