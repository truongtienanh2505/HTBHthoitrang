using Microsoft.AspNetCore.Mvc;
using Shop.Application.Articles;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticlesController : ControllerBase
{
    private readonly ArticleService _service;
    public ArticlesController(ArticleService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return Ok(await _service.GetListAsync());
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetDetail(string slug)
    {
        var data = await _service.GetDetailAsync(slug);
        if (data == null) return NotFound(new { message = "Không tìm thấy bài viết!" });
        return Ok(data);
    }
}