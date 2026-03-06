using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixProductVariantAndDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_SanPham_MaSanPham",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamKhuyenMai_SanPham_MaSanPham",
                schema: "dbo",
                table: "SanPhamKhuyenMai");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SanPham",
                table: "SanPham");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SanPham");

            migrationBuilder.AlterColumn<int>(
                name: "MaSanPham",
                table: "SanPham",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SanPham",
                table: "SanPham",
                column: "MaSanPham");

            migrationBuilder.CreateTable(
                name: "SanPhams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VariantId = table.Column<int>(type: "int", nullable: false),
                    TenSanPham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaGoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhams", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_SanPham_MaSanPham",
                table: "ProductVariants",
                column: "MaSanPham",
                principalTable: "SanPham",
                principalColumn: "MaSanPham",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamKhuyenMai_SanPham_MaSanPham",
                schema: "dbo",
                table: "SanPhamKhuyenMai",
                column: "MaSanPham",
                principalTable: "SanPham",
                principalColumn: "MaSanPham",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_SanPham_MaSanPham",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamKhuyenMai_SanPham_MaSanPham",
                schema: "dbo",
                table: "SanPhamKhuyenMai");

            migrationBuilder.DropTable(
                name: "SanPhams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SanPham",
                table: "SanPham");

            migrationBuilder.AlterColumn<int>(
                name: "MaSanPham",
                table: "SanPham",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SanPham",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SanPham",
                table: "SanPham",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_SanPham_MaSanPham",
                table: "ProductVariants",
                column: "MaSanPham",
                principalTable: "SanPham",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamKhuyenMai_SanPham_MaSanPham",
                schema: "dbo",
                table: "SanPhamKhuyenMai",
                column: "MaSanPham",
                principalTable: "SanPham",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
