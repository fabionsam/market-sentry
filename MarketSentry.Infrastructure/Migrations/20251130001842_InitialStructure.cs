using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketSentry.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                    BaseUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ApiToken = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockPriceHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPriceHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    PriceSell = table.Column<decimal>(type: "TEXT", nullable: false),
                    PriceBuy = table.Column<decimal>(type: "TEXT", nullable: false),
                    EmailNotification = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApiId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockConfigs_ApiConfigurations_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiConfigurations_ProviderName",
                table: "ApiConfigurations",
                column: "ProviderName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockConfigs_ApiId",
                table: "StockConfigs",
                column: "ApiId");

            migrationBuilder.CreateIndex(
                name: "IX_StockConfigs_Symbol",
                table: "StockConfigs",
                column: "Symbol",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockConfigs");

            migrationBuilder.DropTable(
                name: "StockPriceHistories");

            migrationBuilder.DropTable(
                name: "ApiConfigurations");
        }
    }
}
