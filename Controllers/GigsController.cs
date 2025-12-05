using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FreelancerPlatform.DTOs;

namespace FreelancerPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GigsController : ControllerBase
    {
        private readonly IMapper _mapper;

        public GigsController(IMapper mapper)
        {
            _mapper = mapper;
        }

        // GET: api/gigs
        [HttpGet]
        public ActionResult<IEnumerable<GigDto>> GetGigs()
        {
            // TODO: In the next step, we will fetch data from the SQL Database.
            // For now, we return an empty list to ensure the code compiles and runs without mock data.

            return Ok(new List<GigDto>());
        }
    }
}