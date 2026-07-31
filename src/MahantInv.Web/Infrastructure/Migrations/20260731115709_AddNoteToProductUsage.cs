using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MahantInv.Web.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNoteToProductUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ProductUsages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "Notifications",
                type: "REAL",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DefaultFilters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    FilterData = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFilters", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultFilters");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "ProductUsages");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Notifications");
        }
    }
}
