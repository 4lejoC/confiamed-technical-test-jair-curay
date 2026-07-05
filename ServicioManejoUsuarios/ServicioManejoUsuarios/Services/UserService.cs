using ServicioManejoUsuarios.DTOs;
using ServicioManejoUsuarios.Models;
using ServicioManejoUsuarios.Repositories;

namespace ServicioManejoUsuarios.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository; //Guarda una referencia al repositorio

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository; //Recibe el repositorio por inyección de dependencias
        }

        private static UserResponseDto MapToUserResponseDto(User user) //Transforma un User en UserResponseDto
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                IsActive = user.IsActive,
            };
        }

        public async Task<List<UserResponseDto>> GetAllAsync() //Pide todos los usuarios y los convierte a DTO
        {
            List<User> users = await _userRepository.GetAllAsync();
            return users.Select(MapToUserResponseDto).ToList();
        }

        public async Task<UserResponseDto?> GetByUsernameAsync(string username) //Valida que no venga vacío, normaliza y busca
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            string normalizedUsername = username.Trim().ToLower();
            User? user = await _userRepository.GetByUsernameAsync(normalizedUsername);

            if (user == null)
            {
                return null;
            }

            return MapToUserResponseDto(user);
        }

        public async Task<UserExistsResponseDto> ExistsAsync(string username) //Valida, normaliza y responde si existe
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return new UserExistsResponseDto
                {
                    Username = string.Empty,
                    Exists = false
                };
            }

            string normalizedUsername = username.Trim().ToLower();
            bool exist = await _userRepository.ExistsAsync(normalizedUsername);

            return new UserExistsResponseDto
            {
                Username = normalizedUsername,
                Exists = exist
            };
        }
    }
}
