using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Reviews;
using System.Security.Claims;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewService _service;

    public ReviewsController(ReviewService service)
    {
        _service = service;
    }

    // Lấy danh sách đánh giá của 1 sản phẩm (Ai cũng xem được)
    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetReviews(int productId)
    {
        var list = await _service.GetReviewsByProductAsync(productId);
        return Ok(list);
    }

    // Viết đánh giá mới (BẮT BUỘC PHẢI ĐĂNG NHẬP BẰNG TOKEN)
    [HttpPost]
    [Authorize] 
    public async Task<IActionResult> SubmitReview([FromBody] ReviewDto dto)
    {
        // Lấy UserID từ JWT Token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        int userId = int.Parse(userIdClaim.Value);

        var result = await _service.SubmitReviewAsync(userId, dto);
        
        if (!result.Success) return BadRequest(new { message = result.Message, success = false });
        return Ok(new { message = result.Message, success = true });
    }
}