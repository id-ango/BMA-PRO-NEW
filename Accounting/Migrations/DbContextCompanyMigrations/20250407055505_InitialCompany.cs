using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.DbContextCompanyMigrations
{
    public partial class InitialCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoSetups",
                columns: table => new
                {
                    CoSetupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoKode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    CoName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account6 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoSetups", x => x.CoSetupId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoSetups");
        }
    }
}
