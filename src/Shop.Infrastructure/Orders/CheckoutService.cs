using Shop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Shop.Infrastructure.Services;

public class CheckoutService
{
    private readonly ShopDbContext _context;

    public CheckoutService(ShopDbContext context)
    {
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
}