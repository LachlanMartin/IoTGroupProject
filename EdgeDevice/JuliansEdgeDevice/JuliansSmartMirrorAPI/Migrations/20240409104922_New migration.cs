using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMirror.Migrations
{
    /// <inheritdoc />
    public partial class Newmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ConditionalRule",
                columns: new[] { "Id", "MotionEnabled", "TemperatureThreshold", "UpdateInterval" },
                values: new object[] { 1, true, 25f, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConditionalRule",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
