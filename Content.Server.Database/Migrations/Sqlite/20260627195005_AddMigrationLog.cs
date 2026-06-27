using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
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
                    migration_log_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    source_user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    target_user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    source_user_name = table.Column<string>(type: "TEXT", nullable: false),
                    target_user_name = table.Column<string>(type: "TEXT", nullable: false),
                    time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    automatic = table.Column<bool>(type: "INTEGER", nullable: false),
                    status = table.Column<int>(type: "INTEGER", nullable: false),
                    scope = table.Column<int>(type: "INTEGER", nullable: false),
                    match_reason = table.Column<string>(type: "TEXT", nullable: false),
                    performed_by_user_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    detail = table.Column<string>(type: "TEXT", nullable: true)
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
