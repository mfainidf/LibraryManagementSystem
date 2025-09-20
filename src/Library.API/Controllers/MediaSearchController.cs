using Library.Application.Services;
using Library.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/v1/media/search")]
    public class MediaSearchController : ControllerBase
    {
        private readonly SearchService _searchService;

        public MediaSearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SearchCriteria criteria)
        {
            var results = await _searchService.SearchAsync(criteria);
            // Return only the items list for simple integration test consumption
            return Ok(results.Items);
        }
    }
}
