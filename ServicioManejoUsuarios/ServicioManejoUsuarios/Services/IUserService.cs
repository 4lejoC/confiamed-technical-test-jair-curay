using ServicioManejoUsuarios.DTOs;

namespace ServicioManejoUsuarios.Services
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllAsync(); //Devuelve una lista de usuarios ya convertidos a UserResponseDto
        Task<UserResponseDto?> GetByUsernameAsync(string username); //Devuelve un usuario ya listo para responder por API
        Task<UserExistsResponseDto> ExistsAsync(string username); //Devuelve un DTO con el resultado de existencia
    }
}
