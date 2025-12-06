using AutoMapper;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        // GET: api/messages/conversations
        [HttpGet("conversations")]
        public async Task<ActionResult<IEnumerable<ConversationDto>>> GetMyConversations()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var conversations = await _messageRepository.GetConversationsForUserAsync(userId);
            var dto = _mapper.Map<IEnumerable<ConversationDto>>(conversations);
            return Ok(dto);
        }

        // GET: api/messages/{conversationId}
        [HttpGet("{conversationId:int}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(int conversationId)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var messages = await _messageRepository.GetMessagesForConversationAsync(conversationId, userId);
            var dto = _mapper.Map<IEnumerable<MessageDto>>(messages);
            return Ok(dto);
        }

        // POST: api/messages
        [HttpPost]
        public async Task<ActionResult<MessageDto>> SendMessage(CreateMessageDto createDto)
        {
            var senderId = int.Parse(User.FindFirst("userId")!.Value);

            if (senderId == createDto.ReceiverId)
                return BadRequest("You cannot send a message to yourself.");

            var conversation = await _messageRepository.GetOrCreateConversationAsync(senderId, createDto.ReceiverId);

            var message = new Entities.Message
            {
                ConversationId = conversation!.Id,
                SenderId = senderId,
                Content = createDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _messageRepository.AddMessage(message);
            conversation.LastMessageAt = DateTime.UtcNow;

            if (await _messageRepository.SaveChangesAsync())
            {
                var dto = _mapper.Map<MessageDto>(message);
                return Ok(dto);
            }

            return BadRequest("Failed to send message.");
        }
    }
}
