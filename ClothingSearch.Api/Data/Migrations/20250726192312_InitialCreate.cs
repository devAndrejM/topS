using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClothingSearch.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Query = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ResultCount = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchAnalytics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    ProviderType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Config = table.Column<string>(type: "text", nullable: false, defaultValue: "{}"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stores_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    ClothingSize = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ShoeSize = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ShoeSizeSystem = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserSettings_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SearchCaches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SearchQuery = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StoreId = table.Column<int>(type: "integer", nullable: false),
                    Results = table.Column<string>(type: "text", nullable: false, defaultValue: "[]"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchCaches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchCaches_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Created", "Currency", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 26, 19, 23, 10, 968, DateTimeKind.Utc).AddTicks(2339), "EUR", "Croatia" },
                    { 2, new DateTime(2025, 7, 26, 19, 23, 10, 968, DateTimeKind.Utc).AddTicks(2481), "EUR", "Germany" },
                    { 3, new DateTime(2025, 7, 26, 19, 23, 10, 968, DateTimeKind.Utc).AddTicks(2482), "USD", "United States" },
                    { 4, new DateTime(2025, 7, 26, 19, 23, 10, 968, DateTimeKind.Utc).AddTicks(2483), "GBP", "United Kingdom" }
                });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "Id", "Config", "CountryId", "Created", "IsActive", "Name", "ProviderType" },
                values: new object[,]
                {
                    { 1, "{\"affiliateTag\":\"your-tag\",\"region\":\"US\"}", 3, new DateTime(2025, 7, 26, 19, 23, 10, 968, DateTimeKind.Utc).AddTicks(7859), true, "Amazon", "affiliate" },
                    { 2, "{\"affiliateId\":\"your-id\",\"region\":\"DE\"}", 2, new DateTime(2025, 7, 26, 19, 23, 10, 968, DateTimeKind.Utc).AddTicks(8024), true, "Zalando", "affiliate" },
                    { 3, "{\"baseUrl\":\"https://www.hervis.hr\",\"searchPath\":\"/search\"}", 1, new DateTime(2025, 7, 26, 19, 23, 10, 968, DateTimeKind.Utc).AddTicks(8026), true, "Hervis", "scraping" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchAnalytics_Query",
                table: "SearchAnalytics",
                column: "Query");

            migrationBuilder.CreateIndex(
                name: "IX_SearchAnalytics_Timestamp",
                table: "SearchAnalytics",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_SearchCaches_ExpiresAt",
                table: "SearchCaches",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_SearchCaches_SearchQuery_Category_StoreId",
                table: "SearchCaches",
                columns: new[] { "SearchQuery", "Category", "StoreId" });

            migrationBuilder.CreateIndex(
                name: "IX_SearchCaches_StoreId",
                table: "SearchCaches",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_CountryId",
                table: "Stores",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Name",
                table: "Stores",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_CountryId",
                table: "UserSettings",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchAnalytics");

            migrationBuilder.DropTable(
                name: "SearchCaches");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
