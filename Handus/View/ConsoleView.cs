using SFML.Graphics;
using SFML.System;
using SFML.Window;
namespace Handus;

public class ConsoleView : IGameView
{
    public string GetUsername()
    {
        Console.WriteLine("Enter username:");
        return Console.ReadLine() ?? "";
    }
    public string GetPassword()
    {
        Console.WriteLine("Enter password:");
        return Console.ReadLine() ?? "";
    }
    public string GetSecondPassword()
    {
        Console.WriteLine("Repeat password:");
        return Console.ReadLine() ?? "";
    }
    public string GetEmail()
    {
        Console.WriteLine("Enter email:");
        return Console.ReadLine() ?? "";
    }
    public string MenuChoice()
    {
        string choice = null!;
        while (choice != "1" && choice != "2") {
            Console.WriteLine("What do you want to do?\n1 - login\n2 - register");
            choice = Console.ReadLine() ?? "";
        }
        return choice;
        }
    public void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void StartGame(User user)
    {
        RenderWindow window = new RenderWindow(
            new VideoMode(new Vector2u(1920, 1080)),
            "Handus",
            Styles.Default,
            State.Fullscreen);

        Engine engine = new(window);

        if (user.PositionX == 0 && user.PositionY == 0)
        {
            engine.player.PositionX = engine.level.spawnPoint.X;
            engine.player.PositionY = engine.level.spawnPoint.Y;
        }
        else
        {
            engine.player.PositionX = user.PositionX;
            engine.player.PositionY = user.PositionY;
        }

        engine.Loop();

        user.PositionX = engine.player.PositionX;
        user.PositionY = engine.player.PositionY;
    }
}
