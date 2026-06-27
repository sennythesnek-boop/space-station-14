using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class AddMigrationLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "migration_log",
                columns: table => new
                {
                    migration_log_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    source_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_user_name = table.Column<string>(type: "text", nullable: false),
                    target_user_name = table.Column<string>(type: "text", nullable: false),
                    time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    automatic = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    scope = table.Column<int>(type: "integer", nullable: false),
                    match_reason = table.Column<string>(type: "text", nullable: false),
                    performed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    detail = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_migration_log", x => x.migration_log_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_migration_log_source_user_id",
                table: "migration_log",
                column: "source_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_migration_log_target_user_id",
                table: "migration_log",
                column: "target_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_migration_log_time",
                table: "migration_log",
                column: "time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "migration_log");
        }
    }
}
