using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.DAL.Presistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderFulfillment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourierName",
                table: "Orders",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EstimatedDeliveryDate",
                table: "Orders",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FulfillmentStatus",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "Orders",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourierName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EstimatedDeliveryDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FulfillmentStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "Orders");
        }
    }
}
