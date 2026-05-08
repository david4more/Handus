namespace Handus
{
    internal class Presenter
    {
        private readonly IGameView view;
        private readonly UserService userService;
        private enum Mode {Login, Register};

        public Presenter(IGameView view)
        {
            this.view = view;
            userService = new UserService();
        }

        public async Task Run()
        {
            User? user = null;
            string username = null!;
            string password = null!;
            string email = null!;
            while (user == null)
            {
                Mode mode = view.MenuChoice() == "1" ? Mode.Login : Mode.Register;
                switch (mode)
                {
                    case Mode.Login:
                        {
                            view.ShowMessage("Time to login!");
                            username = view.GetUsername();
                            user = await userService.GetUser(username);

                            if (user == null)
                            {
                                view.ShowMessage("User not found. Try to enter username again.");
                                break;

                            }
                            password = view.GetPassword();
                            if(user.Password != password)
                            {
                                view.ShowMessage("Wrong password.");
                                user = null;
                            }
                            break;
                        }
                    case Mode.Register:
                        {
                            view.ShowMessage("Time to register!");
                            username = view.GetUsername();
                            User? existingUser = await userService.GetUser(username);
                            if(existingUser != null)
                            {
                                view.ShowMessage("User already exists.");
                                break;
                            }
                            email = view.GetEmail();
                            password = view.GetPassword();
                            string password2 = view.GetSecondPassword();
                            if(password != password2)
                            {
                                view.ShowMessage("Passwords do not match.");
                                break;
                            }
                            user = await userService.CreateUser(username, email, password);

                            if (user == null)
                            {
                                view.ShowMessage("Error creating user.");
                            }
                            break;
                        }
                }
            }

            bool loggedIn = await userService.Login(user.Name);
            if (!loggedIn)
            {
                view.ShowMessage("Login failed!");
                return;
            }
            view.ShowMessage($"Welcome, {user.Name}!");

            view.StartGame(user);

            await userService.SendPosition(user.PositionX, user.PositionY);
        }
    }
}
