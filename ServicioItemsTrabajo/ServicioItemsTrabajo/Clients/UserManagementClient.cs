using System.Net.Http.Json;

namespace ServicioItemsTrabajo.Clients
{
    public class UserManagementClient : IUserManagementClient
    {
        private readonly HttpClient _httpClient;

        public UserManagementClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> GetAvailableUsernamesAsync()
        {
            List<UserManagementUserDto>? users = await _httpClient
                .GetFromJsonAsync<List<UserManagementUserDto>>("api/users");

            if (users == null)
            {
                return new List<string>();
            }

            return users
                .Where(user => user.IsActive && !string.IsNullOrWhiteSpace(user.Username))
                .Select(user => user.Username.Trim().ToLower())
                .Distinct()
                .OrderBy(username => username)
                .ToList();
        }
    }
}
