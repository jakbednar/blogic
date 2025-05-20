using blogic.Models;

namespace blogic.Services;

public class CartService
{
    private readonly List<CartItem> _cartItems = new();

    public event Action? OnChange;

    public void Add(Product product)
    {
        var existingItem = _cartItems.FirstOrDefault(ci => ci.Product.ProductID == product.ProductID);

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            _cartItems.Add(new CartItem
            {
                Product = product,
                Quantity = 1
            });
        }

        OnChange?.Invoke();
    }

    public void Remove(int productId)
    {
        var item = _cartItems.FirstOrDefault(ci => ci.Product.ProductID == productId);
        if (item != null)
        {
            _cartItems.Remove(item);
            OnChange?.Invoke();
        }
    }

    public List<CartItem> GetCartItems() => _cartItems;

    public int GetCartCount() => _cartItems.Sum(ci => ci.Quantity);

    public decimal GetTotalPrice() => _cartItems.Sum(ci => ci.Product.Price * ci.Quantity);

    public void Clear()
    {
        _cartItems.Clear();
        OnChange?.Invoke();
    }
}