using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace webApplication.Migrations
{
    /// <inheritdoc />
    public partial class Initialv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "movies",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    title = table.Column<string>(type: "text", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "people",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    birth = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_people", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                schema: "public",
                columns: table => new
                {
                    movie_id = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<float>(type: "real", nullable: false),
                    votes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => x.movie_id);
                    table.ForeignKey(
                        name: "FK_ratings_movies_movie_id",
                        column: x => x.movie_id,
                        principalSchema: "public",
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "directors",
                schema: "public",
                columns: table => new
                {
                    movie_id = table.Column<int>(type: "integer", nullable: false),
                    person_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_directors", x => new { x.movie_id, x.person_id });
                    table.ForeignKey(
                        name: "FK_directors_movies_movie_id",
                        column: x => x.movie_id,
                        principalSchema: "public",
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_directors_people_person_id",
                        column: x => x.person_id,
                        principalSchema: "public",
                        principalTable: "people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stars",
                schema: "public",
                columns: table => new
                {
                    movie_id = table.Column<int>(type: "integer", nullable: false),
                    person_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stars", x => new { x.movie_id, x.person_id });
                    table.ForeignKey(
                        name: "FK_stars_movies_movie_id",
                        column: x => x.movie_id,
                        principalSchema: "public",
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stars_people_person_id",
                        column: x => x.person_id,
                        principalSchema: "public",
                        principalTable: "people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_movie_list",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    movie_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<char>(type: "character(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_movie_list", x => new { x.user_id, x.movie_id });
                    table.ForeignKey(
                        name: "FK_user_movie_list_movies_movie_id",
                        column: x => x.movie_id,
                        principalSchema: "public",
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_movie_list_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_directors_person_id",
                schema: "public",
                table: "directors",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_stars_person_id",
                schema: "public",
                table: "stars",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_movie_list_movie_id",
                schema: "public",
                table: "user_movie_list",
                column: "movie_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "directors",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ratings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "stars",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_movie_list",
                schema: "public");

            migrationBuilder.DropTable(
                name: "people",
                schema: "public");

            migrationBuilder.DropTable(
                name: "movies",
                schema: "public");

            migrationBuilder.DropTable(
                name: "users",
                schema: "public");
        }
    }
}
