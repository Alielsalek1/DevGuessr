using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Techdle.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class logodle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "logodle_targets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    image_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    blurred_image_urls = table.Column<List<string>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_logodle_targets", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_logodle_targets_name",
                table: "logodle_targets",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "logodle_targets");
        }
    }
}
