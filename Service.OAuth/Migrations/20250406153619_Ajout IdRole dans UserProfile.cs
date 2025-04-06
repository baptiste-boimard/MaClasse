using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.OAuth.Migrations
{
    /// <inheritdoc />
    public partial class AjoutIdRoledansUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdRole",
                table: "UserProfiles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Rattachments",
                columns: table => new
                {
                    IdGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    IdDirecteur = table.Column<string>(type: "text", nullable: false),
                    IdProfesseur = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rattachments", x => x.IdGuid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rattachments");

            migrationBuilder.DropColumn(
                name: "IdRole",
                table: "UserProfiles");
        }
    }
}
