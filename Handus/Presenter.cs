using System.Numerics;
using Handus.Model;
using Handus.View;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
namespace Handus
{
    internal class Presenter
    {
        RenderWindow window = new RenderWindow(new VideoMode(new Vector2u(1920, 1080)),
            "Handus",
            Styles.Default,
            State.Fullscreen);
        Menu menu = new Menu();

        public Presenter()
        {
            window.TextEntered += (sender, e) => { menu.HandleEvent(e, window); };
            window.MouseButtonPressed += (sender, e) => { menu.HandleEvent(e, window); };
            window.Closed += (sender, e) => window.Close();
            window.KeyPressed += (sender, e) => { if (e.Code == Keyboard.Key.Escape) window.Close(); };
            menu.SetLabelsPositions();
        }

        public async Task Run()
        {
            while (window.IsOpen)
            {
                window.DispatchEvents();
                menu.Update(window);
                window.Clear();
                menu.Draw(window);
                window.Display();
            }
        }
    }
}
