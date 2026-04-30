using SFML.Graphics;
using SFML.System;
using SFML.Window;
namespace Handus;

public class ConsoleView : IGameView
{
    public string GetUsername()
    {
        Console.WriteLine("Enter your username:");
        return Console.ReadLine() ?? "";
    }

    public void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }

    public string AskYesNo(string message)
    {
        Console.WriteLine(message);
        return Console.ReadLine()?.ToLower() ?? "n";
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
