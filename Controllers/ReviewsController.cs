using AutoMapper;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Entities.Enums;
using FreelancerPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerPlatform.Controllers
{
    [ApiController]
    [Route("api/gigs/{gigId:int}/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public ReviewsController(
            IReviewRepository reviewRepository,
            IOrderRepository orderRepository,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        // GET: api/gigs/{gigId}/reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsForGig(int gigId)
        {
            var reviews = await _reviewRepository.GetReviewsForGigAsync(gigId);
            var dto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            return Ok(dto);
        }

        // POST: api/gigs/{gigId}/reviews/{orderId}
        [Authorize(Roles = "Client")]
        [HttpPost("{orderId:int}")]
        public async Task<ActionResult> CreateReview(int gigId, int orderId, CreateReviewDto createDto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var clientId = int.Parse(userIdClaim);

            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null || order.GigId != gigId || order.ClientId != clientId)
                return BadRequest("Invalid order.");

            if (order.Status != OrderStatus.Completed)
                return BadRequest("Order is not completed yet.");

            if (await _reviewRepository.HasReviewAsync(orderId))
                return BadRequest("Review already exists for this order.");

            var review = new Review
            {
                OrderId = orderId,
                Rating = createDto.Rating,
                Comment = createDto.Comment
            };

            _reviewRepository.AddReview(review);

            if (await _reviewRepository.SaveChangesAsync())
                return Ok();

            return BadRequest("Failed to add review.");
        }
    }
}
