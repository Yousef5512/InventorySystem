using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Dal.Migrations
{
    /// <inheritdoc />
    public partial class supp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplierUserId",
                table: "ProductSends",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSends_SupplierUserId",
                table: "ProductSends",
                column: "SupplierUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSends_AspNetUsers_SupplierUserId",
                table: "ProductSends",
                column: "SupplierUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSends_AspNetUsers_SupplierUserId",
                table: "ProductSends");

            migrationBuilder.DropIndex(
                name: "IX_ProductSends_SupplierUserId",
                table: "ProductSends");

            migrationBuilder.DropColumn(
                name: "SupplierUserId",
                table: "ProductSends");
        }
    }
}
