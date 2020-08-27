using Microsoft.EntityFrameworkCore.Migrations;

namespace Xpandables.Samples.Infrastructure.Migrations.XpandablesLog
{
    public partial class updateLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Properties",
                table: "DefaultLogEntity",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ActionId",
                table: "DefaultLogEntity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionName",
                table: "DefaultLogEntity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ControllerName",
                table: "DefaultLogEntity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventId",
                table: "DefaultLogEntity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "DefaultLogEntity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestPath",
                table: "DefaultLogEntity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceContext",
                table: "DefaultLogEntity",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionId",
                table: "DefaultLogEntity");

            migrationBuilder.DropColumn(
                name: "ActionName",
                table: "DefaultLogEntity");

            migrationBuilder.DropColumn(
                name: "ControllerName",
                table: "DefaultLogEntity");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "DefaultLogEntity");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "DefaultLogEntity");

            migrationBuilder.DropColumn(
                name: "RequestPath",
                table: "DefaultLogEntity");

            migrationBuilder.DropColumn(
                name: "SourceContext",
                table: "DefaultLogEntity");

            migrationBuilder.AlterColumn<string>(
                name: "Properties",
                table: "DefaultLogEntity",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
