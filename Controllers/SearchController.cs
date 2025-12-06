using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Repositories.Interfaces;

namespace FreelancerPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchRepository _searchRepository;
        private readonly IMapper _mapper;

        public SearchController(ISearchRepository searchRepository, IMapper mapper)
        {
            _searchRepository = searchRepository;
            _mapper = mapper;
        }

        // GET: api/search/gigs?query=design&minPrice=50&pageNumber=1
        [HttpGet("gigs")]
        public async Task<ActionResult<PagedResultDto<GigSearchResultDto>>> SearchGigs(
            [FromQuery] GigSearchQueryDto queryDto)
        {
            var (items, totalCount) = await _searchRepository.SearchGigsAsync(
                queryDto.Query,
                queryDto.CategoryId,
                queryDto.MinPrice,
                queryDto.MaxPrice,
                queryDto.SortBy,
                queryDto.PageNumber,
                queryDto.PageSize);

            var mappedItems = _mapper.Map<List<GigSearchResultDto>>(items);

            var result = new PagedResultDto<GigSearchResultDto>
            {
                PageNumber = queryDto.PageNumber,
                PageSize = queryDto.PageSize,
                TotalCount = totalCount,
                Items = mappedItems
            };

            return Ok(result);
        }
    }
}