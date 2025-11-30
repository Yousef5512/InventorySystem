using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Dal.Migrations
{
    /// <inheritdoc />
    public partial class suppl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSends_AspNetUsers_SupplierUserId",
                table: "ProductSends");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSends_Suppliers_SupplierId",
                table: "ProductSends");

            migrationBuilder.DropIndex(
                name: "IX_ProductSends_SupplierUserId",
                table: "ProductSends");

            migrationBuilder.DropColumn(
                name: "SupplierUserId",
                table: "ProductSends");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierId",
                table: "ProductSends",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSends_Suppliers_SupplierId",
                table: "ProductSends",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSends_Suppliers_SupplierId",
                table: "ProductSends");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierId",
                table: "ProductSends",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSends_Suppliers_SupplierId",
                table: "ProductSends",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }
    }
}
