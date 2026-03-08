using Microsoft.AspNetCore.Mvc;
using Shop.Application.Banners;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/banners")]
public class BannersController : ControllerBase
{
    private readonly BannerService _service;
    public BannersController(BannerService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetBanners() => Ok(await _service.GetBannersAsync());
}