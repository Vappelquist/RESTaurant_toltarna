using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Restaurant.Migrations
{
    /// <inheritdoc />
    public partial class moreSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "AmountOfGuests", "BookingNotes", "DateBooked", "EndTime", "GuestId", "StartTime", "Status" },
                values: new object[] { 2, 4, "Inställt pga sjukdom", new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 10, 21, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2026, 5, 10, 19, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "Id", "Allergies", "Email", "FirstName", "IsEighteen", "LastName", "Note", "Password", "PhoneNumber" },
                values: new object[] { 3, "Laktos", "stig@mail.com", "Stig", true, "Stigsson", "Barnstol behövs", "Lösen123", "070-7778899" });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "AmountOfGuests", "BookingNotes", "DateBooked", "EndTime", "GuestId", "StartTime", "Status" },
                values: new object[] { 3, 12, "Släktträff", new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 15, 20, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2026, 5, 15, 17, 0, 0, 0, DateTimeKind.Unspecified), 0 });

            migrationBuilder.InsertData(
                table: "BookingTables",
                columns: new[] { "BookingsId", "TablesId" },
                values: new object[,]
                {
                    { 3, 5 },
                    { 3, 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BookingTables",
                keyColumns: new[] { "BookingsId", "TablesId" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "BookingTables",
                keyColumns: new[] { "BookingsId", "TablesId" },
                keyValues: new object[] { 3, 6 });

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
