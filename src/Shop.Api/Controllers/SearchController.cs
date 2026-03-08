using Microsoft.AspNetCore.Mvc;
using Shop.Application.Search;
using Shop.Infrastructure.Search;
namespace Shop.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly SearchService _searchService;

    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchRequest request)
    {
        try
        {
            var result = await _searchService.SearchProductsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Lỗi hệ thống: " + ex.Message });
        }
    }
}