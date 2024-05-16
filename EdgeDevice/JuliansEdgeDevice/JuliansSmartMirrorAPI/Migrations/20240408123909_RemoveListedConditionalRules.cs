using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMirror.Migrations
{
    /// <inheritdoc />
    public partial class RemoveListedConditionalRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ConditionalRules",
                table: "ConditionalRules");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ConditionalRules");

            migrationBuilder.RenameTable(
                name: "ConditionalRules",
                newName: "ConditionalRule");

            migrationBuilder.AddColumn<int>(
                name: "UpdateInterval",
                table: "ConditionalRule",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConditionalRule",
                table: "ConditionalRule",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ConditionalRule",
                table: "ConditionalRule");

            migrationBuilder.DropColumn(
                name: "UpdateInterval",
                table: "ConditionalRule");

            migrationBuilder.RenameTable(
                name: "ConditionalRule",
                newName: "ConditionalRules");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ConditionalRules",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConditionalRules",
                table: "ConditionalRules",
                column: "Id");
        }
    }
}
