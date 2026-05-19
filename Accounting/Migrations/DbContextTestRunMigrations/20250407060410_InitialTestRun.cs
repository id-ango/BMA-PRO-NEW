using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.DbContextTestRunMigrations
{
    public partial class InitialTestRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TsSchedules",
                columns: table => new
                {
                    TsScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TsOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dokumen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaCustomer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Daftar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Daerah = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TglKirim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TglTest = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasilTest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PInvoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tested = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TsSchedules", x => x.TsScheduleId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TsSchedules");
        }
    }
}
