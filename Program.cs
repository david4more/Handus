using SFML.Graphics;
using Handus;
using SFML.System;
using SFML.Window;
using System.Net.Http.Json;

var client = new HttpClient();
User? user = null;
bool newUser = false;

// цикл для реєстрації входу/користувача
while (user == null)
{
    Console.WriteLine("Enter your username:");
    string usernameInput = Console.ReadLine() ?? "";

    try
    {
        user = await client.GetFromJsonAsync<User>($"https://localhost:7220/users/byname/{usernameInput}");
    }
    catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        Console.WriteLine("User not found.");
        Console.WriteLine("Do you want to create a new user? (y/n)");
        string answer = Console.ReadLine() ?? "n";

        if (answer.ToLower() == "y")
        {
            user = await CreateNewUser();
            newUser = true;

        }
        else
        {
            Console.WriteLine("Try entering username again.");
        }
    }
}

Console.WriteLine($"Welcome, {user.Name}!");

// метод створення нового користувача
async Task<User?> CreateNewUser()
{
    Console.WriteLine("Enter new username:");
    string newUsername = Console.ReadLine() ?? "Unnamed";

    var response = await client.PostAsJsonAsync("https://localhost:7220/users", new { name = newUsername });

    if (response.IsSuccessStatusCode)
    {
        var createdUser = await response.Content.ReadFromJsonAsync<User>();
        Console.WriteLine($"User '{createdUser?.Name}' created successfully!");
        return createdUser;
    }
    else
    {
        Console.WriteLine("Error creating user.");
        return null;
    }
}

// створення вікна і двигуна гри
RenderWindow window = new RenderWindow(new VideoMode(new Vector2u(1920, 1080)), "Handus", Styles.Default, State.Fullscreen);
Engine engine = new(window);
//встановлення позиції гравця
if (!newUser)
{
    engine.player.PositionX = user.PositionX;
    engine.player.PositionY = user.PositionY;
}

engine.Loop();

//збереження позиції гравця
user.PositionX = engine.player.PositionX;
user.PositionY = engine.player.PositionY;

await client.PutAsJsonAsync($"https://localhost:7220/users/{user.Id}", user);

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public float PositionX { get; set; }
    public float PositionY { get; set; }
}
