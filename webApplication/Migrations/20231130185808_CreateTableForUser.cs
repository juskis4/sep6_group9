using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webApplication.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users",
                newSchema: "public");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                schema: "public",
                table: "users",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                schema: "public",
                table: "users");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "public",
                newName: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "user_id");
        }
    }
}
