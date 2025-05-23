using blogic.Models;
using Blazored.LocalStorage;
using Dapper;
using System.Data;

namespace blogic.Services;

public class UserSessionService
{
    private readonly ILocalStorageService _localStorage;

    public User? CurrentUser { get; private set; }
    public string? FlashMessage { get; private set; }

    public event Action? OnChange;

    public UserSessionService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task Login(User user)
    {
        CurrentUser = user;
        await _localStorage.SetItemAsync("userId", user.UserId);
        OnChange?.Invoke();
    }

    public async Task Logout()
    {
        CurrentUser = null;
        await _localStorage.RemoveItemAsync("userId");
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

    public async Task TryRestoreSession(IDbConnection db)
    {
        var userId = await _localStorage.GetItemAsync<int?>("userId");
        if (userId is not null)
        {
            var user = db.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE UserId = @Id",
                new { Id = userId.Value });

            if (user != null)
            {
                CurrentUser = user;
                OnChange?.Invoke();
            }
        }
    }

}