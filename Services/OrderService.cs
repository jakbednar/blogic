using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blogic.Models;

namespace blogic.Services
{
    public class OrderService
    {
        private readonly IDbConnection _db;

        public OrderService(IDbConnection db)
        {
            _db = db;
        }

        public async Task<List<OrderGroupView>> GetOrdersByUserId(int userId)
        {
            var raw = (await _db.QueryAsync<OrderFlat>(
                @"SELECT
                    og.OrderId,
                    og.DateCreated,
                    oi.Quantity,
                    p.Price       AS ItemPrice,
                    p.Name        AS ProductName,
                    p.ImageUrl    AS ProductImageUrl
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
                    OrderId     = g.Key.OrderId,
                    DateCreated = g.Key.DateCreated,
                    Items       = g.Select(i => new OrderItemView
                    {
                        ProductName = i.ProductName,
                        Quantity    = i.Quantity,
                        Price       = i.ItemPrice,
                        ImageUrl    = i.ProductImageUrl
                    }).ToList()
                })
                .ToList();

            return groupedOrders;
        }

        public async Task<List<OrderWithUserView>> GetAllOrdersWithUserAsync()
        {
            var raw = (await _db.QueryAsync<OrderFlatUser>(
                @"SELECT
                    og.OrderId,
                    og.DateCreated,
                    u.UserId,
                    u.Name        AS UserName,
                    u.Email       AS UserEmail,
                    u.Role        AS UserRole,
                    u.Credit      AS UserCredit,
                    u.ImageUrl    AS UserImageUrl,
                    oi.Quantity,
                    p.Price      AS ItemPrice,
                    p.Name        AS ProductName,
                    p.ImageUrl    AS ProductImageUrl
                  FROM OrderGroups og
                  JOIN [Users] u ON og.UserId = u.UserId
                  JOIN OrderItems oi ON og.OrderId = oi.OrderId
                  JOIN Products p ON oi.ProductId = p.ProductId
                  ORDER BY og.DateCreated DESC"
            )).ToList();

            var grouped = raw
                .GroupBy(r => new
                {
                    r.OrderId,
                    r.DateCreated,
                    r.UserId,
                    r.UserName,
                    r.UserEmail,
                    r.UserRole,
                    r.UserCredit,
                    r.UserImageUrl
                })
                .Select(g => new OrderWithUserView
                {
                    OrderId     = g.Key.OrderId,
                    DateCreated = g.Key.DateCreated,
                    User        = new UserView
                    {
                        UserId   = g.Key.UserId,
                        Name     = g.Key.UserName,
                        Email    = g.Key.UserEmail,
                        Role     = g.Key.UserRole,
                        Credit   = g.Key.UserCredit,
                        ImageUrl = g.Key.UserImageUrl
                    },
                    Items = g.Select(i => new OrderItemView
                    {
                        ProductName = i.ProductName,
                        Quantity    = i.Quantity,
                        Price       = i.ItemPrice,
                        ImageUrl    = i.ProductImageUrl
                    }).ToList()
                })
                .ToList();

            return grouped;
        }
    }

    public class OrderFlat
    {
        public int     OrderId          { get; set; }
        public string  DateCreated      { get; set; } = "";
        public int     Quantity         { get; set; }
        public decimal ItemPrice        { get; set; }
        public string  ProductName      { get; set; } = "";
        public string  ProductImageUrl  { get; set; } = "";
    }

    public class OrderGroupView
    {
        public int                 OrderId     { get; set; }
        public string              DateCreated { get; set; } = "";
        public List<OrderItemView> Items       { get; set; } = new();
    }

    public class OrderItemView
    {
        public string  ProductName { get; set; } = "";
        public int     Quantity    { get; set; }
        public decimal Price       { get; set; }
        public string  ImageUrl    { get; set; } = "";
    }

    public class OrderFlatUser
    {
        public int     OrderId          { get; set; }
        public string  DateCreated      { get; set; } = "";
        public int     UserId           { get; set; }
        public string  UserName         { get; set; } = "";
        public string  UserEmail        { get; set; } = "";
        public string  UserRole         { get; set; } = "";
        public decimal UserCredit       { get; set; }
        public string  UserImageUrl     { get; set; } = "";
        public int     Quantity         { get; set; }
        public decimal ItemPrice        { get; set; }
        public string  ProductName      { get; set; } = "";
        public string  ProductImageUrl  { get; set; } = "";
    }

    public class UserView
    {
        public int     UserId    { get; set; }
        public string  Name      { get; set; } = "";
        public string  Email     { get; set; } = "";
        public string  Role      { get; set; } = "";
        public decimal Credit    { get; set; }
        public string  ImageUrl  { get; set; } = "";
    }

    public class OrderWithUserView
    {
        public int                  OrderId     { get; set; }
        public string               DateCreated { get; set; } = "";
        public UserView             User        { get; set; } = new();
        public List<OrderItemView>  Items       { get; set; } = new();
    }
}
