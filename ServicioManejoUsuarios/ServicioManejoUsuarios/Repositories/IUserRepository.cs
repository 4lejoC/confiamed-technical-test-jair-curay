using ServicioManejoUsuarios.Models;

namespace ServicioManejoUsuarios.Repositories
{
    public interface IUserRepository //Establece las solicitudes que se le pueden hacer a la base de datos
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> ExistsAsync(string username);
    }
}
