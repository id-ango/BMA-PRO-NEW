using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.DbContextPersediaanMigrations
{
    /// <inheritdoc />
    public partial class RenameIcItemGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IcItems_ItemGroup_ItemGroupId",
                table: "IcItems");

            migrationBuilder.DropTable(
                name: "ItemGroup");

            migrationBuilder.CreateTable(
                name: "IcItemGroup",
                columns: table => new
                {
                    ItemGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcItemGroup", x => x.ItemGroupId);
                    table.ForeignKey(
                        name: "FK_IcItemGroup_IcItemGroup_ParentId",
                        column: x => x.ParentId,
                        principalTable: "IcItemGroup",
                        principalColumn: "ItemGroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IcItemGroup_ParentId_Code",
                table: "IcItemGroup",
                columns: new[] { "ParentId", "Code" },
                unique: true,
                filter: "[ParentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_IcItems_IcItemGroup_ItemGroupId",
                table: "IcItems",
                column: "ItemGroupId",
                principalTable: "IcItemGroup",
                principalColumn: "ItemGroupId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IcItems_IcItemGroup_ItemGroupId",
                table: "IcItems");

            migrationBuilder.DropTable(
                name: "IcItemGroup");

            migrationBuilder.CreateTable(
                name: "ItemGroup",
                columns: table => new
                {
                    ItemGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemGroup", x => x.ItemGroupId);
                    table.ForeignKey(
                        name: "FK_ItemGroup_ItemGroup_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ItemGroup",
                        principalColumn: "ItemGroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroup_ParentId_Code",
                table: "ItemGroup",
                columns: new[] { "ParentId", "Code" },
                unique: true,
                filter: "[ParentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_IcItems_ItemGroup_ItemGroupId",
                table: "IcItems",
                column: "ItemGroupId",
                principalTable: "ItemGroup",
                principalColumn: "ItemGroupId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
