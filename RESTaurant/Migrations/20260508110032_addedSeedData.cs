using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Restaurant.Migrations
{
    /// <inheritdoc />
    public partial class addedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "Id", "Allergies", "Email", "FirstName", "IsEighteen", "LastName", "Note", "Password", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "Gluten", "anna.lindqvist@email.se", "Anna", true, "Lindqvist", "Föredrar fönsterbord", "password123", "0701234567" },
                    { 2, null, "erik.svensson@email.se", "Erik", true, "Svensson", null, "password123", "0709876543" },
                    { 3, "Nötter, Laktos", "maria.johansson@email.se", "Maria", false, "Johansson", "Allergiker — dubbelkolla alltid", "password123", "0731122334" }
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "Seats", "TableNumber" },
                values: new object[,]
                {
                    { 1, 4, 10 },
                    { 2, 4, 20 },
                    { 3, 4, 30 },
                    { 4, 4, 40 },
                    { 5, 4, 50 },
                    { 6, 4, 60 },
                    { 7, 4, 70 },
                    { 8, 2, 15 },
                    { 9, 2, 25 },
                    { 10, 2, 35 },
                    { 11, 2, 45 },
                    { 12, 2, 55 },
                    { 13, 2, 65 },
                    { 14, 2, 75 }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "AmountOfGuests", "BookingNotes", "DateBooked", "EndTime", "GuestId", "StartTime", "Status" },
                values: new object[,]
                {
                    { 1, 2, "Jubileum — gärna levande ljus", new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 10, 20, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2025, 5, 10, 18, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 2, 4, null, new DateTime(2025, 5, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 11, 21, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2025, 5, 11, 19, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, 6, "Allergiker i sällskapet", new DateTime(2025, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 12, 20, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2025, 5, 12, 17, 30, 0, 0, DateTimeKind.Unspecified), 0 }
                });

            migrationBuilder.InsertData(
                table: "BookingTables",
                columns: new[] { "BookingsId", "TablesId" },
                values: new object[,]
                {
                    { 1, 8 },
                    { 2, 1 },
                    { 3, 2 },
                    { 3, 9 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BookingTables",
                keyColumns: new[] { "BookingsId", "TablesId" },
                keyValues: new object[] { 1, 8 });

            migrationBuilder.DeleteData(
                table: "BookingTables",
                keyColumns: new[] { "BookingsId", "TablesId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "BookingTables",
                keyColumns: new[] { "BookingsId", "TablesId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "BookingTables",
                keyColumns: new[] { "BookingsId", "TablesId" },
                keyValues: new object[] { 3, 9 });

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
