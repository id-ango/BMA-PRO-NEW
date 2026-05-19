using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.DbContextOrderMigrations
{
    public partial class InitialOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoTransHs",
                columns: table => new
                {
                    PoTransHId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    NoLpb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoSJ = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoPrj = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lokasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kurs = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JthTempo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TtlJumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    DPayment = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Ongkos = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PpnPersen = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Ppn = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Jumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Tagihan = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalQty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    QtyTerima = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Nilai = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Vendor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaVendor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pajak = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoTransHs", x => x.PoTransHId);
                });

            migrationBuilder.CreateTable(
                name: "PoTransDs",
                columns: table => new
                {
                    PoTransDId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    NoLpb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaItem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Satuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lokasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kurs = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Harga = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Persen = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    QtyBo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    JumDpp = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Jumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Vendor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PoTransHId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoTransDs", x => x.PoTransDId);
                    table.ForeignKey(
                        name: "FK_PoTransDs_PoTransHs_PoTransHId",
                        column: x => x.PoTransHId,
                        principalTable: "PoTransHs",
                        principalColumn: "PoTransHId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PoTransDs_PoTransHId",
                table: "PoTransDs",
                column: "PoTransHId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoTransDs");

            migrationBuilder.DropTable(
                name: "PoTransHs");
        }
    }
}
