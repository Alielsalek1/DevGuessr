using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevGuessr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Mythdle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mythdle_targets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_fake = table.Column<bool>(type: "boolean", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mythdle_targets", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_mythdle_targets_name",
                table: "mythdle_targets",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mythdle_targets");
        }
    }
}
