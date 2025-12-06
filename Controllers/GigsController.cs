using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace FreelancerPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GigsController : ControllerBase
    {
        private readonly IGigRepository _gigRepository;
        private readonly IMapper _mapper;

        public GigsController(IGigRepository gigRepository, IMapper mapper)
        {
            _gigRepository = gigRepository;
            _mapper = mapper;
        }

        // GET: api/gigs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GigDto>>> GetGigs()
        {
            var gigs = await _gigRepository.GetGigsAsync();
            var gigDtos = _mapper.Map<IEnumerable<GigDto>>(gigs);
            return Ok(gigDtos);
        }

        // GET: api/gigs/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GigDto>> GetGigById(int id)
        {
            var gig = await _gigRepository.GetGigByIdAsync(id);

            if (gig == null)
                return NotFound();

            var gigDto = _mapper.Map<GigDto>(gig);
            return Ok(gigDto);
        }

        // POST: api/gigs
        // Create a new Gig – the user must be a Freelancer
        [Authorize(Roles = "Freelancer")]
        [HttpPost]
        public async Task<ActionResult<GigDto>> CreateGig(CreateGigDto createGigDto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var freelancerId = int.Parse(userIdClaim);

            var gig = _mapper.Map<Gig>(createGigDto);
            gig.FreelancerId = freelancerId;

            _gigRepository.AddGig(gig);

            if (await _gigRepository.SaveChangesAsync())
            {
                var gigDto = _mapper.Map<GigDto>(gig);
                return CreatedAtAction(nameof(GetGigById), new { id = gig.Id }, gigDto);
            }

            return BadRequest("Failed to create the gig.");
        }

        // PUT: api/gigs/{id}
        [Authorize(Roles = "Freelancer,Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGig(int id, CreateGigDto updateGigDto)
        {
            var gig = await _gigRepository.GetGigByIdAsync(id);
            if (gig == null)
                return NotFound();

            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var currentUserId = int.Parse(userIdClaim);

            // Freelancers cannot edit Gigs they do not own (Admins are excluded)
            if (!User.IsInRole("Admin") && gig.FreelancerId != currentUserId)
                return Forbid();

            _mapper.Map(updateGigDto, gig);

            if (await _gigRepository.SaveChangesAsync())
                return NoContent();

            return BadRequest("Failed to update the gig.");
        }

        // DELETE: api/gigs/{id}
        [Authorize(Roles = "Freelancer,Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGig(int id)
        {
            var gig = await _gigRepository.GetGigByIdAsync(id);
            if (gig == null)
                return NotFound();

            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var currentUserId = int.Parse(userIdClaim);

            if (!User.IsInRole("Admin") && gig.FreelancerId != currentUserId)
                return Forbid();

            _gigRepository.DeleteGig(gig);

            if (await _gigRepository.SaveChangesAsync())
                return NoContent();

            return BadRequest("Failed to delete the gig.");
        }
    }
}
