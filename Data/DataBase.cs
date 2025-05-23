using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using BCrypt.Net;

namespace blogic.Data;

public static class Database
{
    private static string _connectionString = "Data Source=Data/DutyFree.db";

    public static IDbConnection Get() => new SqliteConnection(_connectionString);

    public static void InitDb()
{
    using var db = Get();

    db.Execute(@"
        CREATE TABLE IF NOT EXISTS Users (
            UserId INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Email TEXT NOT NULL UNIQUE,
            Password TEXT NOT NULL,
            Role TEXT NOT NULL CHECK (Role IN ('User', 'Admin')),
            ImageUrl TEXT,
            Credit INTEGER DEFAULT 0
        );

        CREATE TABLE IF NOT EXISTS Products (
            ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Price INTEGER NOT NULL,
            Quantity INTEGER NOT NULL,
            ImageUrl TEXT,
            ImageFile BLOB,
            IsDeleted INTEGER DEFAULT 0,
            DateCreated TEXT DEFAULT (datetime('now')),
            CreatedBy INTEGER NOT NULL DEFAULT 1,
            FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
        );

        CREATE TABLE IF NOT EXISTS CartItems (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER NOT NULL,
            ProductId INTEGER NOT NULL,
            Quantity INTEGER NOT NULL,
            FOREIGN KEY (UserId) REFERENCES Users(UserId),
            FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
        );

        CREATE TABLE IF NOT EXISTS OrderGroups (
            OrderId INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER NOT NULL,
            DateCreated TEXT DEFAULT (datetime('now')),
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
        );

        CREATE TABLE IF NOT EXISTS OrderItems (
            OrderItemId INTEGER PRIMARY KEY AUTOINCREMENT,
            OrderId INTEGER NOT NULL,
            ProductId INTEGER NOT NULL,
            Quantity INTEGER NOT NULL,
            FOREIGN KEY (OrderId) REFERENCES OrderGroups(OrderId),
            FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
        );
    ");

    // Admin účet
    var adminExists = db.QueryFirstOrDefault("SELECT 1 FROM Users WHERE Email = @Email", new { Email = "admin@test.cz" });
    if (adminExists == null)
    {
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        db.Execute("""
            INSERT INTO Users (Name, Email, Password, Role)
            VALUES (@Name, @Email, @Password, @Role);
        """, new
        {
            Name = "Admin",
            Email = "admin@test.cz",
            Password = adminPasswordHash,
            Role = "Admin"
        });

        Console.WriteLine("✅ Admin účet vytvořen.");
    }

    // Testovací user
    var userExists = db.QueryFirstOrDefault("SELECT 1 FROM Users WHERE Email = @Email", new { Email = "user@test.cz" });
    if (userExists == null)
    {
        var userPasswordHash = BCrypt.Net.BCrypt.HashPassword("user123");
        db.Execute("""
            INSERT INTO Users (Name, Email, Password, Role)
            VALUES (@Name, @Email, @Password, @Role);
        """, new
        {
            Name = "Testovací uživatel",
            Email = "user@test.cz",
            Password = userPasswordHash,
            Role = "User"
        });

        Console.WriteLine("✅ Testovací user vytvořen.");
    }

    // Testovací produkty
    var productExists = db.QueryFirstOrDefault("SELECT 1 FROM Products LIMIT 1");
    if (productExists == null)
    {
        db.Execute(@"
            INSERT INTO Products (Name, Price, Quantity, ImageUrl, CreatedBy)
            VALUES
            ('Birell', 5, 50, 'https://www.birell.cz/wp-content/uploads/2024/04/Flavor_PomeloGrep.png', 1),
            ('Pepsi 0.5l', 15, 25, 'https://digitalcontent.api.tesco.com/v2/media/ghs/d6056d1f-8a49-4d2e-8f3b-2caa55bcd9b9/c27773e3-61f6-4323-bf42-1b2eddf7ae93_178254785.jpeg?h=960&w=960', 1),
            ('7 Days Croissant', 10, 100, 'https://digitalcontent.api.tesco.com/v2/media/ghs/63fd7094-cda5-429c-8a9b-ccb050f992bc/9020513e-6334-4fd0-9c86-59690d8cdfc1_699790999.jpeg?h=960&w=960', 1),
            ('Milka čokoláda', 22, 40, 'https://digitalcontent.api.tesco.com/v2/media/ghs/a5c4bcd9-cee1-4c67-8722-3144c0e6cc36/75462dc6-c90d-4857-ad90-6065db18c6f6_945773882.jpeg?h=960&w=960', 1),
            ('RedBull 250ml', 30, 20, 'https://digitalcontent.api.tesco.com/v2/media/ghs/1eca72a3-8a91-40e0-9b82-cc029ba71911/af635662-7acf-45e5-ae4e-5e32fc678c50_1781248521.jpeg?h=960&w=960', 1),
            ('Tatranka', 8, 60, 'https://digitalcontent.api.tesco.com/v2/media/ghs/3b9113c2-6686-4653-b265-722e8f72608e/2aad6e2c-eed5-449b-9b1f-0cdffc1ac736_1040641616.jpeg?h=960&w=960', 1);
        ");

        Console.WriteLine("✅ Testovací produkty vytvořeny.");
    }
}

}