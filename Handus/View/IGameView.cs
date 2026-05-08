namespace Handus;

public interface IGameView
{
    string MenuChoice();
    string GetUsername();
    string GetPassword();
    string GetSecondPassword();
    string GetEmail();
    void ShowMessage(string message);
    void StartGame(User user);
}
