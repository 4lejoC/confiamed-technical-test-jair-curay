namespace ServicioItemsTrabajo.Clients
{
    public class UserManagementUserDto //DTO interno para leer la respuesta del microservicio de usuarios
    {
        public string Username { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
