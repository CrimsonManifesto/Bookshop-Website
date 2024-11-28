using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookshop_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStockQuantityToBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Books");
        }
    }
}
