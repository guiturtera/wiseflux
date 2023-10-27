using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wiseflux.Migrations
{
    /// <inheritdoc />
    public partial class addedSensorName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SensorName",
                table: "Sensors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SensorName",
                table: "Sensors");
        }
    }
}
