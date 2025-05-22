using blogic.Models;
using Dapper;
using System.Data;

namespace blogic.Services;

public class ProductService
{
    private readonly IDbConnection _db;

    public ProductService(IDbConnection db)
    {
        _db = db;
    }

    public List<Product> GetAllProducts()
    {
        return _db.Query<Product>("SELECT * FROM Products WHERE IsDeleted = 0").ToList();
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        var result = await _db.QueryAsync<Product>("SELECT * FROM Products WHERE IsDeleted = 0");
        return result.ToList();
    }

    public void AddProduct(Product product)
    {
        _db.Execute(@"
            INSERT INTO Products (ProductID, Name, Price, Quantity, ImageUrl, DateCreated, CreatedBy, IsDeleted)
            VALUES (@ProductID, @Name, @Price, @Quantity, @ImageUrl, @DateCreated, @CreatedBy, @IsDeleted)", product);
    }

    public int NextId()
    {
        var maxId = _db.ExecuteScalar<int?>("SELECT MAX(ProductID) FROM Products");
        return (maxId ?? 0) + 1;
    }

    public void UpdateProduct(Product product)
    {
        _db.Execute(@"
            UPDATE Products 
            SET Name = @Name, Price = @Price, Quantity = @Quantity, ImageUrl = @ImageUrl
            WHERE ProductID = @ProductID", product);
    }

    public void RemoveProduct(int productId)
    {
        _db.Execute("UPDATE Products SET IsDeleted = 1 WHERE ProductID = @Id", new { Id = productId });
    }
}