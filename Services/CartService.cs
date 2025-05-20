using blogic.Models;

namespace blogic.Services;

public class CartService
{
    public int GetCartCount() => _cartproducts.Count;
    public List<Product> GetCartProducts() => _cartproducts;
    
    public event Action? OnChange;

    public void Add(Product product)
    {
        _cartproducts.Add(product);
        OnChange?.Invoke();
    }

    private readonly List<Product> _cartproducts = new()
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