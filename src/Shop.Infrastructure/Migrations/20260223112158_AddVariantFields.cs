using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GiaBienThe",
                table: "BienTheSanPham",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "KichThuoc",
                table: "BienTheSanPham",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MauSac",
                table: "BienTheSanPham",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiaBienThe",
                table: "BienTheSanPham");

            migrationBuilder.DropColumn(
                name: "KichThuoc",
                table: "BienTheSanPham");

            migrationBuilder.DropColumn(
                name: "MauSac",
                table: "BienTheSanPham");
        }
    }
}
