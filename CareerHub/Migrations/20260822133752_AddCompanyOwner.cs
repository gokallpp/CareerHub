using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerHub.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "owner_id",
                table: "companies",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_companies_owner_id",
                table: "companies",
                column: "owner_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_companies_AspNetUsers_owner_id",
                table: "companies",
                column: "owner_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_companies_AspNetUsers_owner_id",
                table: "companies");

            migrationBuilder.DropIndex(
                name: "IX_companies_owner_id",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "companies");
        }
    }
}
