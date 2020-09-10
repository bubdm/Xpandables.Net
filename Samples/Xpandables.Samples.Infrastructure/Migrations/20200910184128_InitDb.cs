using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Xpandables.Samples.Infrastructure.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    Password_Key = table.Column<string>(nullable: true),
                    Password_Value = table.Column<string>(nullable: true),
                    Password_Salt = table.Column<string>(nullable: true),
                    Picture_Title = table.Column<string>(nullable: true),
                    Picture_Content = table.Column<byte[]>(nullable: true),
                    Picture_Height = table.Column<int>(nullable: true),
                    Picture_Width = table.Column<int>(nullable: true),
                    Picture_Extension = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                table: "Users",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
