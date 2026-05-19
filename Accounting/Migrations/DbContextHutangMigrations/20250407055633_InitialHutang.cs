using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.DbContextHutangMigrations
{
    public partial class InitialHutang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApAccts",
                columns: table => new
                {
                    ApAcctId = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_ApAccts", x => x.ApAcctId);
                });

            migrationBuilder.CreateTable(
                name: "ApDists",
                columns: table => new
                {
                    ApDistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dist1 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApDists", x => x.ApDistId);
                });

            migrationBuilder.CreateTable(
                name: "ApHutangs",
                columns: table => new
                {
                    ApHutangId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dokumen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KodeTran = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LPB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pajak = table.Column<bool>(type: "bit", nullable: false),
                    Jumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Bayar = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Sisa = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UnApplied = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Dpp = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PPn = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PPh = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    SldSisa = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    SldBayar = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    SldDisc = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    SldUnpl = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kurs = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Nilai = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApHutangs", x => x.ApHutangId);
                });

            migrationBuilder.CreateTable(
                name: "ApSuppls",
                columns: table => new
                {
                    ApSupplId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaSup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Person = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alamat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kota = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Provinsi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telpon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NPWP_Sup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlmtNPWP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kurs = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Termin = table.Column<int>(type: "int", nullable: false),
                    Disc1 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Disc2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kontak = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SldAwal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Pajak = table.Column<bool>(type: "bit", nullable: false),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcctPjk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TglPost = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TglMasuk = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LstOrder = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Hutang = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NamaLengkap = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApSuppls", x => x.ApSupplId);
                });

            migrationBuilder.CreateTable(
                name: "ApTransHs",
                columns: table => new
                {
                    ApTransHId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Bukti = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KdBank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoFaktur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JthTempo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PPn = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PPh = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    JumPPh = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    JumPPn = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Bruto = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Netto = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Jumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Hutang = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Pajak = table.Column<bool>(type: "bit", nullable: false),
                    Unapplied = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kurs = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Nilai = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApSupplId = table.Column<int>(type: "int", nullable: false),
                    NamaSup = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApTransHs", x => x.ApTransHId);
                });

            migrationBuilder.CreateTable(
                name: "ApTransDs",
                columns: table => new
                {
                    ApTransDId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bukti = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Kode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    KodeTran = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Lpb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Jumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Bayar = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Sisa = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApTransHId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApTransDs", x => x.ApTransDId);
                    table.ForeignKey(
                        name: "FK_ApTransDs_ApTransHs_ApTransHId",
                        column: x => x.ApTransHId,
                        principalTable: "ApTransHs",
                        principalColumn: "ApTransHId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApTransDs_ApTransHId",
                table: "ApTransDs",
                column: "ApTransHId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApAccts");

            migrationBuilder.DropTable(
                name: "ApDists");

            migrationBuilder.DropTable(
                name: "ApHutangs");

            migrationBuilder.DropTable(
                name: "ApSuppls");

            migrationBuilder.DropTable(
                name: "ApTransDs");

            migrationBuilder.DropTable(
                name: "ApTransHs");
        }
    }
}
