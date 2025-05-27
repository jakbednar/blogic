using blogic.Models;
using Dapper;
using System.Data;

namespace blogic.Services;

public class CartService
{
    private readonly IDbConnection _db;
    private readonly UserSessionService _session;

    public event Action? OnChange;

    // CartService method
    public CartService(IDbConnection db, UserSessionService session)
    {
        _db = db;
        _session = session;
    }

    // NotifyStateChanged method
    private void NotifyStateChanged() => OnChange?.Invoke();

    // GetCartItems method
    public List<CartItem> GetCartItems()
    {
        if (!_session.IsLoggedIn) return new();

        var sql = @"
            SELECT c.Id, c.UserId, c.ProductId, c.Quantity,
                   p.ProductId, p.Name, p.Price, p.Quantity AS ProductQuantity, p.ImageUrl, p.IsDeleted, p.DateCreated, p.CreatedBy
            FROM CartItems c
            JOIN Products p ON p.ProductId = c.ProductId
            WHERE c.UserId = @UserId";

        var items = _db.Query<CartItem, Product, CartItem>(
            sql,
            (cart, product) =>
            {
                cart.Product = product;
                return cart;
            },
            new { UserId = _session.CurrentUser!.UserId },
            splitOn: "ProductId"
        ).ToList();

        return items;
    }

    // AddToCart method
    public void AddToCart(int productId)
    {
        if (!_session.IsLoggedIn) return;

        var existing = _db.QueryFirstOrDefault<CartItem>(
            "SELECT * FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId",
            new { UserId = _session.CurrentUser!.UserId, ProductId = productId });

        if (existing != null)
        {
            _db.Execute("UPDATE CartItems SET Quantity = Quantity + 1 WHERE Id = @Id", new { Id = existing.Id });
        }
        else
        {
            _db.Execute("INSERT INTO CartItems (UserId, ProductId, Quantity) VALUES (@UserId, @ProductId, 1)",
                new { UserId = _session.CurrentUser!.UserId, ProductId = productId });
        }

        NotifyStateChanged();
    }

    // RemoveProduct method
    public void RemoveProduct(int productId)
    {
        if (!_session.IsLoggedIn) return;

        _db.Execute("DELETE FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId",
            new { UserId = _session.CurrentUser!.UserId, ProductId = productId });

        NotifyStateChanged();
    }

    // IncreaseQuantity method
    public void IncreaseQuantity(int productId)
    {
        if (!_session.IsLoggedIn) return;

        var item = _db.QueryFirstOrDefault<CartItem>(
            "SELECT * FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId",
            new { UserId = _session.CurrentUser!.UserId, ProductId = productId });

        var stock = _db.ExecuteScalar<int>(
            "SELECT Quantity FROM Products WHERE ProductId = @ProductId",
            new { ProductId = productId });

        if (item != null && item.Quantity < stock)
        {
            _db.Execute(
                "UPDATE CartItems SET Quantity = Quantity + 1 WHERE Id = @Id",
                new { Id = item.Id });

            NotifyStateChanged();
        }
        else if (item == null && stock > 0)
        {
            _db.Execute(
                "INSERT INTO CartItems (UserId, ProductId, Quantity) VALUES (@UserId, @ProductId, 1)",
                new { UserId = _session.CurrentUser!.UserId, ProductId = productId });

            NotifyStateChanged();
        }
    }

    // DecreaseQuantity method
    public void DecreaseQuantity(int productId)
    {
        if (!_session.IsLoggedIn) return;

        var item = _db.QueryFirstOrDefault<CartItem>(
            "SELECT * FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId",
            new { UserId = _session.CurrentUser!.UserId, ProductId = productId });

        if (item != null)
        {
            if (item.Quantity > 1)
            {
                _db.Execute("UPDATE CartItems SET Quantity = Quantity - 1 WHERE Id = @Id", new { Id = item.Id });
            }
            else
            {
                _db.Execute("DELETE FROM CartItems WHERE Id = @Id", new { Id = item.Id });
            }

            NotifyStateChanged();
        }
    }

    // ClearCart method
    public void ClearCart()
    {
        if (!_session.IsLoggedIn) return;

        _db.Execute("DELETE FROM CartItems WHERE UserId = @UserId",
            new { UserId = _session.CurrentUser!.UserId });

        NotifyStateChanged();
    }

    // GetCartCount method
    public int GetCartCount()
    {
        if (!_session.IsLoggedIn) return 0;

        return _db.ExecuteScalar<int>(
            "SELECT IFNULL(SUM(Quantity), 0) FROM CartItems WHERE UserId = @UserId",
            new { UserId = _session.CurrentUser!.UserId });
    }
    
    // GetCartByUserId method
    public List<CartItem> GetCartByUserId(int userId)
    {
        var sql = @"
        SELECT c.Id, c.UserId, c.ProductId, c.Quantity,
               p.ProductId, p.Name, p.Price, p.Quantity AS ProductQuantity, p.ImageUrl, p.IsDeleted, p.DateCreated, p.CreatedBy
        FROM CartItems c
        JOIN Products p ON p.ProductId = c.ProductId
        WHERE c.UserId = @UserId";

        var items = _db.Query<CartItem, Product, CartItem>(
            sql,
            (cart, product) =>
            {
                cart.Product = product;
                return cart;
            },
            new { UserId = userId },
            splitOn: "ProductId"
        ).ToList();

        return items;
    }
}
