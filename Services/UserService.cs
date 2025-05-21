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
        // Check if user exists
        var existing = _db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new { user.Email });
        if (existing != null) return false;

        // Hash password
        string hash = BCrypt.Net.BCrypt.HashPassword(user.Password); // pouzijeme Password
        user.Password = hash;

        _db.Execute("""
                        INSERT INTO Users (Name, Email, Password, Role, ImageUrl)
                        VALUES (@Name, @Email, @Password, @Role, @ImageUrl)
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
}