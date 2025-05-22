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

        // Vytvoření tabulek
        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Users (
                UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL,
                Role TEXT NOT NULL CHECK (Role IN ('User', 'Admin')),
                ImageUrl TEXT
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

            CREATE TABLE IF NOT EXISTS Orders (
                OrderId INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                DateCreated TEXT DEFAULT (datetime('now')),
                FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );

            CREATE TABLE IF NOT EXISTS CartItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    Quantity INTEGER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);


        ");

        // Vložení admin účtu, pokud neexistuje
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

        // Vložení testovacího usera, pokud neexistuje
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

        // Vložení testovacích produktů jen pokud není žádný
        var productExists = db.QueryFirstOrDefault("SELECT 1 FROM Products LIMIT 1");
        if (productExists == null)
        {
            db.Execute("""
                INSERT INTO Products (Name, Price, Quantity, CreatedBy)
                VALUES
                ('Birell', 5, 50, 1),
                ('Pepsi 0.5l', 15, 25, 1),
                ('7 Days Croissant', 10, 100, 1);
            """);

            Console.WriteLine("✅ Testovací produkty vytvořeny.");
        }
    }
}