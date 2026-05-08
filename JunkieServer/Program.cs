using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseWebSockets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// json files
string usersFile = "users.json";
string logFile = "log.json";

// users/tokens data
List<User> users = new();
Dictionary<string, User?> tokens = new();

// load data
if (File.Exists(usersFile))
    users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(usersFile)) ?? new();

// save data
void SaveUsers() =>
    File.WriteAllText(usersFile, JsonSerializer.Serialize(users));

bool IsAuthorized(string token)
{
    return token != null && tokens.ContainsKey(token);
}

// logs
void Log(string action, string info)
{
    var entry = new LogEntry
    {
        Action = action,
        Info = info,
        Time = DateTime.Now
    };

    List<LogEntry> logs = new();

    if (File.Exists(logFile))
        logs = JsonSerializer.Deserialize<List<LogEntry>>(File.ReadAllText(logFile)) ?? new();

    logs.Add(entry);
    File.WriteAllText(logFile, JsonSerializer.Serialize(logs));
}

var messageQueue = Channel.CreateUnbounded<UserAction>();

// worker
_ = Task.Run(async () =>
{
    await foreach (var action in messageQueue.Reader.ReadAllAsync())
    {
        Log(action.Type, action.Description);

        if (action.Type == "UPDATE_POSITION")
        {
            var user = users.FirstOrDefault(u => u.Id == action.UserId);
            if (user != null)
            {
                user.PositionX = action.X;
                user.PositionY = action.Y;
                SaveUsers();
            }
        }

        Console.WriteLine($"Worker: Опрацьовано дію {action.Type} для користувача {action.UserId}");
    }
});

// methods for login and register
app.MapGet("/users/byname/{username}", (string username) =>
{
    var user = users.FirstOrDefault(u => u.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
    Log("USER_CHECK", $"Checking existence of {username}");
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("/users", (User user) =>
{
    user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
    users.Add(user);
    SaveUsers();

    Log("CREATE USER", $"User {user.Name} created");

    return Results.Ok(user);
});

app.MapPost("/login", (string username) =>
{
    var user = users.FirstOrDefault(u => u.Name == username);
    if (user == null)
        return Results.Unauthorized();

    string token = Guid.NewGuid().ToString();
    tokens[token] = user;

    Log("LOGIN", $"User {user?.Name} logged in");

    return Results.Ok(new { token = token });
});

// methods for authorized users
app.MapGet("/users", (string token) =>
{
    if (!IsAuthorized(token))
    {
        return Results.Unauthorized();
    }
    Log("GET USERS", "All users requested");
    return Results.Ok(users);
});

app.MapPut("/users/update", async (User updatedUser, string token) =>
{
    if (!IsAuthorized(token)) return Results.Unauthorized();

    var user = tokens[token];
    if (user == null) return Results.NotFound();

    await messageQueue.Writer.WriteAsync(new UserAction(
        "UPDATE_POSITION",
        user.Id,
        $"Manual HTTP update to X={updatedUser.PositionX}, Y={updatedUser.PositionY}",
        updatedUser.PositionX,
        updatedUser.PositionY
    ));

    return Results.Ok(new { message = "Update queued" });
});

// web sockets
app.Map("/ws", async (HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        // get token
        var token = context.Request.Query["token"];
        Log("WS_AUTH", $"User with token {token} attempting to connect");

        // check if token is valid
        if (string.IsNullOrEmpty(token) || !tokens.ContainsKey(token!))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }


        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        // find user
        var user = tokens[token!];

        if (user == null)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.ProtocolError, "User not found", CancellationToken.None);
            return;
        }

        Log("WS_CONNECTED", $"Player {user.Name} (ID: {user.Id}) established WebSocket connection");

        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                // receive data from client
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                // handle incoming message
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var coords = JsonSerializer.Deserialize<Dictionary<string, float>>(message);

                    if (coords != null)
                    {
                        Log("WS_RECEIVE", $"Received coords from {user.Name}: X={coords["x"]}, Y={coords["y"]}");
                        await messageQueue.Writer.WriteAsync(new UserAction(
                            "UPDATE_POSITION",
                            user.Id,
                            $"User moved to {coords["x"]}, {coords["y"]}",
                            coords["x"],
                            coords["y"]
                        ));
                    }
                }
                // connection closed by client 
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Log("WS_DISCONNECT", $"Player {user.Name} closed connection gracefully");
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            Log("WS_ERROR", $"Connection lost for {user?.Name}. Error: {ex.Message}");
        }
        finally
        {
            if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
            {
                webSocket.Abort();
            }
            Log("WS_SESSION_ENDED", $"Session ended for: {user.Name}");
        }
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

// start

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();


// models
public record UserAction(string Type, int UserId, string Description, float X = 0, float Y = 0);
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public float PositionX { get; set; }
    public float PositionY { get; set; }
}

public class LogEntry
{
    public string Action { get; set; } = null!;
    public string Info { get; set; } = null!;
    public DateTime Time { get; set; }
}
