using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure.Services;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly CheckoutService _checkoutService;

        public CheckoutController(CheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            try
            {
                var orderId = await _checkoutService.CheckoutAsync(request);

                return Ok(new
                {
                    success = true,
                    orderId = orderId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpPost("validate-cart")]
        public async Task<IActionResult> ValidateCart([FromBody] List<CartItem> cart)
        {
            var result = await _checkoutService.ValidateCart(cart);

            if (result != "Valid")
            {
                return BadRequest(result);
            }

            return Ok("Giỏ hàng hợp lệ");
        }
            }
}