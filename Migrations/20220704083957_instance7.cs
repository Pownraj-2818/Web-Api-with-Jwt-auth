using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTAUTH.Migrations
{
    public partial class instance7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "verficationCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "verifiedat",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "verficationCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "verifiedat",
                table: "Users");
        }
    }
}
