using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Xpandables.Samples.Infrastructure.Migrations.XpandablesLog
{
    public partial class InitLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultLogEntity",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                    Exception = table.Column<string>(nullable: false),
                    Level = table.Column<string>(maxLength: 128, nullable: false),
                    Message = table.Column<string>(nullable: false),
                    MessageTemplate = table.Column<string>(nullable: false),
                    TimeSpan = table.Column<DateTimeOffset>(nullable: false),
                    Properties = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultLogEntity", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultLogEntity");
        }
    }
}
