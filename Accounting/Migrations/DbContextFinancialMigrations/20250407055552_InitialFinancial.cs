using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.DbContextFinancialMigrations
{
    public partial class InitialFinancial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FcAccounts",
                columns: table => new
                {
                    FcAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FcTahun = table.Column<int>(type: "int", nullable: false),
                    FcComKode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlAcct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlNama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlDept = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipeGl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlTipe = table.Column<int>(type: "int", nullable: false),
                    GlSaldo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPost = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GlSldAwal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlKurs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlFisc1 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlFisc2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlFisc3 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlFisc4 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlFisc5 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlFisc6 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlFisc7 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlFisc8 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlFisc9 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlFisc10 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlFisc11 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlFisc12 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPreFisc1 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPreFisc2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPreFisc3 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPreFisc4 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPreFisc5 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPreFisc6 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPreFisc7 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPreFisc8 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPreFisc9 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPreFisc10 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPreFisc11 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GlPreFisc12 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NamaLengkap = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcAccounts", x => x.FcAccountId);
                });

            migrationBuilder.CreateTable(
                name: "FcComs",
                columns: table => new
                {
                    FcComId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FcComKode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FcNamaPerusahaan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FcAlamat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlAcct1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlAcct2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlAcct3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlAcct4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlAcct5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlAcct6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlDept1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlDept2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlDept3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlDept4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlDept5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlDept6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FcFiscalYear = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcComs", x => x.FcComId);
                });

            migrationBuilder.CreateTable(
                name: "FcGlTransHs",
                columns: table => new
                {
                    FcGlTransHId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FcComKode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GlMemo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KodeGl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Debet = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kredit = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kurs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NonPPn = table.Column<bool>(type: "bit", nullable: false),
                    Cek = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcGlTransHs", x => x.FcGlTransHId);
                });

            migrationBuilder.CreateTable(
                name: "FcPrintGls",
                columns: table => new
                {
                    FcPrintGlId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FcComKode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KodeCetak = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoBaris = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoRek1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoRek2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CetakDetil = table.Column<bool>(type: "bit", nullable: false),
                    CetakGaris1 = table.Column<bool>(type: "bit", nullable: false),
                    CetakGaris2 = table.Column<bool>(type: "bit", nullable: false),
                    CetakBln1 = table.Column<bool>(type: "bit", nullable: false),
                    CetakBln2 = table.Column<bool>(type: "bit", nullable: false),
                    Spasi = table.Column<int>(type: "int", nullable: false),
                    CetakTebal = table.Column<bool>(type: "bit", nullable: false),
                    CetakNegatif = table.Column<bool>(type: "bit", nullable: false),
                    CetakHide = table.Column<bool>(type: "bit", nullable: false),
                    RumusBaris = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Persen1 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Persen2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Qty1 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Qty2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    JumTran1 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    JumTran2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    JumRekap1 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    JumRekap2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    JumSaldo = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcPrintGls", x => x.FcPrintGlId);
                });

            migrationBuilder.CreateTable(
                name: "FcPrintTPs",
                columns: table => new
                {
                    FcPrintTPId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KodeCetak = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaCetak = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JnsReport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FcComKode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcPrintTPs", x => x.FcPrintTPId);
                });

            migrationBuilder.CreateTable(
                name: "FcTransHs",
                columns: table => new
                {
                    FcTransHId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FcComKode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GlMemo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KodeGl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Debet = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kredit = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kurs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NonPPn = table.Column<bool>(type: "bit", nullable: false),
                    Cek = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcTransHs", x => x.FcTransHId);
                });

            migrationBuilder.CreateTable(
                name: "FcGlTransDs",
                columns: table => new
                {
                    FcGlTransDId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FcComKode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlAcct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlDept = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Debet = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kredit = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Jumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kurs = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    NomKurs = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    JumKurs = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NonPPn = table.Column<bool>(type: "bit", nullable: false),
                    FcGlTransHId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcGlTransDs", x => x.FcGlTransDId);
                    table.ForeignKey(
                        name: "FK_FcGlTransDs_FcGlTransHs_FcGlTransHId",
                        column: x => x.FcGlTransHId,
                        principalTable: "FcGlTransHs",
                        principalColumn: "FcGlTransHId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FcTransDs",
                columns: table => new
                {
                    FcTransDId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FcComKode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlAcct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlDept = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Debet = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kredit = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Jumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kurs = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    NomKurs = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    JumKurs = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NonPPn = table.Column<bool>(type: "bit", nullable: false),
                    FcTransHId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcTransDs", x => x.FcTransDId);
                    table.ForeignKey(
                        name: "FK_FcTransDs_FcTransHs_FcTransHId",
                        column: x => x.FcTransHId,
                        principalTable: "FcTransHs",
                        principalColumn: "FcTransHId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FcGlTransDs_FcGlTransHId",
                table: "FcGlTransDs",
                column: "FcGlTransHId");

            migrationBuilder.CreateIndex(
                name: "IX_FcTransDs_FcTransHId",
                table: "FcTransDs",
                column: "FcTransHId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FcAccounts");

            migrationBuilder.DropTable(
                name: "FcComs");

            migrationBuilder.DropTable(
                name: "FcGlTransDs");

            migrationBuilder.DropTable(
                name: "FcPrintGls");

            migrationBuilder.DropTable(
                name: "FcPrintTPs");

            migrationBuilder.DropTable(
                name: "FcTransDs");

            migrationBuilder.DropTable(
                name: "FcGlTransHs");

            migrationBuilder.DropTable(
                name: "FcTransHs");
        }
    }
}
