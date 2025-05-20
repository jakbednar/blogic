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
            ImageUrl = "https://www.birell.cz/wp-content/uploads/2024/04/Flavor_PomeloGrep.png",
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
            ImageUrl = "https://www.benu.cz/data/images/img-large-product/9/40149.jpg",
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
            ImageUrl = "https://www.bohemiachips.cz/wp-content/uploads/2023/01/bohemia-redline-sul-647x1024_new.png",
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
            ImageUrl = "https://digitalcontent.api.tesco.com/v2/media/ghs/d6056d1f-8a49-4d2e-8f3b-2caa55bcd9b9/c27773e3-61f6-4323-bf42-1b2eddf7ae93_178254785.jpeg?h=960&w=960",
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
            ImageUrl = "https://digitalcontent.api.tesco.com/v2/media/ghs/f68c4acc-2f64-4d0a-84f6-37ad2d3bd6cb/3fb4fffa-c8b1-46c4-80da-f67df8291bbd_1606819767.jpeg?h=960&w=960",
            IsDeleted = false
        }
    };
}