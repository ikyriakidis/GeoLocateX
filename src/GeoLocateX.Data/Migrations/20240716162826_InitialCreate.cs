using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoLocateX.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BatchProcess",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchProcess", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BatchProcessItemResponse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(39)", maxLength: 39, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timezones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchProcessItemResponse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BatchProcessItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(39)", maxLength: 39, nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchProcessItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchProcessItem_BatchProcess_BatchProcessId",
                        column: x => x.BatchProcessId,
                        principalTable: "BatchProcess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchProcessItem_BatchProcessId",
                table: "BatchProcessItem",
                column: "BatchProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchProcessItemResponse_IpAddress",
                table: "BatchProcessItemResponse",
                column: "IpAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchProcessItem");

            migrationBuilder.DropTable(
                name: "BatchProcessItemResponse");

            migrationBuilder.DropTable(
                name: "BatchProcess");
        }
    }
}
