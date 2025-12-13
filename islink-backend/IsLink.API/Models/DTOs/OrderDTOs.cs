using System.ComponentModel.DataAnnotations;

namespace IsLink.API.Models.DTOs;

// ============================================
// Request DTOs
// ============================================

public class CreateOrderRequest
{
    [Required]
    public Guid GigId { get; set; }

    [Required]
    public string PackageType { get; set; } = string.Empty; // basic, standard, premium

    public string? Requirements { get; set; }
}

public class UpdateOrderStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
}

// ============================================
// Response DTOs
// ============================================

public class OrderResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public OrderDto? Data { get; set; }
}

public class OrderListResponse
{
    public bool Success { get; set; }
    public List<OrderDto> Data { get; set; } = new();
}

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PackageType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Requirements { get; set; }
    public DateTime? DeliveryDeadline { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderGigDto? Gig { get; set; }
    public OrderUserDto? Buyer { get; set; }
    public OrderUserDto? Seller { get; set; }
}

public class OrderGigDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Image { get; set; }
}

public class OrderUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
}

