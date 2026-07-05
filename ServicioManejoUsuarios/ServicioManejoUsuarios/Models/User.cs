namespace ServicioManejoUsuarios.Models
{
    //Se creará una sola tabla para este microservicio; será la de usuarios y tendrá 4 atributos. Sus especificaciones están
    //en Data/UsersDbContext
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
