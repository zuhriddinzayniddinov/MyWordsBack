using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DatabaseBroker.Migrations
{
    /// <inheritdoc />
    public partial class language_id_null : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "native_language",
                schema: "auth",
                table: "users");

            migrationBuilder.EnsureSchema(
                name: "words");

            migrationBuilder.AddColumn<long>(
                name: "native_language_id",
                schema: "auth",
                table: "users",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "languages",
                schema: "words",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_delete = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_languages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                schema: "words",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    to_language_id = table.Column<long>(type: "bigint", nullable: false),
                    from_language_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    is_delete = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_groups_languages_from_language_id",
                        column: x => x.from_language_id,
                        principalSchema: "words",
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_groups_languages_to_language_id",
                        column: x => x.to_language_id,
                        principalSchema: "words",
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_groups_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "words",
                schema: "words",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "text", nullable: false),
                    translation = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    group_id = table.Column<long>(type: "bigint", nullable: false),
                    is_delete = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_words", x => x.id);
                    table.ForeignKey(
                        name: "FK_words_groups_group_id",
                        column: x => x.group_id,
                        principalSchema: "words",
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_words_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_native_language_id",
                schema: "auth",
                table: "users",
                column: "native_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_from_language_id",
                schema: "words",
                table: "groups",
                column: "from_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_to_language_id",
                schema: "words",
                table: "groups",
                column: "to_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_user_id",
                schema: "words",
                table: "groups",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_words_group_id",
                schema: "words",
                table: "words",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_words_user_id",
                schema: "words",
                table: "words",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_languages_native_language_id",
                schema: "auth",
                table: "users",
                column: "native_language_id",
                principalSchema: "words",
                principalTable: "languages",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_languages_native_language_id",
                schema: "auth",
                table: "users");

            migrationBuilder.DropTable(
                name: "words",
                schema: "words");

            migrationBuilder.DropTable(
                name: "groups",
                schema: "words");

            migrationBuilder.DropTable(
                name: "languages",
                schema: "words");

            migrationBuilder.DropIndex(
                name: "IX_users_native_language_id",
                schema: "auth",
                table: "users");

            migrationBuilder.DropColumn(
                name: "native_language_id",
                schema: "auth",
                table: "users");

            migrationBuilder.AddColumn<int>(
                name: "native_language",
                schema: "auth",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
