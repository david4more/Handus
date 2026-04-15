using System.Net;
using System.Net.Http.Json;
namespace Handus
{
    public class UserService
    {
        private readonly HttpClient client;

        public UserService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5275/");
        }

        public async Task<User?> GetUser(string username)
        {
            try
            {
                return await client.GetFromJsonAsync<User>($"users/byname/{username}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<User?> CreateUser(string username)
        {
            var response = await client.PostAsJsonAsync("users", new { name = username });

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<User>();
        }

        public async Task SaveUser(User user)
        {
            await client.PutAsJsonAsync($"users/{user.Id}", user);
        }
    }
}
