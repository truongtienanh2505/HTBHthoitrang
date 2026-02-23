using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialProductDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "DanhMuc",
                schema: "dbo",
                columns: table => new
                {
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaDanhMucCha = table.Column<int>(type: "int", nullable: true),
                    HoatDong = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TaoLuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CapNhatLuc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMuc", x => x.MaDanhMuc);
                    table.ForeignKey(
                        name: "FK_DanhMuc_DanhMuc_MaDanhMucCha",
                        column: x => x.MaDanhMucCha,
                        principalSchema: "dbo",
                        principalTable: "DanhMuc",
                        principalColumn: "MaDanhMuc");
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    MaSanPham = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSanPham = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaGoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoatDong = table.Column<bool>(type: "bit", nullable: false),
                    TaoLuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CapNhatLuc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPham", x => x.MaSanPham);
                    table.ForeignKey(
                        name: "FK_SanPham_DanhMuc_MaDanhMuc",
                        column: x => x.MaDanhMuc,
                        principalSchema: "dbo",
                        principalTable: "DanhMuc",
                        principalColumn: "MaDanhMuc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BienTheSanPham",
                columns: table => new
                {
                    MaBienThe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSanPham = table.Column<int>(type: "int", nullable: false),
                    MaMauSac = table.Column<int>(type: "int", nullable: false),
                    MaKichCo = table.Column<int>(type: "int", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    DieuChinhGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HoatDong = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BienTheSanPham", x => x.MaBienThe);
                    table.ForeignKey(
                        name: "FK_BienTheSanPham_SanPham_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "SanPham",
                        principalColumn: "MaSanPham",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BienTheSanPham_MaSanPham",
                table: "BienTheSanPham",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMuc_MaDanhMucCha",
                schema: "dbo",
                table: "DanhMuc",
                column: "MaDanhMucCha");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMuc_Slug",
                schema: "dbo",
                table: "DanhMuc",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_MaDanhMuc",
                table: "SanPham",
                column: "MaDanhMuc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BienTheSanPham");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "DanhMuc",
                schema: "dbo");
        }
    }
}
