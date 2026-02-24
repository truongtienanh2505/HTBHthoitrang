using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SKU",
                table: "BienTheSanPham",
                newName: "Sku");

            migrationBuilder.CreateTable(
                name: "KhuyenMai",
                schema: "dbo",
                columns: table => new
                {
                    MaKhuyenMai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhuyenMai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiGiamGia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GiaTriGiam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GiamToiDa = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KichHoat = table.Column<bool>(type: "bit", nullable: false),
                    UuTien = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuyenMai", x => x.MaKhuyenMai);
                });

            migrationBuilder.CreateTable(
                name: "DieuKienKhuyenMai",
                schema: "dbo",
                columns: table => new
                {
                    Ma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhuyenMai = table.Column<int>(type: "int", nullable: false),
                    TruongDuLieu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToanTu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaTri = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DieuKienKhuyenMai", x => x.Ma);
                    table.ForeignKey(
                        name: "FK_DieuKienKhuyenMai_KhuyenMai_MaKhuyenMai",
                        column: x => x.MaKhuyenMai,
                        principalSchema: "dbo",
                        principalTable: "KhuyenMai",
                        principalColumn: "MaKhuyenMai",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SanPhamKhuyenMai",
                schema: "dbo",
                columns: table => new
                {
                    Ma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSanPham = table.Column<int>(type: "int", nullable: false),
                    MaKhuyenMai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhamKhuyenMai", x => x.Ma);
                    table.ForeignKey(
                        name: "FK_SanPhamKhuyenMai_KhuyenMai_MaKhuyenMai",
                        column: x => x.MaKhuyenMai,
                        principalSchema: "dbo",
                        principalTable: "KhuyenMai",
                        principalColumn: "MaKhuyenMai",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SanPhamKhuyenMai_SanPham_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "SanPham",
                        principalColumn: "MaSanPham",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DieuKienKhuyenMai_MaKhuyenMai",
                schema: "dbo",
                table: "DieuKienKhuyenMai",
                column: "MaKhuyenMai");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamKhuyenMai_MaKhuyenMai",
                schema: "dbo",
                table: "SanPhamKhuyenMai",
                column: "MaKhuyenMai");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamKhuyenMai_MaSanPham",
                schema: "dbo",
                table: "SanPhamKhuyenMai",
                column: "MaSanPham");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DieuKienKhuyenMai",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SanPhamKhuyenMai",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "KhuyenMai",
                schema: "dbo");

            migrationBuilder.RenameColumn(
                name: "Sku",
                table: "BienTheSanPham",
                newName: "SKU");
        }
    }
}
