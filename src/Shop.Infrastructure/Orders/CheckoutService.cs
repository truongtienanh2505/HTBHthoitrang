using Shop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Models;
using Shop.Application.Products;
namespace Shop.Infrastructure.Services;

public class CheckoutService
{
    private readonly ShopDbContext _context;
    private readonly IProductRepository _productRepository;  
    public CheckoutService(
        IProductRepository productRepository,
        ShopDbContext context)
    {
        _productRepository = productRepository;
        _context = context;
    }

    public async Task<int> CheckoutAsync(CheckoutRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            decimal total = 0;

            var order = new Order
            {
                CustomerName = request.CustomerName,
                Phone = request.Phone,
                Address = request.Address
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in request.Items)
            {
                
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(x => x.ProductVariantId == item.ProductVariantId);

                if (variant == null)
                    throw new Exception("Product not found");

                if (variant.Stock < item.Quantity)
                    throw new Exception("Not enough stock");

                variant.Stock -= item.Quantity;

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductVariantId = item.ProductVariantId,
                    Quantity = item.Quantity,
                    Price = variant.Price
                };

                total += variant.Price * item.Quantity;

                _context.OrderItems.Add(orderItem);
            }

            order.TotalAmount = total;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return order.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
      public async Task<string> ValidateCart(List<CartItem> cartItems)
        {
            if (cartItems == null || cartItems.Count == 0)
            {
                return "Giỏ hàng trống";
            }

            foreach (var item in cartItems)
            {
                var variant = await _productRepository.GetProductVariantById(item.ProductVariantId);

                if (variant == null)
                {
                    return $"Sản phẩm {item.ProductVariantId} không tồn tại";
                }

                if (!variant.HoatDong)
                {
                    return "Sản phẩm hiện không còn bán";
                }

                if (item.Quantity <= 0)
                {
                    return "Số lượng không hợp lệ";
                }

                if (item.Quantity > variant.SoLuongTon)
                {
                    return $"Sản phẩm {variant.Sku} không đủ tồn kho";
                }
            }

            return "Valid";
        }
}