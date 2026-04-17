using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevGuessr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DailyMythdleEngine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "ak_mythdle_targets_name",
                table: "mythdle_targets",
                column: "name");

            migrationBuilder.CreateTable(
                name: "daily_mythdles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    puzzle_date = table.Column<DateOnly>(type: "date", nullable: false),
                    mythdle_target_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_daily_mythdles", x => x.id);
                    table.ForeignKey(
                        name: "fk_daily_mythdles_mythdle_targets_mythdle_target_name",
                        column: x => x.mythdle_target_name,
                        principalTable: "mythdle_targets",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_daily_mythdles_mythdle_target_name",
                table: "daily_mythdles",
                column: "mythdle_target_name");

            migrationBuilder.CreateIndex(
                name: "ix_daily_mythdles_puzzle_date",
                table: "daily_mythdles",
                column: "puzzle_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_mythdles");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_mythdle_targets_name",
                table: "mythdle_targets");
        }
    }
}
