using Microsoft.AspNetCore.Mvc;
using ServicioManejoUsuarios.DTOs;
using ServicioManejoUsuarios.Services;

namespace ServicioManejoUsuarios.Controllers
{
    /// <summary>
    /// Expone operaciones de consulta para los usuarios disponibles en el sistema.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Obtiene la lista completa de usuarios registrados.
        /// </summary>
        /// <returns>Una lista de usuarios con su información básica.</returns>
        [HttpGet]
        public async Task<ActionResult<List<UserResponseDto>>> GetAll() //Devuelve todos los usuarios
        {
            List<UserResponseDto> users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Busca un usuario por su nombre de usuario.
        /// </summary>
        /// <param name="username">Nombre de usuario que se desea consultar.</param>
        /// <returns>El usuario encontrado o un 404 si no existe.</returns>
        [HttpGet("{username}")]
        public async Task<ActionResult<UserResponseDto>> GetByUsername(string username) //Devuelve un usuario por username o 404
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("El nombre de usuario no puede estar vacío.");
            }

            UserResponseDto? user = await _userService.GetByUsernameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Verifica si un nombre de usuario existe en la base de datos.
        /// </summary>
        /// <param name="username">Nombre de usuario que se desea validar.</param>
        /// <returns>Un indicador booleano que informa si el usuario existe.</returns>
        [HttpGet("exists/{username}")]
        public async Task<ActionResult<UserExistsResponseDto>> Exists(string username) //Devuelve un objeto con Username y Exists
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("El nombre de usuario no puede estar vacío.");
            }

            UserExistsResponseDto user = await _userService.ExistsAsync(username);
            return Ok(user);
        }
    }
}
