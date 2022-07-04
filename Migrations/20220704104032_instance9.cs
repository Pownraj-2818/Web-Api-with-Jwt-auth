using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTAUTH.Migrations
{
    public partial class instance9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "resetToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "resetToken",
                table: "Users");
        }
    }
}
