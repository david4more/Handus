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
        private enum AppState { Menu, Game }
        private AppState state = AppState.Menu;

        private RenderWindow window = new RenderWindow(new VideoMode(new Vector2u(1920, 1080)),
            "Handus",
            Styles.Default,
            State.Fullscreen);
        private Menu menu = new Menu();
        private Engine engine;
        private Clock clock = new Clock();

        public Presenter()
        {
            window.TextEntered += (sender, e) => { menu.HandleEvent(e, window); };
            window.MouseButtonPressed += (sender, e) => { menu.HandleEvent(e, window); };
            window.Closed += (sender, e) => window.Close();
            window.KeyPressed += (sender, e) => { if (e.Code == Keyboard.Key.Escape) window.Close(); };
            menu.SetLabelsPositions();
            menu.OnLoginSuccess = StartGame;
        }
        public void StartGame(User user)
        {
            engine = new(window);
            state = AppState.Game;

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


            user.PositionX = engine.player.PositionX;
            user.PositionY = engine.player.PositionY;
        }
        public async Task Run()
        {
            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear();
                if (state == AppState.Menu)
                {
                    menu.Update(window);
                    menu.Draw(window);
                }
                if (state == AppState.Game)
                {
                    float dt = clock.Restart().AsSeconds();

                    engine.Update(dt);
                    engine.Render();
                }
            window.Display();
            }

        }
    }
}
