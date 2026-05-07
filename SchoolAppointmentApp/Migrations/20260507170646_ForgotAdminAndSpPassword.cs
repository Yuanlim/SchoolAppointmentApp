using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolAppointmentApp.Migrations
{
    /// <inheritdoc />
    public partial class ForgotAdminAndSpPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMRaGtjqdh0fBDTRhUT6cyIJME6FyreJVOKsdXOSOhJ6ifmqjbg1I/4i5TTGP66x9A==");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMRaGtjqdh0fBDTRhUT6cyIJME6FyreJVOKsdXOSOhJ6ifmqjbg1I/4i5TTGP66x9A==");

            migrationBuilder.UpdateData(
                table: "SchoolPrincipal",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMRaGtjqdh0fBDTRhUT6cyIJME6FyreJVOKsdXOSOhJ6ifmqjbg1I/4i5TTGP66x9A==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPkLFJ63cyGZh4YEMMBflj7olrKjkCRfswg70N4NWZyONPxcarnHnhuX2zozI1OGAg==");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEF5EzKnMIKp0aWrnmYAxClS2aiFfz0dDljh38TEU1KdwOcJnzpjiSK6Hczvs53pM1Q==");

            migrationBuilder.UpdateData(
                table: "SchoolPrincipal",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOe6noMHGXGrXzbCSkir9wB2m2z8GwLZTUp69XY2CT9Bpe4dwpTh29iOYbVBPp2dNw==");
        }
    }
}
