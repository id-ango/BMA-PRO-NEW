using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.DbContextPersediaanMigrations
{
    /// <inheritdoc />
    public partial class AddItemGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAutoCode",
                table: "IcItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ItemGroupId",
                table: "IcItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartNumber",
                table: "IcItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SearchTags",
                table: "IcItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupplierCode",
                table: "IcItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemGroup",
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
                    table.PrimaryKey("PK_ItemGroup", x => x.ItemGroupId);
                    table.ForeignKey(
                        name: "FK_ItemGroup_ItemGroup_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ItemGroup",
                        principalColumn: "ItemGroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IcItems_ItemGroupId",
                table: "IcItems",
                column: "ItemGroupId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IcItems_ItemGroup_ItemGroupId",
                table: "IcItems");

            migrationBuilder.DropTable(
                name: "ItemGroup");

            migrationBuilder.DropIndex(
                name: "IX_IcItems_ItemGroupId",
                table: "IcItems");

            migrationBuilder.DropColumn(
                name: "IsAutoCode",
                table: "IcItems");

            migrationBuilder.DropColumn(
                name: "ItemGroupId",
                table: "IcItems");

            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "IcItems");

            migrationBuilder.DropColumn(
                name: "SearchTags",
                table: "IcItems");

            migrationBuilder.DropColumn(
                name: "SupplierCode",
                table: "IcItems");
        }
    }
}
