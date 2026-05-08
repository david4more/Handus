using System.Net;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
namespace Handus
{
    public class UserService
    {
        private readonly HttpClient client;
        private ClientWebSocket? ws;
        private string? token;

        public UserService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5275/");
        }

        public async Task<bool> Login(string username)
        {
            var response = await client.PostAsync($"login?username={username}", null);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            token = data?["token"];

            await ConnectWebSocket();
            return true;
        }
        private async Task ConnectWebSocket()
        {
            ws = new ClientWebSocket();
            Uri uri = new Uri($"ws://localhost:5275/ws?token={token}");
            await ws.ConnectAsync(uri, CancellationToken.None);
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

        public async Task<User?> CreateUser(string username, string email, string password)
        {
            var response = await client.PostAsJsonAsync($"users", new { name = username, email = email, password = password });

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<User>();
        }

        public async Task SendPosition(float x, float y)
        {
            if (ws?.State == WebSocketState.Open)
            {
                var msg = JsonSerializer.Serialize(new { x, y });
                var bytes = Encoding.UTF8.GetBytes(msg);
                await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
