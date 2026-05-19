using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.DbContextBankMigrations
{
    public partial class InitialBank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CbBanks",
                columns: table => new
                {
                    CbBankId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KodeBank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NmBank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Kurs = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Acctset = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    SldAwal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    KSldAwal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ClrDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    KSaldo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NonPpn = table.Column<bool>(type: "bit", nullable: false),
                    Pajak = table.Column<bool>(type: "bit", nullable: false),
                    GrpBank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoKode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CbBanks", x => x.CbBankId);
                });

            migrationBuilder.CreateTable(
                name: "CbGrps",
                columns: table => new
                {
                    CbGrpId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grp = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    NamaGrp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CbGrps", x => x.CbGrpId);
                });

            migrationBuilder.CreateTable(
                name: "CbSrcCodes",
                columns: table => new
                {
                    CbSrcCodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SrcCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaSrc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlAcct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Grp = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CbSrcCodes", x => x.CbSrcCodeId);
                });

            migrationBuilder.CreateTable(
                name: "CbTransfers",
                columns: table => new
                {
                    CbTransferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KodeBank1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KodeBank2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Refno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Acctset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Giro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kurs = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Kurs2 = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    KValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    KSaldo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Biaya = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    KBiaya = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NonPPN = table.Column<bool>(type: "bit", nullable: false),
                    Pajak = table.Column<bool>(type: "bit", nullable: false),
                    Cek = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CbTransfers", x => x.CbTransferId);
                });

            migrationBuilder.CreateTable(
                name: "CbTransHs",
                columns: table => new
                {
                    CbTransHId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KodeBank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Refno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Acctset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Giro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kurs = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    NonPPN = table.Column<bool>(type: "bit", nullable: false),
                    Pajak = table.Column<bool>(type: "bit", nullable: false),
                    Cek = table.Column<bool>(type: "bit", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    KSaldo = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CbTransHs", x => x.CbTransHId);
                });

            migrationBuilder.CreateTable(
                name: "CbTransDs",
                columns: table => new
                {
                    CbTransDId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoPrj = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SrcCodeId = table.Column<int>(type: "int", nullable: false),
                    SrcCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlAcct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Jumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Terima = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Bayar = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Kurs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    KJumlah = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    KTerima = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    KBayar = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CbTransHId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CbTransDs", x => x.CbTransDId);
                    table.ForeignKey(
                        name: "FK_CbTransDs_CbTransHs_CbTransHId",
                        column: x => x.CbTransHId,
                        principalTable: "CbTransHs",
                        principalColumn: "CbTransHId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CbTransDs_CbTransHId",
                table: "CbTransDs",
                column: "CbTransHId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CbBanks");

            migrationBuilder.DropTable(
                name: "CbGrps");

            migrationBuilder.DropTable(
                name: "CbSrcCodes");

            migrationBuilder.DropTable(
                name: "CbTransDs");

            migrationBuilder.DropTable(
                name: "CbTransfers");

            migrationBuilder.DropTable(
                name: "CbTransHs");
        }
    }
}
