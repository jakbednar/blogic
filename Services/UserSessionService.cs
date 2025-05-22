namespace blogic.Services;

using blogic.Models;

public class UserSessionService
{
    public User? CurrentUser { get; private set; }
    public string? FlashMessage { get; private set; }

    public event Action? OnChange;

    public void Login(User user)
    {
        CurrentUser = user;
        OnChange?.Invoke();
    }

    public void Logout()
    {
        CurrentUser = null;
        OnChange?.Invoke();
    }

    public bool IsLoggedIn => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == "Admin";

    public void SetFlashMessage(string message)
    {
        FlashMessage = message;
    }

    public string? GetFlashMessage() => FlashMessage;

    public void ClearFlashMessage()
    {
        FlashMessage = null;
    }

    public void Refresh(User user)
    {
        CurrentUser = user;
        OnChange?.Invoke();
    }
}