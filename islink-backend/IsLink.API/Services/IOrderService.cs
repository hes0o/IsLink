using IsLink.API.Models.DTOs;

namespace IsLink.API.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(Guid buyerId, CreateOrderRequest request);
    Task<OrderListResponse> GetOrdersAsync(Guid userId, string role, string? status, int limit, int offset);
    Task<OrderResponse> GetOrderByIdAsync(Guid userId, Guid orderId);
    Task<OrderResponse> UpdateOrderStatusAsync(Guid userId, Guid orderId, string status);
}

