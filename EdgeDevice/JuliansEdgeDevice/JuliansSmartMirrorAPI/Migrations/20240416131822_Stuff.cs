using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMirror.Migrations
{
    /// <inheritdoc />
    public partial class Stuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Motion",
                table: "SensorData",
                newName: "LightLevel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LightLevel",
                table: "SensorData",
                newName: "Motion");
        }
    }
}
