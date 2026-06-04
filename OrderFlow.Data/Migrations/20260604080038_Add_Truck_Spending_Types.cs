using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Truck_Spending_Types : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpendingType",
                table: "TrucksSpendings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpendingType",
                table: "TrucksSpendings");
        }
    }
}
