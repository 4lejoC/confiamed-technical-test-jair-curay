using Microsoft.EntityFrameworkCore;
using ServicioManejoUsuarios.Data;
using ServicioManejoUsuarios.Models;

namespace ServicioManejoUsuarios.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UsersDbContext _context; //Mantiene conexión con la base de datos

        public UserRepository(UsersDbContext context) //Da un contexto de las tablas de la base de datos
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync() //Pide todos los usuarios de la tabla Users en la base de datos
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username) //Busca un usuario por su nombre en la base de datos
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsAsync(string username) //Busca si existe un usuario con un nombre específico y devuelve true o false
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username);
        }
    }
}
