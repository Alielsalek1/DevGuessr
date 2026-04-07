using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Techdle.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DailyLogodle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "daily_logodles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    puzzle_date = table.Column<DateOnly>(type: "date", nullable: false),
                    logodle_target_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_daily_logodles", x => x.id);
                    table.ForeignKey(
                        name: "fk_daily_logodles_logodle_targets_logodle_target_id",
                        column: x => x.logodle_target_id,
                        principalTable: "logodle_targets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_daily_logodles_logodle_target_id",
                table: "daily_logodles",
                column: "logodle_target_id");

            migrationBuilder.CreateIndex(
                name: "ix_daily_logodles_puzzle_date",
                table: "daily_logodles",
                column: "puzzle_date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_logodles");
        }
    }
}
