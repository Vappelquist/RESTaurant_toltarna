using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Restaurant.API.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    TableNumber = table.Column<int>(type: "int", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.TableNumber);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuestId = table.Column<int>(type: "int", nullable: false),
                    DateBooked = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AmountOfGuests = table.Column<int>(type: "int", nullable: false),
                    BookingNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_GuestId",
                        column: x => x.GuestId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingTables",
                columns: table => new
                {
                    BookingsId = table.Column<int>(type: "int", nullable: false),
                    TablesTableNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingTables", x => new { x.BookingsId, x.TablesTableNumber });
                    table.ForeignKey(
                        name: "FK_BookingTables_Bookings_BookingsId",
                        column: x => x.BookingsId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingTables_Tables_TablesTableNumber",
                        column: x => x.TablesTableNumber,
                        principalTable: "Tables",
                        principalColumn: "TableNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "TableNumber", "Seats" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 4 },
                    { 4, 4 },
                    { 5, 6 },
                    { 6, 10 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Allergies", "Discriminator", "Email", "FirstName", "LastName", "Note", "Password", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "Nötter", "Guest", "anna@mail.com", "Anna", "Andersson", "Vill sitta vid fönstret", "Lösen123", "070-1112233" },
                    { 2, null, "Guest", "erik@mail.com", "Erik", "Eriksson", "Fyller år!", "Lösen123", "070-4445566" },
                    { 3, "Laktos", "Guest", "stig@mail.com", "Stig", "Stigsson", "Barnstol behövs", "Lösen123", "070-7778899" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Discriminator", "Email", "Password" },
                values: new object[] { 4, "Admin", "admin@mail.com", "Admin123" });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "AmountOfGuests", "BookingNotes", "DateBooked", "EndTime", "GuestId", "StartTime", "Status" },
                values: new object[,]
                {
                    { 1, 2, "Dejt-kväll", new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 8, 20, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2026, 5, 8, 18, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 4, "Inställt pga sjukdom", new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 10, 21, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2026, 5, 10, 19, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, 12, "Släktträff", new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 15, 20, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2026, 5, 15, 17, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "BookingTables",
                columns: new[] { "BookingsId", "TablesTableNumber" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 3, 5 },
                    { 3, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_GuestId",
                table: "Bookings",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingTables_TablesTableNumber",
                table: "BookingTables",
                column: "TablesTableNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_TableNumber",
                table: "Tables",
                column: "TableNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingTables");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
