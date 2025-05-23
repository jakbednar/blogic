namespace blogic.Models;

public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}
