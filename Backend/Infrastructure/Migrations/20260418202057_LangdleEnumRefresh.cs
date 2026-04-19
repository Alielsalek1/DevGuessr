using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevGuessr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LangdleEnumRefresh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "execution_model",
                table: "langdle_targets");

            migrationBuilder.DropColumn(
                name: "memory_management",
                table: "langdle_targets");

            migrationBuilder.DropColumn(
                name: "type_strength",
                table: "langdle_targets");

            migrationBuilder.DropColumn(
                name: "typing_discipline",
                table: "langdle_targets");

            migrationBuilder.AddColumn<int>(
                name: "memory",
                table: "langdle_targets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "scope_syntax",
                table: "langdle_targets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "semicolons",
                table: "langdle_targets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "type_checking",
                table: "langdle_targets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "memory",
                table: "langdle_targets");

            migrationBuilder.DropColumn(
                name: "scope_syntax",
                table: "langdle_targets");

            migrationBuilder.DropColumn(
                name: "semicolons",
                table: "langdle_targets");

            migrationBuilder.DropColumn(
                name: "type_checking",
                table: "langdle_targets");

            migrationBuilder.AddColumn<int>(
                name: "execution_model",
                table: "langdle_targets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "memory_management",
                table: "langdle_targets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "type_strength",
                table: "langdle_targets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "typing_discipline",
                table: "langdle_targets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
