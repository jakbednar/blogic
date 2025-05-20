namespace blogic.Models;

public class CartItem
{
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }

    public int TotalPrice => Quantity * Product.Price;
}