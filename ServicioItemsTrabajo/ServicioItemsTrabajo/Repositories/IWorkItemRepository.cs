using ServicioItemsTrabajo.Models;

namespace ServicioItemsTrabajo.Repositories
{
    public interface IWorkItemRepository //Establece las solicitudes que se le pueden hacer a la base de datos
    {
        Task<List<WorkItem>> GetAllAsync();
        Task<WorkItem?> GetByIdAsync(int id);
        Task AddAsync(WorkItem workItem);
        Task SaveChangesAsync();
    }
}
