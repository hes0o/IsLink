using IsLink.API.Data;
using IsLink.API.Models.DTOs;
using IsLink.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IsLink.API.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public OrderService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<OrderResponse> CreateOrderAsync(Guid buyerId, CreateOrderRequest request)
    {
        var gig = await _context.Gigs
            .Include(g => g.Packages)
            .Include(g => g.Seller)
            .FirstOrDefaultAsync(g => g.Id == request.GigId && g.IsActive);

        if (gig == null)
        {
            return new OrderResponse { Success = false, Message = "Gig not found" };
        }

        if (gig.SellerId == buyerId)
        {
            return new OrderResponse { Success = false, Message = "You cannot order your own gig" };
        }

        var package = gig.Packages.FirstOrDefault(p => p.PackageType == request.PackageType);
        if (package == null)
        {
            return new OrderResponse { Success = false, Message = "Package not found" };
        }

        var serviceFee = package.Price * 0.055m;
        var totalPrice = package.Price + serviceFee;
        var deliveryDeadline = DateTime.UtcNow.AddDays(package.DeliveryDays);

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            GigId = gig.Id,
            BuyerId = buyerId,
            SellerId = gig.SellerId,
            PackageType = request.PackageType,
            Price = package.Price,
            ServiceFee = serviceFee,
            TotalPrice = totalPrice,
            Requirements = request.Requirements,
            DeliveryDeadline = deliveryDeadline,
            Status = "pending"
        };

        _context.Orders.Add(order);
        
        gig.OrdersInQueue++;
        
        await _context.SaveChangesAsync();

        // Send notification
        await _notificationService.CreateNotificationAsync(
            gig.SellerId.ToString(),
            "order_placed",
            "New Order Received! 🎉",
            $"You have a new order for \"{gig.Title}\"",
            new Dictionary<string, object> { { "orderId", order.Id }, { "orderNumber", order.OrderNumber } }
        );

        return new OrderResponse
        {
            Success = true,
            Message = "Order created successfully",
            Data = new OrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                TotalPrice = order.TotalPrice,
                DeliveryDeadline = order.DeliveryDeadline
            }
        };
    }

    public async Task<OrderListResponse> GetOrdersAsync(Guid userId, string role, string? status, int limit, int offset)
    {
        var query = _context.Orders
            .Include(o => o.Gig).ThenInclude(g => g!.Images)
            .Include(o => o.Buyer)
            .Include(o => o.Seller)
            .AsQueryable();

        query = role == "seller" 
            ? query.Where(o => o.SellerId == userId)
            : query.Where(o => o.BuyerId == userId);

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(o => o.Status == status);
        }

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        return new OrderListResponse
        {
            Success = true,
            Data = orders.Select(MapToOrderDto).ToList()
        };
    }

    public async Task<OrderResponse> GetOrderByIdAsync(Guid userId, Guid orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Gig).ThenInclude(g => g!.Images)
            .Include(o => o.Buyer)
            .Include(o => o.Seller)
            .Include(o => o.Deliveries).ThenInclude(d => d.Attachments)
            .FirstOrDefaultAsync(o => o.Id == orderId && (o.BuyerId == userId || o.SellerId == userId));

        if (order == null)
        {
            return new OrderResponse { Success = false, Message = "Order not found" };
        }

        return new OrderResponse
        {
            Success = true,
            Data = MapToOrderDto(order)
        };
    }

    public async Task<OrderResponse> UpdateOrderStatusAsync(Guid userId, Guid orderId, string status)
    {
        var order = await _context.Orders
            .Include(o => o.Gig)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return new OrderResponse { Success = false, Message = "Order not found" };
        }

        // Validate status transition
        var validTransitions = new Dictionary<string, string[]>
        {
            { "pending", new[] { "in_progress", "cancelled" } },
            { "in_progress", new[] { "delivered", "cancelled" } },
            { "delivered", new[] { "completed", "revision_requested" } },
            { "revision_requested", new[] { "in_progress" } }
        };

        if (!validTransitions.ContainsKey(order.Status) || !validTransitions[order.Status].Contains(status))
        {
            return new OrderResponse { Success = false, Message = $"Cannot change status from {order.Status} to {status}" };
        }

        order.Status = status;
        if (status == "delivered") order.DeliveredAt = DateTime.UtcNow;
        if (status == "completed")
        {
            order.CompletedAt = DateTime.UtcNow;
            if (order.Gig != null) order.Gig.OrdersInQueue = Math.Max(0, order.Gig.OrdersInQueue - 1);
            
            var seller = await _context.Users.FindAsync(order.SellerId);
            if (seller != null) seller.CompletedOrders++;
        }

        await _context.SaveChangesAsync();

        // Send notification
        var notifyUserId = userId == order.BuyerId ? order.SellerId : order.BuyerId;
        if (notifyUserId.HasValue)
        {
            await _notificationService.CreateNotificationAsync(
                notifyUserId.Value.ToString(),
                $"order_{status}",
                $"Order {status.Replace("_", " ")}",
                $"Order #{order.OrderNumber} has been {status.Replace("_", " ")}",
                new Dictionary<string, object> { { "orderId", order.Id } }
            );
        }

        return new OrderResponse
        {
            Success = true,
            Message = "Order status updated",
            Data = new OrderDto { Status = status }
        };
    }

    private static string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.Ticks.ToString("x").ToUpper();
        var random = Guid.NewGuid().ToString("N")[..4].ToUpper();
        return $"IS-{timestamp[..6]}-{random}";
    }

    private static OrderDto MapToOrderDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            PackageType = order.PackageType,
            Price = order.Price,
            ServiceFee = order.ServiceFee,
            TotalPrice = order.TotalPrice,
            Requirements = order.Requirements,
            DeliveryDeadline = order.DeliveryDeadline,
            DeliveredAt = order.DeliveredAt,
            CompletedAt = order.CompletedAt,
            CreatedAt = order.CreatedAt,
            Gig = order.Gig == null ? null : new OrderGigDto
            {
                Id = order.Gig.Id,
                Title = order.Gig.Title,
                Slug = order.Gig.Slug,
                Image = order.Gig.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl ?? order.Gig.Images.FirstOrDefault()?.ImageUrl
            },
            Buyer = order.Buyer == null ? null : new OrderUserDto
            {
                Id = order.Buyer.Id,
                Username = order.Buyer.Username,
                FullName = order.Buyer.FullName,
                Avatar = order.Buyer.AvatarUrl
            },
            Seller = order.Seller == null ? null : new OrderUserDto
            {
                Id = order.Seller.Id,
                Username = order.Seller.Username,
                FullName = order.Seller.FullName,
                Avatar = order.Seller.AvatarUrl
            }
        };
    }
}

