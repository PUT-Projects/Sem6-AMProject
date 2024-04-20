using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatterAPI.Migrations
{
    /// <inheritdoc />
    public partial class Sender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sender",
                table: "AwaitingMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sender",
                table: "AwaitingMessages");
        }
    }
}
