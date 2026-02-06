using Microsoft.AspNetCore.Mvc;
using Shop.Application.Categories;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly DanhMucService _svc;

    public CategoriesController(DanhMucService svc) => _svc = svc;

    [HttpGet]
    public Task<List<DanhMucDto>> GetAll([FromQuery] bool includeInactive = false, CancellationToken ct = default)
        => _svc.GetAllAsync(includeInactive, ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DanhMucDto>> GetById(int id, CancellationToken ct)
    {
        var item = await _svc.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateDanhMucRequest req, CancellationToken ct)
    {
        var id = await _svc.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateDanhMucRequest req, CancellationToken ct)
    {
        var ok = await _svc.UpdateAsync(id, req, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _svc.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("tree")]
    public Task<List<DanhMucTreeNodeDto>> Tree(CancellationToken ct)
        => _svc.GetTreeAsync(ct);
}
