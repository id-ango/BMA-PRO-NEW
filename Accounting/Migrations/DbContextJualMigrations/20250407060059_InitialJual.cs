using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.DbContextJualMigrations
{
    public partial class InitialJual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OeAltItems",
                columns: table => new
                {
                    OeAltItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salesman = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    Komisi = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNo = table.Column<bool>(type: "bit", nullable: false),
                    CostMethod = table.Column<int>(type: "int", nullable: false),
                    JnsBrng = table.Column<int>(type: "int", nullable: false),
                    StdPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TglPost = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastNetto = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OeAltItems", x => x.OeAltItemId);
                });

            migrationBuilder.CreateTable(
                name: "OeKurirs",
                columns: table => new
                {
                    OeKurirId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kurir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaKurir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Golongan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alamat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kota = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Provinsi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlmtKrm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KotaKrm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProvKirim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telpon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NPWP_Kurir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlmtNPWP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expedisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Termin = table.Column<int>(type: "int", nullable: false),
                    Disc1 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Disc2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kontak = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SldAwal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NonPPN = table.Column<bool>(type: "bit", nullable: false),
                    Pajak = table.Column<bool>(type: "bit", nullable: false),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcctPjk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TglPost = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TglMasuk = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LstOrder = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Piutang = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NamaLengkap = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OeKurirs", x => x.OeKurirId);
                });

            migrationBuilder.CreateTable(
                name: "OeSalesmans",
                columns: table => new
                {
                    OeSalesmanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Salesman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaSales = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Golongan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alamat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kota = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Provinsi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlmtKrm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KotaKrm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProvKirim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telpon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NPWP_Sales = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlmtNPWP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expedisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Termin = table.Column<int>(type: "int", nullable: false),
                    Disc1 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Disc2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kontak = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SldAwal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NonPPN = table.Column<bool>(type: "bit", nullable: false),
                    Pajak = table.Column<bool>(type: "bit", nullable: false),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcctPjk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TglPost = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TglMasuk = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LstOrder = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Piutang = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NamaLengkap = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OeSalesmans", x => x.OeSalesmanId);
                });

            migrationBuilder.CreateTable(
                name: "OeTransHs",
                columns: table => new
                {
                    OeTransHId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    NoLpb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoSJ = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoPrj = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lokasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    Salesman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kurir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaCust = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlamatKirim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pajak = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OeTransHs", x => x.OeTransHId);
                });

            migrationBuilder.CreateTable(
                name: "OeTransDs",
                columns: table => new
                {
                    OeTransDId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    NoLpb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaItem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Satuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lokasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Harga = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Persen = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    QtyBo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HrgCost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    JumDpp = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Jumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OeTransHId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OeTransDs", x => x.OeTransDId);
                    table.ForeignKey(
                        name: "FK_OeTransDs_OeTransHs_OeTransHId",
                        column: x => x.OeTransHId,
                        principalTable: "OeTransHs",
                        principalColumn: "OeTransHId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OeTransDs_OeTransHId",
                table: "OeTransDs",
                column: "OeTransHId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OeAltItems");

            migrationBuilder.DropTable(
                name: "OeKurirs");

            migrationBuilder.DropTable(
                name: "OeSalesmans");

            migrationBuilder.DropTable(
                name: "OeTransDs");

            migrationBuilder.DropTable(
                name: "OeTransHs");
        }
    }
}
