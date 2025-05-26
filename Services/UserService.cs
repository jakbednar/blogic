using blogic.Models;
using Dapper;
using System.Data;

namespace blogic.Services;

public class UserService
{
    private readonly IDbConnection _db;

    public UserService(IDbConnection db)
    {
        _db = db;
    }

    public bool Register(User user)
    {
        var existing = _db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new { user.Email });
        if (existing != null) return false;

        string hash = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.Password = hash;

        _db.Execute("""
                        INSERT INTO Users (Name, Email, Password, Credit, Role, ImageUrl)
                        VALUES (@Name, @Email, @Password, @Credit, @Role, @ImageUrl)
                    """, user);

        return true;
    }

    public User? Login(string email, string password)
    {
        var user = _db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new { Email = email });
        if (user == null) return null;

        bool valid = BCrypt.Net.BCrypt.Verify(password, user.Password);
        return valid ? user : null;
    }
    
    public async Task<List<User>> GetAllUsers()
    {
        var sql = "SELECT * FROM Users";
        var users = await _db.QueryAsync<User>(sql);
        return users.ToList();
    }
}