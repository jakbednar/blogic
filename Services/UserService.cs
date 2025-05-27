using blogic.Models;
using Dapper;
using System.Data;

namespace blogic.Services;

public class UserService
{
    private readonly IDbConnection _db;

    // UserService method
    public UserService(IDbConnection db)
    {
        _db = db;
    }

    // Register method
    public bool Register(User user)
    {
        var existing = _db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new { user.Email });
        if (existing != null) return false;

        if (string.IsNullOrWhiteSpace(user.ImageUrl))
        {
            user.ImageUrl = "/images/user.png";
        }

        string hash = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.Password = hash;

        _db.Execute("""
                        INSERT INTO Users (Name, Email, Password, Credit, Role, ImageUrl)
                        VALUES (@Name, @Email, @Password, @Credit, @Role, @ImageUrl)
                    """, user);

        return true;
    }


    // Login method
    public User? Login(string email, string password)
    {
        var user = _db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new { Email = email });
        if (user == null) return null;

        bool valid = BCrypt.Net.BCrypt.Verify(password, user.Password);
        return valid ? user : null;
    }
    
    // GetAllUsers method
    public async Task<List<User>> GetAllUsers()
    {
        var sql = "SELECT * FROM Users";
        var users = await _db.QueryAsync<User>(sql);
        return users.ToList();
    }
    
    // GetUserById method
    public async Task<User?> GetUserById(int userId)
    {
        var sql = "SELECT * FROM Users WHERE UserId = @UserId";
        return await _db.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
    }

}