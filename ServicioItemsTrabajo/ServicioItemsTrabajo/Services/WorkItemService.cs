using Microsoft.Extensions.Options;
using ServicioItemsTrabajo.Clients;
using ServicioItemsTrabajo.DTOs;
using ServicioItemsTrabajo.Models;
using ServicioItemsTrabajo.Repositories;

namespace ServicioItemsTrabajo.Services
{
    public class WorkItemService : IWorkItemService
    {
        private readonly IUserManagementClient _userManagementClient; //Consulta los usuarios disponibles en el otro microservicio
        private readonly IWorkItemRepository _workItemRepository; //Mantiene el acceso a los ítems almacenados localmente
        private readonly DistributionOptions _distributionOptions; //Contiene las reglas de saturación para la distribución

        public WorkItemService(
            IUserManagementClient userManagementClient,
            IWorkItemRepository workItemRepository,
            IOptions<DistributionOptions> distributionOptions)
        {
            _userManagementClient = userManagementClient;
            _workItemRepository = workItemRepository;
            _distributionOptions = distributionOptions.Value;
        }

        public async Task<List<WorkItemResponseDto>> GetAllAsync() //Obtiene todos los ítems y los ordena para visualización
        {
            List<WorkItem> workItems = await _workItemRepository.GetAllAsync();

            return workItems
                .OrderBy(w => w.Status)
                .ThenBy(w => w.AssignedUsername)
                .ThenBy(w => w.PendingOrder)
                .ThenBy(w => w.DueDate)
                .Select(MapToWorkItemResponseDto)
                .ToList();
        }

        public async Task<WorkItemResponseDto?> GetByIdAsync(int id) //Busca un ítem específico por su identificador
        {
            WorkItem? workItem = await _workItemRepository.GetByIdAsync(id);

            if (workItem == null)
            {
                return null;
            }

            return MapToWorkItemResponseDto(workItem);
        }

        public async Task<List<UserWorkloadSummaryDto>> GetSummaryAsync() //Resume la carga actual por cada usuario conocido
        {
            List<WorkItem> workItems = await _workItemRepository.GetAllAsync();
            List<string> availableUsernames = await GetAvailableUsernamesAsync();
            List<string> knownUsernames = MergeUsernames(availableUsernames, workItems);
            List<UserStats> stats = BuildUserStats(workItems, knownUsernames);

            return stats
                .OrderBy(s => s.Username)
                .Select(s => new UserWorkloadSummaryDto
                {
                    Username = s.Username,
                    TotalAssignedCount = s.TotalAssignedCount,
                    PendingCount = s.PendingCount,
                    CompletedCount = s.CompletedCount,
                    HighRelevancePendingCount = s.HighRelevancePendingCount,
                    IsSaturated = s.IsSaturated
                })
                .ToList();
        }

        public async Task<List<PendingItemsByUserDto>> GetPendingByUserAsync() //Agrupa los pendientes por usuario respetando el orden actual
        {
            List<WorkItem> workItems = await _workItemRepository.GetAllAsync();
            List<string> availableUsernames = await GetAvailableUsernamesAsync();
            List<string> usernames = MergeUsernames(availableUsernames, workItems);

            return usernames
                .Select(username => new PendingItemsByUserDto
                {
                    Username = username,
                    PendingItems = workItems
                        .Where(w => w.AssignedUsername == username && w.Status == WorkItemStatus.Pending)
                        .OrderBy(w => w.PendingOrder)
                        .ThenBy(w => w.DueDate)
                        .Select(MapToWorkItemResponseDto)
                        .ToList()
                })
                .ToList();
        }

        public async Task<WorkItemResponseDto> CreateAndAssignAsync(CreateWorkItemRequestDto request) //Crea un ítem nuevo y lo distribuye según las reglas
        {
            ValidateCreateRequest(request);

            List<WorkItem> workItems = await _workItemRepository.GetAllAsync();
            List<string> availableUsernames = await GetAvailableUsernamesAsync();

            if (!availableUsernames.Any())
            {
                throw new InvalidOperationException("No hay usuarios disponibles en el microservicio de usuarios.");
            }

            DateTime dueDate = request.DueDate.ToDateTime(TimeOnly.MinValue);
            string assignedUsername = SelectAssignee(workItems, availableUsernames, request.Relevance, dueDate);

            WorkItem workItem = new WorkItem
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Relevance = request.Relevance,
                DueDate = dueDate,
                Status = WorkItemStatus.Pending,
                AssignedUsername = assignedUsername,
                CreatedAt = DateTime.UtcNow,
                AssignedAt = DateTime.UtcNow,
                PendingOrder = 0
            };

            await _workItemRepository.AddAsync(workItem);
            await _workItemRepository.SaveChangesAsync();

            List<WorkItem> updatedItems = await _workItemRepository.GetAllAsync();
            ReorderPendingItems(updatedItems, assignedUsername);
            await _workItemRepository.SaveChangesAsync();

            return MapToWorkItemResponseDto(workItem);
        }

        public async Task<WorkItemResponseDto?> CompleteAsync(int id) //Marca un ítem como completado y reordena los pendientes del usuario
        {
            WorkItem? workItem = await _workItemRepository.GetByIdAsync(id);

            if (workItem == null)
            {
                return null;
            }

            if (workItem.Status == WorkItemStatus.Completed)
            {
                return MapToWorkItemResponseDto(workItem);
            }

            workItem.Status = WorkItemStatus.Completed;
            workItem.CompletedAt = DateTime.UtcNow;
            workItem.PendingOrder = 0;

            await _workItemRepository.SaveChangesAsync();

            List<WorkItem> updatedItems = await _workItemRepository.GetAllAsync();
            ReorderPendingItems(updatedItems, workItem.AssignedUsername);
            await _workItemRepository.SaveChangesAsync();

            return MapToWorkItemResponseDto(workItem);
        }

