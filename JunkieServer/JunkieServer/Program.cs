using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// файли для збереження
string usersFile = "users.json";
string logFile = "log.json";

// дані для збереження
List<User> users = new();

// завантаження
if (File.Exists(usersFile))
    users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(usersFile)) ?? new();

// збереження
void SaveUsers() =>
    File.WriteAllText(usersFile, JsonSerializer.Serialize(users));

// логування
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

// робота з юзерами

app.MapGet("/users/byname/{username}", (string username) =>
{
    var user = users.FirstOrDefault(u => u.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapGet("/users", () =>
{
    Log("GET USERS", "All users requested");
    return users;
});

app.MapPut("/users/{id}", (int id, User updatedUser) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user == null) return Results.NotFound();

    user.PositionX = updatedUser.PositionX;
    user.PositionY = updatedUser.PositionY;

    SaveUsers();
    Log("UPDATE USER POSITION", $"User {user.Name} position updated to X={user.PositionX}, Y={user.PositionY}");

    return Results.Ok(user);
});

app.MapPost("/users", (User user) =>
{
    user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
    users.Add(user);
    SaveUsers();

    Log("CREATE USER", $"User {user.Name} created");

    return Results.Ok(user);
});

// початок роботи

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


// класи

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public float PositionX { get; set; }
    public float PositionY { get; set; }
}

public class LogEntry
{
    public string Action { get; set; } = null!;
    public string Info { get; set; } = null!;
    public DateTime Time { get; set; }
}
