using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevGuessr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clusterdle_targets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    difficulty_level = table.Column<int>(type: "integer", nullable: false),
                    words = table.Column<List<string>>(type: "jsonb", nullable: false),
                    success_message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clusterdle_targets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "langdle_targets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    year_first_appeared = table.Column<int>(type: "integer", nullable: false),
                    typing_discipline = table.Column<int>(type: "integer", nullable: false),
                    type_strength = table.Column<int>(type: "integer", nullable: false),
                    execution_model = table.Column<int>(type: "integer", nullable: false),
                    memory_management = table.Column<int>(type: "integer", nullable: false),
                    tags = table.Column<List<string>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_langdle_targets", x => x.id);
                    table.UniqueConstraint("ak_langdle_targets_name", x => x.name);
                });

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
                    table.UniqueConstraint("ak_logodle_targets_name", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    google_id = table.Column<string>(type: "text", nullable: true),
                    username = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    is_email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, defaultValue: ""),
                    phone_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "daily_langdles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    puzzle_date = table.Column<DateOnly>(type: "date", nullable: false),
                    langdle_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_daily_langdles", x => x.id);
                    table.ForeignKey(
                        name: "fk_daily_langdles_langdle_targets_langdle_name",
                        column: x => x.langdle_name,
                        principalTable: "langdle_targets",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "daily_logodles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    puzzle_date = table.Column<DateOnly>(type: "date", nullable: false),
                    logodle_target_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_daily_logodles", x => x.id);
                    table.ForeignKey(
                        name: "fk_daily_logodles_logodle_targets_logodle_target_name",
                        column: x => x.logodle_target_name,
                        principalTable: "logodle_targets",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_devices",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_devices", x => new { x.user_id, x.device_id });
                    table.ForeignKey(
                        name: "fk_user_devices_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_refresh_tokens",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    refresh_token_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    refresh_token_expiry_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_refresh_tokens", x => new { x.user_id, x.refresh_token_hash });
                    table.ForeignKey(
                        name: "fk_user_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_clusterdle_targets_group_name",
                table: "clusterdle_targets",
                column: "group_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_daily_langdles_langdle_name",
                table: "daily_langdles",
                column: "langdle_name");

            migrationBuilder.CreateIndex(
                name: "ix_daily_langdles_puzzle_date",
                table: "daily_langdles",
                column: "puzzle_date");

            migrationBuilder.CreateIndex(
                name: "ix_daily_logodles_logodle_target_name",
                table: "daily_logodles",
                column: "logodle_target_name");

            migrationBuilder.CreateIndex(
                name: "ix_daily_logodles_puzzle_date",
                table: "daily_logodles",
                column: "puzzle_date");

            migrationBuilder.CreateIndex(
                name: "ix_langdle_targets_name",
                table: "langdle_targets",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_logodle_targets_name",
                table: "logodle_targets",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_google_id",
                table: "users",
                column: "google_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clusterdle_targets");

            migrationBuilder.DropTable(
                name: "daily_langdles");

            migrationBuilder.DropTable(
                name: "daily_logodles");

            migrationBuilder.DropTable(
                name: "user_devices");

            migrationBuilder.DropTable(
                name: "user_refresh_tokens");

            migrationBuilder.DropTable(
                name: "langdle_targets");

            migrationBuilder.DropTable(
                name: "logodle_targets");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
