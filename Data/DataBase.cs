using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

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
                ImageUrl TEXT
            );

            CREATE TABLE IF NOT EXISTS Products (
                ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,
                ImageUrl TEXT,
                IsDeleted INTEGER DEFAULT 0,
                DateCreated TEXT DEFAULT (datetime('now'))
            );

            CREATE TABLE IF NOT EXISTS Orders (
                OrderId INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                DateCreated TEXT DEFAULT (datetime('now')),
                FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );
        ");

        var existing = db.QueryFirstOrDefault("SELECT * FROM Users WHERE Email = @Email", new { Email = "admin@test.cz" });
        if (existing == null)
        {
            db.Execute("""
                INSERT INTO Users (Name, Email, Password, Role)
                VALUES ('Admin', 'admin@test.cz', 'admin123', 'Admin');
            """);
        }

        db.Execute("""
            INSERT INTO Products (Name, Price, Quantity)
            VALUES
            ('Birell', 5, 50),
            ('Pepsi 0.5l', 15, 25),
            ('7 Days Croissant', 10, 100);
        """);
    }
}