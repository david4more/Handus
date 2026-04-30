namespace Handus
{
    internal class Presenter
    {
        private readonly IGameView view;
        private readonly UserService userService;

        public Presenter(IGameView view)
        {
            this.view = view;
            userService = new UserService();
        }

        public async Task Run()
        {
            User? user = null;

            while (user == null)
            {
                string username = view.GetUsername();

                user = await userService.GetUser(username);

                if (user == null)
                {
                    string answer = view.AskYesNo("User not found. Create new user? (y/n)");

                    if (answer == "y")
                    {
                        user = await userService.CreateUser(username);

                        if (user == null)
                        {
                            view.ShowMessage("Error creating user.");
                        }
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
