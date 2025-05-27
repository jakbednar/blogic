using blogic.Models;
using Dapper;
using System.Data;
using System.Linq;

namespace blogic.Services;

public class ProductService
{
    private readonly IDbConnection _db;

    // ProductService method
    public ProductService(IDbConnection db)
    {
        _db = db;
    }

    // GetAllProducts method
    public List<Product> GetAllProducts()
    {
        return _db.Query<Product>("SELECT * FROM Products WHERE IsDeleted = 0").ToList();
    }

    // GetAllProductsAsync method
    public async Task<List<Product>> GetAllProductsAsync()
    {
        var result = await _db.QueryAsync<Product>("SELECT * FROM Products WHERE IsDeleted = 0");
        return result.ToList();
    }

    // AddProduct method
    public void AddProduct(Product product)
    {
        _db.Execute(@"
            INSERT INTO Products (ProductID, Name, Price, Quantity, ImageUrl, DateCreated, CreatedBy, IsDeleted)
            VALUES (@ProductID, @Name, @Price, @Quantity, @ImageUrl, @DateCreated, @CreatedBy, @IsDeleted)", product);
    }

    // NextId method
    public int NextId()
    {
        var maxId = _db.ExecuteScalar<int?>("SELECT MAX(ProductID) FROM Products");
        return (maxId ?? 0) + 1;
    }

    // UpdateProduct method
    public void UpdateProduct(Product product)
    {
        _db.Execute(@"
            UPDATE Products 
            SET Name = @Name, Price = @Price, Quantity = @Quantity, ImageUrl = @ImageUrl
            WHERE ProductID = @ProductID", product);
    }

    // RemoveProduct method
    public void RemoveProduct(int productId)
    {
        _db.Execute("UPDATE Products SET IsDeleted = 1 WHERE ProductID = @Id", new { Id = productId });
    }
    
    // DecreaseStockAsync method
    public async Task DecreaseStockAsync(int productId, int quantity)
    {
        await _db.ExecuteAsync(@"
        UPDATE Products 
        SET Quantity = Quantity - @Quantity 
        WHERE ProductID = @ProductId", new { ProductId = productId, Quantity = quantity });
    }
    
    // GetReservedQuantities method
    public Dictionary<int, int> GetReservedQuantities()
    {
        var sql = "SELECT ProductId, SUM(Quantity) AS Reserved FROM CartItems GROUP BY ProductId";
        return _db.Query(sql)
            .ToDictionary(row => (int)row.ProductId, row => (int?)row.Reserved ?? 0);
    }

}