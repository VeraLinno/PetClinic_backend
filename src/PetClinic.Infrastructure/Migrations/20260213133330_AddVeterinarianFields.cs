using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetClinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVeterinarianFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Veterinarians",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Veterinarians",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Veterinarians",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Veterinarians");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Veterinarians");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Veterinarians");
        }
    }
}