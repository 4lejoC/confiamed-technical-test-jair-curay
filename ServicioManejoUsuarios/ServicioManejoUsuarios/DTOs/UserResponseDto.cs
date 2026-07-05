namespace ServicioManejoUsuarios.DTOs
{
    public class UserResponseDto //DTO que sirve para devolver todos los datos de un usuario en la base de datos
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
