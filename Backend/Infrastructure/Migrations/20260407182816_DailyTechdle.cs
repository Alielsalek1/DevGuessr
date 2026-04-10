using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevGuessr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DailyDevGuessr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "daily_techdles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    puzzle_date = table.Column<DateOnly>(type: "date", nullable: false),
                    programming_language_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_daily_techdles", x => x.id);
                    table.ForeignKey(
                        name: "fk_daily_techdles_programming_languages_programming_language_id",
                        column: x => x.programming_language_id,
                        principalTable: "programming_languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_daily_techdles_programming_language_id",
                table: "daily_techdles",
                column: "programming_language_id");

            migrationBuilder.CreateIndex(
                name: "ix_daily_techdles_puzzle_date",
                table: "daily_techdles",
                column: "puzzle_date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_techdles");
        }
    }
}
