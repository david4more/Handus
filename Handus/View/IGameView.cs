namespace Handus;

public interface IGameView
{
    string GetUsername();
    void ShowMessage(string message);
    string AskYesNo(string message);
    void StartGame(User user);
}
