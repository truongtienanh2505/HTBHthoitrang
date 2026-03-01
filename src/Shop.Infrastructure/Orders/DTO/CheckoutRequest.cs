public class CheckoutRequest
{
    public string CustomerName { get; set; }

    public string Phone { get; set; }

    public string Address { get; set; }

    public List<CartItem> Items { get; set; }
}

public class CartItem
{
    public int ProductVariantId { get; set; }

    public int Quantity { get; set; }
}