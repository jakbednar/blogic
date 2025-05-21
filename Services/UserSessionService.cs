namespace blogic.Services;

using blogic.Models;

public class UserSessionService
{
    public User? CurrentUser { get; private set; }

    public event Action? OnChange; // ⬅ přidáš toto

    public void Login(User user)
    {
        CurrentUser = user;
        OnChange?.Invoke(); // ⬅ přidáš toto
    }

    public void Logout()
    {
        CurrentUser = null;
        OnChange?.Invoke(); // ⬅ přidáš toto
    }

    public bool IsLoggedIn => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == "Admin";
}