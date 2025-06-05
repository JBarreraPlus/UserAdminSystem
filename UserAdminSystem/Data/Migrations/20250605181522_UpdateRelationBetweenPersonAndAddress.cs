using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAdminSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationBetweenPersonAndAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_PersonId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Persons");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_PersonId",
                table: "Addresses",
                column: "PersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_PersonId",
                table: "Addresses");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Persons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_PersonId",
                table: "Addresses",
                column: "PersonId",
                unique: true);
        }
    }
}
