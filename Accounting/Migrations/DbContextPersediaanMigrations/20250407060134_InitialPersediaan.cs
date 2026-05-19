using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.DbContextPersediaanMigrations
{
    public partial class InitialPersediaan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IcAccts",
                columns: table => new
                {
                    IcAcctId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acct1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acct2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acct3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acct4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acct5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acct6 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcAccts", x => x.IcAcctId);
                });

            migrationBuilder.CreateTable(
                name: "IcAltItems",
                columns: table => new
                {
                    IcAltItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lokasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaItem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Satuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Divisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Harga = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    QtyPo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BefNetto = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HrgNetto = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HrgJual = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    SaldoAwal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CostAwal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNo = table.Column<bool>(type: "bit", nullable: false),
                    CostMethod = table.Column<int>(type: "int", nullable: false),
                    JnsBrng = table.Column<int>(type: "int", nullable: false),
                    StdPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TglPost = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastNetto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Disabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcAltItems", x => x.IcAltItemId);
                });

            migrationBuilder.CreateTable(
                name: "IcCats",
                columns: table => new
                {
                    IcCatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cat1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cat2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cat3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cat4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cat5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cat6 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcCats", x => x.IcCatId);
                });

            migrationBuilder.CreateTable(
                name: "IcDivs",
                columns: table => new
                {
                    IcDivId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Divisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaDiv = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcDivs", x => x.IcDivId);
                });

            migrationBuilder.CreateTable(
                name: "IcItems",
                columns: table => new
                {
                    IcItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaItem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Satuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Divisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HrgUsd = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Harga = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MinQty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    QtyPo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BefNetto = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HrgNetto = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HrgJual = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    SaldoAwal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CostAwal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNo = table.Column<bool>(type: "bit", nullable: false),
                    CostMethod = table.Column<int>(type: "int", nullable: false),
                    JnsBrng = table.Column<int>(type: "int", nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StdPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TglPost = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastNetto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NamaLengkap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcItems", x => x.IcItemId);
                });

            migrationBuilder.CreateTable(
                name: "Iclokasis",
                columns: table => new
                {
                    IcLokasiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Lokasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaLokasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alamat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kota = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telpon = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iclokasis", x => x.IcLokasiId);
                });

            migrationBuilder.CreateTable(
                name: "IcStockCards",
                columns: table => new
                {
                    IcStockCardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dokumen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MenuApp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Harga = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Persen = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HrgCost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Lokasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QtyShp = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Qtybo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Jumfps = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    JumDpp = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Jumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcStockCards", x => x.IcStockCardId);
                });

            migrationBuilder.CreateTable(
                name: "IcTransHs",
                columns: table => new
                {
                    IcTransHId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoFaktur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoSj = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lokasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lokasi2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pajak = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcTransHs", x => x.IcTransHId);
                });

            migrationBuilder.CreateTable(
                name: "IcTransDs",
                columns: table => new
                {
                    IcTransDId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoFaktur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoSj = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lokasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lokasi2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaItem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Satuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Harga = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    QtyBo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    QtyShp = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Jumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IcPoTransD = table.Column<int>(type: "int", nullable: false),
                    IcTransHId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcTransDs", x => x.IcTransDId);
                    table.ForeignKey(
                        name: "FK_IcTransDs_IcTransHs_IcTransHId",
                        column: x => x.IcTransHId,
                        principalTable: "IcTransHs",
                        principalColumn: "IcTransHId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IcTransDs_IcTransHId",
                table: "IcTransDs",
                column: "IcTransHId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IcAccts");

            migrationBuilder.DropTable(
                name: "IcAltItems");

            migrationBuilder.DropTable(
                name: "IcCats");

            migrationBuilder.DropTable(
                name: "IcDivs");

            migrationBuilder.DropTable(
                name: "IcItems");

            migrationBuilder.DropTable(
                name: "Iclokasis");

            migrationBuilder.DropTable(
                name: "IcStockCards");

            migrationBuilder.DropTable(
                name: "IcTransDs");

            migrationBuilder.DropTable(
                name: "IcTransHs");
        }
    }
}
