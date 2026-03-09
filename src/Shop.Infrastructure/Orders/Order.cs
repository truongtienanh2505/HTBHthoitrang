public class Order
{
    public int Id { get; set; }

    public string CustomerName { get; set; }

    public string Phone { get; set; }

    public string Address { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<OrderItem> Items { get; set; }
}