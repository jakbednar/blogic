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

    // UserSessionService method
    public UserSessionService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    // Login method
    public async Task Login(User user)
    {
        CurrentUser = user;
        await _localStorage.SetItemAsync("userId", user.UserId);
        OnChange?.Invoke();
    }

    // Logout method
    public async Task Logout()
    {
        CurrentUser = null;
        await _localStorage.RemoveItemAsync("userId");
        OnChange?.Invoke();
    }

    public bool IsLoggedIn => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == "Admin";

    // SetFlashMessage method
    public void SetFlashMessage(string message)
    {
        FlashMessage = message;
    }

    // GetFlashMessage method
    public string? GetFlashMessage() => FlashMessage;

    // ClearFlashMessage method
    public void ClearFlashMessage()
    {
        FlashMessage = null;
    }

    // Refresh method
    public void Refresh(User user)
    {
        CurrentUser = user;
        OnChange?.Invoke();
    }

    // TryRestoreSession method
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