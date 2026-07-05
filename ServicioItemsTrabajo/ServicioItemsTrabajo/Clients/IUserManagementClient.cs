namespace ServicioItemsTrabajo.Clients
{
    public interface IUserManagementClient
    {
        Task<List<string>> GetAvailableUsernamesAsync();
    }
}
