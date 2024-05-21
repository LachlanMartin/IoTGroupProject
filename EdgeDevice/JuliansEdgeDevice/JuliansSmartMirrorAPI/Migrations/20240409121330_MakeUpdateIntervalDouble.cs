using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMirror.Migrations
{
    /// <inheritdoc />
    public partial class MakeUpdateIntervalDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConditionalRule",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdateInterval",
                value: 1.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConditionalRule",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdateInterval",
                value: 1f);
        }
    }
}
