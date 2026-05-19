using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.DbContextPiutangMigrations
{
    public partial class InitialPiutang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArAccts",
                columns: table => new
                {
                    ArAcctId = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_ArAccts", x => x.ArAcctId);
                });

            migrationBuilder.CreateTable(
                name: "ArCusts",
                columns: table => new
                {
                    ArCustId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaCust = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Golongan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alamat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kota = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Provinsi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlmtKrm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KotaKrm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProvKirim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telpon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NPWP_Cust = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_ArCusts", x => x.ArCustId);
                });

            migrationBuilder.CreateTable(
                name: "ArDists",
                columns: table => new
                {
                    ArDistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dist1 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArDists", x => x.ArDistId);
                });

            migrationBuilder.CreateTable(
                name: "ArPiutngs",
                columns: table => new
                {
                    ArPiutngId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dokumen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KodeTran = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LPB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salesman = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArPiutngs", x => x.ArPiutngId);
                });

            migrationBuilder.CreateTable(
                name: "ArTransHs",
                columns: table => new
                {
                    ArTransHId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Bukti = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KdBank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    Piutang = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Pajak = table.Column<bool>(type: "bit", nullable: false),
                    Unapplied = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AcctSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArCustId = table.Column<int>(type: "int", nullable: false),
                    NamaCust = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salesman = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArTransHs", x => x.ArTransHId);
                });

            migrationBuilder.CreateTable(
                name: "ArTransDs",
                columns: table => new
                {
                    ArTransDId = table.Column<int>(type: "int", nullable: false)
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
                    ArTransHId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArTransDs", x => x.ArTransDId);
                    table.ForeignKey(
                        name: "FK_ArTransDs_ArTransHs_ArTransHId",
                        column: x => x.ArTransHId,
                        principalTable: "ArTransHs",
                        principalColumn: "ArTransHId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArTransDs_ArTransHId",
                table: "ArTransDs",
                column: "ArTransHId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArAccts");

            migrationBuilder.DropTable(
                name: "ArCusts");

            migrationBuilder.DropTable(
                name: "ArDists");

            migrationBuilder.DropTable(
                name: "ArPiutngs");

            migrationBuilder.DropTable(
                name: "ArTransDs");

            migrationBuilder.DropTable(
                name: "ArTransHs");
        }
    }
}
