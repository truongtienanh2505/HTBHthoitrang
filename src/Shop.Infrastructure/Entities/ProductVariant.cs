namespace Shop.Infrastructure.Entities
{
    public class ProductVariant
    {
        public int Id { get; set; }

        public int MaSanPham { get; set; }
        public int MaMauSac { get; set; }
        public int MaKichCo { get; set; }

        public string SKU { get; set; } = null!;

        public int SoLuongTon { get; set; }

        public decimal DieuChinhGia { get; set; }
    }
}
