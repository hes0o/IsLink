using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Repositories.Interfaces;

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

        // 1. GET: api/gigs
        // Get all gigs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GigDto>>> GetGigs()
        {
            var gigs = await _gigRepository.GetGigsAsync();
            var gigDtos = _mapper.Map<IEnumerable<GigDto>>(gigs);
            return Ok(gigDtos);
        }

        // 2. GET: api/gigs/{id}
        // Get a single gig by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<GigDto>> GetGigById(int id)
        {
            var gig = await _gigRepository.GetGigByIdAsync(id);

            if (gig == null)
                return NotFound();

            var gigDto = _mapper.Map<GigDto>(gig);
            return Ok(gigDto);
        }

        // 3. POST: api/gigs
        // Create a new gig
        [HttpPost]
        public async Task<ActionResult<GigDto>> CreateGig(CreateGigDto createGigDto)
        {
            // Map data from Frontend DTO to Entity
            var gig = _mapper.Map<Gig>(createGigDto);

            // TODO: Temporary solution because Auth system is not ready yet
            // Assume the current user is ID 1
            gig.FreelancerId = 1;

            _gigRepository.AddGig(gig);

            if (await _gigRepository.SaveChangesAsync())
            {
                var gigDto = _mapper.Map<GigDto>(gig);
                return CreatedAtAction(nameof(GetGigById), new { id = gig.Id }, gigDto);
            }

            return BadRequest("Failed to create the gig.");
        }

        // 4. PUT: api/gigs/{id}
        // Update an existing gig
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGig(int id, CreateGigDto updateGigDto)
        {
            var gig = await _gigRepository.GetGigByIdAsync(id);

            if (gig == null)
                return NotFound();

            // AutoMapper updates existing gig values with new values from updateGigDto
            _mapper.Map(updateGigDto, gig);

            if (await _gigRepository.SaveChangesAsync())
            {
                return NoContent(); // 204 Success
            }

            return BadRequest("Failed to update the gig.");
        }

        // 5. DELETE: api/gigs/{id}
        // Delete a gig
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGig(int id)
        {
            var gig = await _gigRepository.GetGigByIdAsync(id);

            if (gig == null)
                return NotFound();

            _gigRepository.DeleteGig(gig);

            if (await _gigRepository.SaveChangesAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to delete the gig.");
        }
    }
}