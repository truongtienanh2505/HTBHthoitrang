namespace Shop.Infrastructure.Products
{
    public class SanPham
    {
        public int Id { get; set; }
        public int VariantId { get; set; } 

        public string TenSanPham { get; set; }

        public decimal GiaGoc { get; set; }
        public decimal Price { get; set; }
    }
}