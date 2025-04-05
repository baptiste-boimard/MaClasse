using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.OAuth.Migrations
{
    /// <inheritdoc />
    public partial class AjoutdeZonedansUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Zone",
                table: "UserProfiles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "SessionDatas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Zone",
                table: "UserProfiles");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "SessionDatas",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
