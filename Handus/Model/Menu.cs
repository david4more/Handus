using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Handus.View;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Handus.Model
{
    internal class Menu : IUIElement
    {
        public enum MenuState {Login, Register};
        MenuState menuState = MenuState.Login;

        private readonly UserService userService;

        private static string filePrefix = "..\\..\\..\\Files\\";
        private static string fontFilePath = filePrefix + "ARLRDBD.TTF";

        private static IntRect TextBoxRect = new IntRect(new Vector2i(722, 0), new Vector2i(70, 21));
        private static IntRect NormalButtonRect = new IntRect(new Vector2i(504, 0), new Vector2i(36, 21));
        private static IntRect PressedButtonRect = new IntRect(new Vector2i(540, 0), new Vector2i(36, 21));

        public CheckButton loginButton = new(filePrefix + "Sprite-loginButton.png", 300f, 200f, NormalButtonRect, PressedButtonRect, true);
        public CheckButton registerButton = new(filePrefix + "Sprite-registerButton.png", 300f, 300f, NormalButtonRect, PressedButtonRect);

        public Text usernameLabel = new(new Font(fontFilePath), "Enter username:");
        public TextBox usernameTextBox = new(500f, 200f, TextBoxRect);

        public Text passwordLabel = new(new Font(fontFilePath), "Enter password:");
        public TextBox passwordTextBox = new(900f, 200f, TextBoxRect);

        public Text emailLabel = new(new Font(fontFilePath), "Enter email:");
        public TextBox emailTextBox = new(500f, 350f, TextBoxRect);

        public Text repeatPasswordLabel = new(new Font(fontFilePath), "Repeat password:");
        public TextBox repeatPasswordTextBox = new(900f, 350f, TextBoxRect);
        
        public Button confirmButton = new(filePrefix + "Sprite-confirmButton.png", 1050f, 475f, NormalButtonRect, PressedButtonRect);

        public Menu() 
        {
            userService = new UserService();
        }
        public void Update(RenderWindow window)
        {
            loginButton.Update(window);
            registerButton.Update(window);
            confirmButton.Update(window);
        }
        
        public void SetLabelsPositions()
        {
            usernameLabel.Position = new Vector2f(525f, 150f);
            passwordLabel.Position = new Vector2f(925f, 150f);
            emailLabel.Position = new Vector2f(525f, 300f);
            repeatPasswordLabel.Position = new Vector2f(925f, 300f);
        }
        public void Draw(RenderWindow window)
        {
            DrawLabels(window);
            DrawButtons(window);
            DrawTextBoxes(window);
        }
        public void DrawButtons(RenderWindow window)
        {
            loginButton.Draw(window);
            registerButton.Draw(window);
            confirmButton.Draw(window);
        }
        public void DrawTextBoxes(RenderWindow window)
        {
            usernameTextBox.Draw(window);
            passwordTextBox.Draw(window);
            if (menuState == MenuState.Register)
            {
                repeatPasswordTextBox.Draw(window);
                emailTextBox.Draw(window);
            }
        }
        public void DrawLabels(RenderWindow window)
        {
            window.Draw(usernameLabel);
            window.Draw(passwordLabel);
            if (menuState == MenuState.Register)
            {
                window.Draw(emailLabel);
                window.Draw(repeatPasswordLabel);
            }
        }
        public void StartGame(User user)
        {
            RenderWindow newWindow = new RenderWindow(
                new VideoMode(new Vector2u(1920, 1080)),
                "Handus",
                Styles.Default,
                State.Fullscreen);

            Engine engine = new(newWindow);

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
        public async void HandleConfirm(EventArgs e, RenderWindow window)
        {
                if (e is MouseButtonEventArgs)
                {
                    Vector2f mousePos = window.MapPixelToCoords(Mouse.GetPosition(window));
                    if (confirmButton.sprite.GetGlobalBounds().Contains(mousePos))
                    {
                        User? user = null;
                        string username = null!;
                        string password = null!;
                        string email = null!;
                            switch (menuState)
                            {
                                case MenuState.Login:
                                    {
                                        username = usernameTextBox.GetText();
                                        user = await userService.GetUser(username);
  
                                        if (user == null)
                                        {
                                            Console.WriteLine("User not found. Try to enter username again.");
                                            break;

                                        }
                                        password = passwordTextBox.GetText();
                                        if (user.Password != password)
                                        {
                                            Console.WriteLine("Wrong password.");
                                            user = null;
                                        }
                                        break;
                                    }
                                case MenuState.Register:
                                    {
                                        username = usernameTextBox.GetText();
                                        User? existingUser = await userService.GetUser(username);
                                        if (existingUser != null)
                                        {
                                            Console.WriteLine("User already exists.");
                                            break;
                                        }
                                        email = emailTextBox.GetText();
                                        password = passwordTextBox.GetText();
                                        string password2 = repeatPasswordTextBox.GetText();
                                        if (password != password2)
                                        {
                                            Console.WriteLine("Passwords do not match.");
                                            break;
                                        }
                                        user = await userService.CreateUser(username, email, password);

                                        if (user == null)
                                        {
                                            Console.WriteLine("Error creating user.");
                                        }
                                        break;
                                    }
                            }
                            bool loggedIn = await userService.Login(user.Name);
                            if (!loggedIn)
                            {
                                Console.WriteLine("Login failed!");
                                return;
                            }
                            Console.WriteLine($"Welcome, {user.Name}!");

                            StartGame(user);

                            await userService.SendPosition(user.PositionX, user.PositionY);
                    }
                }
        }
        public void HandleEvent(EventArgs e, RenderWindow window)
        {
            HandleConfirm(e,window);
            HandleText(e, window);
            HandleCheckButtons(e, window);
        }
        public void HandleText(EventArgs e, RenderWindow window)
        {
            usernameTextBox.HandleEvent(e, window);
            passwordTextBox.HandleEvent(e, window);
            repeatPasswordTextBox.HandleEvent(e, window);
            emailTextBox.HandleEvent(e, window);
        }
        public void HandleCheckButtons(EventArgs e, RenderWindow window)
        {
            if (loginButton.HandleEventAndCheck(e, window))
            {
                registerButton.IsChecked = false;
                menuState = MenuState.Login;

            }
            if (registerButton.HandleEventAndCheck(e, window))
            {
                loginButton.IsChecked = false;
                menuState = MenuState.Register;
            }
        }
    }
}
