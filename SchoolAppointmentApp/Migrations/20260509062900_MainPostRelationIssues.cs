using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolAppointmentApp.Migrations
{
    /// <inheritdoc />
    public partial class MainPostRelationIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CheckMessageHasExactlyOneTypeOfContent",
                table: "Messages");

            migrationBuilder.AddCheckConstraint(
                name: "CheckMessageHasExactlyOneTypeOfContent",
                table: "Messages",
                sql: "(\r\n                (CASE WHEN \"Content\" IS NOT NULL AND length(trim(\"Content\")) > 0 THEN 1 ELSE 0 END) +\r\n                (CASE WHEN \"AudioMessageRoot\" IS NOT NULL THEN 1 ELSE 0 END) +\r\n                (CASE WHEN \"ImageMessageRoot\" IS NOT NULL THEN 1 ELSE 0 END)\r\n            ) = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CheckMessageHasExactlyOneTypeOfContent",
                table: "Messages");

            migrationBuilder.AddCheckConstraint(
                name: "CheckMessageHasExactlyOneTypeOfContent",
                table: "Messages",
                sql: "(\r\n                    (CASE WHEN \"Content\" IS NOT NULL AND length(trim(\"Content\")) > 0 THEN 1 ELSE 0 END) +\r\n                    (CASE WHEN \"AudioMessageRoot\" IS NOT NULL THEN 1 ELSE 0 END) +\r\n                    (CASE WHEN \"ImageMessageRoot\" IS NOT NULL THEN 1 ELSE 0 END)\r\n                ) = 1");
        }
    }
}
