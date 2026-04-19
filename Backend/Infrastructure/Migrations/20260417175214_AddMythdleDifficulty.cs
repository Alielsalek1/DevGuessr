using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevGuessr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMythdleDifficulty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "difficulty",
                table: "mythdle_targets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "difficulty",
                table: "mythdle_targets");
        }
    }
}
