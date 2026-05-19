using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations
{
    public partial class InitialAssets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AsAcctsets",
                columns: table => new
                {
                    AsAcctId = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_AsAcctsets", x => x.AsAcctId);
                });

            migrationBuilder.CreateTable(
                name: "AsAssetss",
                columns: table => new
                {
                    AsAssetsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarcodeAssets = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AsItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Merek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaBarang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoMesin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoRangka = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoPol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Nilai = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PPn = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Asuransi = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Bunga = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Administrasi = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Provisi = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Termin = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Penyusutan = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    SisaNilai = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TglBeli = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JatuhTempo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NilaiTerjual = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TglJual = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Acctset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsAssetss", x => x.AsAssetsId);
                });

            migrationBuilder.CreateTable(
                name: "AsDistSets",
                columns: table => new
                {
                    AsDistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dist1 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsDistSets", x => x.AsDistId);
                });

            migrationBuilder.CreateTable(
                name: "AsDivisis",
                columns: table => new
                {
                    AsDivId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Divisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaDiv = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsDivisis", x => x.AsDivId);
                });

            migrationBuilder.CreateTable(
                name: "AsItems",
                columns: table => new
                {
                    AsItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaItem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Satuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Divisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsItems", x => x.AsItemId);
                });

            migrationBuilder.CreateTable(
                name: "AsTransaksis",
                columns: table => new
                {
                    AsTransaksiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BarcodeAssets = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nilai = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Cek = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsTransaksis", x => x.AsTransaksiId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsAcctsets");

            migrationBuilder.DropTable(
                name: "AsAssetss");

            migrationBuilder.DropTable(
                name: "AsDistSets");

            migrationBuilder.DropTable(
                name: "AsDivisis");

            migrationBuilder.DropTable(
                name: "AsItems");

            migrationBuilder.DropTable(
                name: "AsTransaksis");
        }
    }
}
