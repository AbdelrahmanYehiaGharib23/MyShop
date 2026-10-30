using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.DAL.Presistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckoutRequestId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckoutRequestId",
                table: "Orders",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CheckoutRequestId",
                table: "Orders",
                column: "CheckoutRequestId",
                unique: true,
                filter: "[CheckoutRequestId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_CheckoutRequestId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CheckoutRequestId",
                table: "Orders");
        }
    }
}
