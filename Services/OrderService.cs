using Dapper;
using System.Data;
using blogic.Models;

namespace blogic.Services;

public class OrderService
{
    private readonly IDbConnection _db;

    // OrderService method
    public OrderService(IDbConnection db)
    {
        _db = db;
    }

    // GetOrdersByUserId method
    public async Task<List<OrderGroupView>> GetOrdersByUserId(int userId)
    {
        var raw = (await _db.QueryAsync<OrderFlat>(
            @"SELECT og.OrderId, og.DateCreated, oi.Quantity, p.Name AS ProductName, p.ImageUrl
              FROM OrderGroups og
              JOIN OrderItems oi ON og.OrderId = oi.OrderId
              JOIN Products p ON p.ProductId = oi.ProductId
              WHERE og.UserId = @UserId
              ORDER BY og.DateCreated DESC",
            new { UserId = userId }
        )).ToList();

        var groupedOrders = raw
            .GroupBy(o => new { o.OrderId, o.DateCreated })
            .Select(g => new OrderGroupView
            {
                OrderId = g.Key.OrderId,
                DateCreated = g.Key.DateCreated,
                Items = g.Select(i => new OrderItemView
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    ImageUrl = i.ImageUrl
                }).ToList()
            }).ToList();

        return groupedOrders;
    }
}

public class OrderFlat
{
    public int OrderId { get; set; }
    public string DateCreated { get; set; } = "";
    public int Quantity { get; set; }
    public string ProductName { get; set; } = "";
    public string ImageUrl { get; set; } = "";
}

public class OrderGroupView
{
    public int OrderId { get; set; }
    public string DateCreated { get; set; } = "";
    // new method
    public List<OrderItemView> Items { get; set; } = new();
}

public class OrderItemView
{
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = "";
}