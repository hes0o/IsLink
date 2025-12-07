using AutoMapper;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Entities.Enums;
using FreelancerPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreelancerPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All the action needs JWT
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGigRepository _gigRepository;
        private readonly IMapper _mapper;

        public OrdersController(
            IOrderRepository orderRepository,
            IGigRepository gigRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _gigRepository = gigRepository;
            _mapper = mapper;
        }

        // GET: api/orders/{id}
        // Get a single order (Client أو Freelancer أو Admin فقط)
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var currentUserId = int.Parse(userIdClaim);

            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            // Only Client, Freelancer on this order, or Admin
            if (!User.IsInRole("Admin") &&
                order.ClientId != currentUserId &&
                order.FreelancerId != currentUserId)
            {
                return Forbid();
            }

            var dto = _mapper.Map<OrderDto>(order);
            return Ok(dto);
        }

        // GET: api/orders/client
        // Get all orders where current user is Client (Buyer)
        [HttpGet("client")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyClientOrders()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var clientId = int.Parse(userIdClaim);

            var orders = await _orderRepository.GetOrdersForClientAsync(clientId);
            var dto = _mapper.Map<IEnumerable<OrderDto>>(orders);

            return Ok(dto);
        }

        // GET: api/orders/freelancer
        // Get all orders where current user is Freelancer (Seller)
        [HttpGet("freelancer")]
        [Authorize(Roles = "Freelancer,Admin")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyFreelancerOrders()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var freelancerId = int.Parse(userIdClaim);

            var orders = await _orderRepository.GetOrdersForFreelancerAsync(freelancerId);
            var dto = _mapper.Map<IEnumerable<OrderDto>>(orders);

            return Ok(dto);
        }

        // POST: api/orders
        // Create a new Order – only Client can buy
        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createDto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var clientId = int.Parse(userIdClaim);

            // 1. Get Gig (with Packages)
            var gig = await _gigRepository.GetGigByIdAsync(createDto.GigId);
            if (gig == null)
                return NotFound("Gig not found.");

            // Optional: Only Active gigs can be ordered
            if (gig.Status != GigStatus.Active)
                return BadRequest("This gig is not active.");

            // Client cannot buy his own gig
            if (gig.FreelancerId == clientId)
                return BadRequest("You cannot buy your own gig.");

            // 2. Find chosen package
            var package = gig.Packages.FirstOrDefault(p => p.Id == createDto.PackageId);
            if (package == null)
                return BadRequest("Invalid package for this gig.");

            // 3. Create Order entity
            var now = DateTime.UtcNow;

            var order = new Order
            {
                ClientId = clientId,
                FreelancerId = gig.FreelancerId,
                GigId = gig.Id,
                PackageId = package.Id,
                Price = package.Price,
                Status = OrderStatus.Pending,
                CreatedAt = now,
                DueDate = now.AddDays(package.DeliveryTimeDays)
              // NoteToFreelancer from DTO is not currently available in the Entity
            };

            _orderRepository.AddOrder(order);

            if (await _orderRepository.SaveChangesAsync())
            {
                var dto = _mapper.Map<OrderDto>(order);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, dto);
            }

            return BadRequest("Failed to create order.");
        }

        // PATCH: api/orders/{id}/status
        // Update the status of an order with basic business rules
        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatus newStatus)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var currentUserId = int.Parse(userIdClaim);

            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            var isAdmin = User.IsInRole("Admin");
            var isClient = order.ClientId == currentUserId;
            var isFreelancer = order.FreelancerId == currentUserId;

            if (!isAdmin && !isClient && !isFreelancer)
                return Forbid();

           // Basic transition rules (you can modify them later as needed)
            switch (newStatus)
            {
                case OrderStatus.InProgress:
                    if (!isFreelancer || order.Status != OrderStatus.Pending)
                        return BadRequest("Only the freelancer can move from Pending to InProgress.");
                    break;

                case OrderStatus.Delivered:
                    if (!isFreelancer || order.Status != OrderStatus.InProgress)
                        return BadRequest("Only the freelancer can deliver an InProgress order.");
                    break;

                case OrderStatus.Completed:
                    if (!isClient || order.Status != OrderStatus.Delivered)
                        return BadRequest("Only the client can complete a Delivered order.");
                    break;

                case OrderStatus.Cancelled:
                    if (!isClient || (order.Status != OrderStatus.Pending && order.Status != OrderStatus.InProgress))
                        return BadRequest("Only the client can cancel a Pending or InProgress order.");
                    break;

                case OrderStatus.Disputed:
                    // You could later create a custom API to open Disputes
                    return BadRequest("Use a dedicated endpoint to open a dispute.");
            }

            order.Status = newStatus;

            if (await _orderRepository.SaveChangesAsync())
                return Ok();

            return BadRequest("Failed to update order status.");
        }
    }
}
