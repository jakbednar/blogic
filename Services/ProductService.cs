using blogic.Models;

namespace blogic.Services;

public class ProductService
{
    public List<Product> GetAllProducts() => _products;
    
    private readonly List<Product> _products = new()
    {
        new Product
        {
            ProductID = 1,
            DateCreated = DateTime.UtcNow,
            CreatedBy = 1,
            Name = "Birell",
            Price = 5,
            Quantity = 50,
            ImageUrl = "https://example.com/images/birell.png",
            IsDeleted = false
        },
        new Product
        {
            ProductID = 2,
            DateCreated = DateTime.UtcNow,
            CreatedBy = 1,
            Name = "Anticol",
            Price = 10,
            Quantity = 100,
            ImageUrl = "https://example.com/images/anticol.png",
            IsDeleted = false
        },
        new Product
        {
            ProductID = 3,
            DateCreated = DateTime.UtcNow,
            CreatedBy = 1,
            Name = "Bohemia Chips",
            Price = 10,
            Quantity = 40,
            ImageUrl = "https://example.com/images/bohemia.png",
            IsDeleted = false
        },
        new Product
        {
            ProductID = 4,
            DateCreated = DateTime.UtcNow,
            CreatedBy = 1,
            Name = "Pepsi 0.5l",
            Price = 15,
            Quantity = 30,
            ImageUrl = "https://example.com/images/pepsi.png",
            IsDeleted = false
        },
        new Product
        {
            ProductID = 5,
            DateCreated = DateTime.UtcNow,
            CreatedBy = 1,
            Name = "7 Days Croissant",
            Price = 10,
            Quantity = 25,
            ImageUrl = "https://example.com/images/croissant.png",
            IsDeleted = false
        }
    };
}