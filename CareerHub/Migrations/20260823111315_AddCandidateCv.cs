using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerHub.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateCv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CvFileName",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CvStoredFileName",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvFileName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CvStoredFileName",
                table: "AspNetUsers");
        }
    }
}
