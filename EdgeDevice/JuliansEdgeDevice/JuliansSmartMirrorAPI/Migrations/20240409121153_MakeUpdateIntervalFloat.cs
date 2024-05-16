using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMirror.Migrations
{
    /// <inheritdoc />
    public partial class MakeUpdateIntervalFloat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "UpdateInterval",
                table: "ConditionalRule",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.UpdateData(
                table: "ConditionalRule",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdateInterval",
                value: 1f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UpdateInterval",
                table: "ConditionalRule",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.UpdateData(
                table: "ConditionalRule",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdateInterval",
                value: 1);
        }
    }
}
