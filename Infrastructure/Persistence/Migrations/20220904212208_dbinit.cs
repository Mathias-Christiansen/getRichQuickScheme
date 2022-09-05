using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class dbinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email_Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Password_Salt = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 32, nullable: false),
                    Password_Hashed = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Secret_Bearer = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 4096, nullable: true),
                    Secret_CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Email",
                table: "Users",
                column: "Email_Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Secret_Bearer",
                table: "Users",
                column: "Secret_Bearer",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
