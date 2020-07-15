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
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password_Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password_Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password_Salt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name_LastName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Name_FirstName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Picture_Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Picture_Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Picture_Height = table.Column<int>(type: "int", nullable: true),
                    Picture_Width = table.Column<int>(type: "int", nullable: true),
                    Picture_Extension = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
