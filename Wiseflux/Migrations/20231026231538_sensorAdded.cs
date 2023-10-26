using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wiseflux.Migrations
{
    /// <inheritdoc />
    public partial class sensorAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    User = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SensorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SensorGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SensorType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => new { x.User, x.SensorId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sensors");
        }
    }
}
