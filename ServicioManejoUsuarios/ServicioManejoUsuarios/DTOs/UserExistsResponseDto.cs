namespace ServicioManejoUsuarios.DTOs
{
    public class UserExistsResponseDto //DTO que sirve para saber si un usuario existe en la base de datos
    {
        public string Username { get; set; } = string.Empty;
        public bool Exists { get; set; }
    }
}
