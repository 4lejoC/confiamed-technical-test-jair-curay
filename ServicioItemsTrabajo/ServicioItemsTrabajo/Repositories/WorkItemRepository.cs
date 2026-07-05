using Microsoft.EntityFrameworkCore;
using ServicioItemsTrabajo.Data;
using ServicioItemsTrabajo.Models;

namespace ServicioItemsTrabajo.Repositories
{
    public class WorkItemRepository : IWorkItemRepository
    {
        private readonly WorkItemsDbContext _context; //Mantiene conexión con la base de datos

        public WorkItemRepository(WorkItemsDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkItem>> GetAllAsync() //Trae todos los ítems de trabajo
        {
            return await _context.WorkItems.ToListAsync();
        }

        public async Task<WorkItem?> GetByIdAsync(int id) //Busca un ítem por su identificador
        {
            return await _context.WorkItems.FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task AddAsync(WorkItem workItem) //Agrega un ítem nuevo a la tabla
        {
            await _context.WorkItems.AddAsync(workItem);
        }

        public async Task SaveChangesAsync() //Guarda todos los cambios pendientes del contexto
        {
            await _context.SaveChangesAsync();
        }
    }
}
