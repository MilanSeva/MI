using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MahantInv.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MFA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthenticatorKey",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMfaEnabled",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthenticatorKey",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsMfaEnabled",
                table: "AspNetUsers");
        }
    }
}
