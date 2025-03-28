using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.OAuth.Migrations
{
    /// <inheritdoc />
    public partial class AjoutdeRoledansUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionData",
                table: "SessionData");

            migrationBuilder.RenameTable(
                name: "SessionData",
                newName: "SessionDatas");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "UserProfiles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionDatas",
                table: "SessionDatas",
                column: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionDatas",
                table: "SessionDatas");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserProfiles");

            migrationBuilder.RenameTable(
                name: "SessionDatas",
                newName: "SessionData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionData",
                table: "SessionData",
                column: "Token");
        }
    }
}
