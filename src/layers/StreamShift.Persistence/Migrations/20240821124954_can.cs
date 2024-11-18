using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamShift.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class can : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TableName",
                schema: "StreamShift",
                table: "Transfers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TableName",
                schema: "StreamShift",
                table: "Transfers");
        }
    }
}