        private static WorkItemResponseDto MapToWorkItemResponseDto(WorkItem workItem) //Transforma la entidad en un DTO de respuesta
        {
            return new WorkItemResponseDto
            {
                Id = workItem.Id,
                Title = workItem.Title,
                Description = workItem.Description,
                Relevance = workItem.Relevance,
                DueDate = workItem.DueDate,
                Status = workItem.Status,
                AssignedUsername = workItem.AssignedUsername,
                CreatedAt = workItem.CreatedAt,
                AssignedAt = workItem.AssignedAt,
                CompletedAt = workItem.CompletedAt,
                PendingOrder = workItem.PendingOrder
            };
        }

        private static void ValidateCreateRequest(CreateWorkItemRequestDto request) //Valida los datos mínimos antes de intentar la asignación
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("El título del ítem es obligatorio.");
            }
        }

        private string SelectAssignee(
            List<WorkItem> workItems,
            List<string> usernames,
            WorkItemRelevance relevance,
            DateTime dueDate) //Aplica las reglas del ejercicio para elegir al usuario asignado
        {
            List<UserStats> stats = BuildUserStats(workItems, usernames);
            List<UserStats> candidates = stats
                .Where(s => !s.IsSaturated)
                .ToList();

            if (!candidates.Any())
            {
                throw new InvalidOperationException("No hay usuarios disponibles para distribuir el ítem.");
            }

            if (IsDueSoon(dueDate))
            {
                return candidates
                    .OrderBy(s => s.CurrentAssignedCount)
                    .ThenBy(s => s.PendingCount)
                    .ThenBy(s => s.Username)
                    .First()
                    .Username;
            }

            if (relevance == WorkItemRelevance.High)
            {
                return candidates
                    .OrderBy(s => s.PendingCount)
                    .ThenBy(s => s.CurrentAssignedCount)
                    .ThenBy(s => s.Username)
                    .First()
                    .Username;
            }

            return candidates
                .OrderBy(s => s.CurrentAssignedCount)
                .ThenBy(s => s.PendingCount)
                .ThenBy(s => s.Username)
                .First()
                .Username;
        }

        private static bool IsDueSoon(DateTime dueDate) //Indica si la fecha de entrega vence dentro de menos de tres días
        {
            DateTime today = DateTime.UtcNow.Date;
            return dueDate.Date < today.AddDays(3);
        }

        private List<UserStats> BuildUserStats(List<WorkItem> workItems, List<string> usernames) //Calcula la carga operativa de cada usuario
        {
            return usernames
                .Select(username =>
                {
                    List<WorkItem> assignedItems = workItems
                        .Where(w => w.AssignedUsername == username)
                        .ToList();

                    int pendingCount = assignedItems.Count(w => w.Status == WorkItemStatus.Pending);
                    int completedCount = assignedItems.Count(w => w.Status == WorkItemStatus.Completed);
                    int highRelevancePendingCount = assignedItems.Count(w =>
                        w.Status == WorkItemStatus.Pending &&
                        w.Relevance == WorkItemRelevance.High);

                    return new UserStats
                    {
                        Username = username,
                        TotalAssignedCount = assignedItems.Count,
                        CurrentAssignedCount = pendingCount,
                        PendingCount = pendingCount,
                        CompletedCount = completedCount,
                        HighRelevancePendingCount = highRelevancePendingCount,
                        IsSaturated = highRelevancePendingCount > _distributionOptions.HighRelevanceSaturationLimit
                    };
                })
                .ToList();
        }

        private static void ReorderPendingItems(List<WorkItem> workItems, string username) //Reordena los pendientes del usuario después de cada cambio
        {
            List<WorkItem> pendingItems = workItems
                .Where(w => w.AssignedUsername == username && w.Status == WorkItemStatus.Pending)
                .OrderByDescending(w => w.Relevance)
                .ThenBy(w => w.DueDate)
                .ThenBy(w => w.CreatedAt)
                .ThenBy(w => w.Id)
                .ToList();

            for (int i = 0; i < pendingItems.Count; i++)
            {
                pendingItems[i].PendingOrder = i + 1;
            }
        }

        private async Task<List<string>> GetAvailableUsernamesAsync() //Consulta el microservicio de usuarios y devuelve usernames activos
        {
            try
            {
                return await _userManagementClient.GetAvailableUsernamesAsync();
            }
            catch (HttpRequestException)
            {
                throw new InvalidOperationException("No se pudo consultar el microservicio de usuarios.");
            }
        }

        private static List<string> MergeUsernames(List<string> availableUsernames, List<WorkItem> workItems) //Une usuarios remotos con usuarios ya referenciados en la base local
        {
            return availableUsernames
                .Concat(workItems
                    .Where(workItem => !string.IsNullOrWhiteSpace(workItem.AssignedUsername))
                    .Select(workItem => workItem.AssignedUsername.Trim().ToLower()))
                .Distinct()
                .OrderBy(username => username)
                .ToList();
        }

        private class UserStats
        {
            public string Username { get; set; } = string.Empty;
            public int TotalAssignedCount { get; set; }
            public int CurrentAssignedCount { get; set; }
            public int PendingCount { get; set; }
            public int CompletedCount { get; set; }
            public int HighRelevancePendingCount { get; set; }
            public bool IsSaturated { get; set; }
        }
    }
}
