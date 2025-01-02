using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MahantInv.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GujaratiName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GujaratiName",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GujaratiName",
                table: "Products");
        }
    }
}
