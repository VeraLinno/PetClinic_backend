using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetClinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVetAuditMetadataAndProtection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add AdminAuditEvent table
            migrationBuilder.CreateTable(
                name: "AdminAuditEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TargetType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    PerformedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PerformedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PerformedByRolesCsv = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MetadataJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminAuditEvents", x => x.Id);
                });

            // Create indices for AdminAuditEvent
            migrationBuilder.CreateIndex(
                name: "IX_AdminAuditEvent_OccurredAtUtc",
                table: "AdminAuditEvents",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AdminAuditEvent_Action_OccurredAtUtc",
                table: "AdminAuditEvents",
                columns: new[] { "Action", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AdminAuditEvent_TargetType_TargetId",
                table: "AdminAuditEvents",
                columns: new[] { "TargetType", "TargetId" });

            migrationBuilder.CreateIndex(
                name: "IX_AdminAuditEvent_PerformedByUserId",
                table: "AdminAuditEvents",
                column: "PerformedByUserId");

            // Add audit metadata columns to Owners table
            migrationBuilder.AddColumn<DateTime>(
                name: "VetAccountCreatedAtUtc",
                table: "Owners",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VetAccountCreatedByUserId",
                table: "Owners",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VetAccountCreatedByRolesCsv",
                table: "Owners",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VetAccountUpdatedAtUtc",
                table: "Owners",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VetAccountUpdatedByUserId",
                table: "Owners",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VetAccountUpdatedByRolesCsv",
                table: "Owners",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VetCleanupProtected",
                table: "Owners",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Create indices for Owner audit fields
            migrationBuilder.CreateIndex(
                name: "IX_Owner_VetCleanupProtected",
                table: "Owners",
                column: "VetCleanupProtected");

            migrationBuilder.CreateIndex(
                name: "IX_Owner_VetAccountCreatedAtUtc",
                table: "Owners",
                column: "VetAccountCreatedAtUtc");

            // Add audit metadata columns to Veterinarians table
            migrationBuilder.AddColumn<DateTime>(
                name: "VetAccountCreatedAtUtc",
                table: "Veterinarians",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VetAccountCreatedByUserId",
                table: "Veterinarians",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VetAccountCreatedByRolesCsv",
                table: "Veterinarians",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VetAccountUpdatedAtUtc",
                table: "Veterinarians",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VetAccountUpdatedByUserId",
                table: "Veterinarians",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VetAccountUpdatedByRolesCsv",
                table: "Veterinarians",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VetCleanupProtected",
                table: "Veterinarians",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Create indices for Veterinarian audit fields
            migrationBuilder.CreateIndex(
                name: "IX_Veterinarian_VetCleanupProtected",
                table: "Veterinarians",
                column: "VetCleanupProtected");

            migrationBuilder.CreateIndex(
                name: "IX_Veterinarian_VetAccountCreatedAtUtc",
                table: "Veterinarians",
                column: "VetAccountCreatedAtUtc");

            // Protect all existing vet accounts (seeded and previously created)
            // This ensures no existing accounts are candidates for future cleanup
            migrationBuilder.Sql(
                @"UPDATE ""Owners"" 
                  SET ""VetCleanupProtected"" = true 
                  WHERE ""Roles"" LIKE '%Vet%' 
                  AND EXISTS (SELECT 1 FROM ""Veterinarians"" WHERE ""Veterinarians"".""Id"" = ""Owners"".""Id"")");

            migrationBuilder.Sql(
                @"UPDATE ""Veterinarians"" 
                  SET ""VetCleanupProtected"" = true 
                  WHERE ""Id"" IN (SELECT ""Id"" FROM ""Owners"" WHERE ""Roles"" LIKE '%Vet%')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop AdminAuditEvent table
            migrationBuilder.DropTable(name: "AdminAuditEvents");

            // Remove audit columns from Owners
            migrationBuilder.DropIndex(
                name: "IX_Owner_VetCleanupProtected",
                table: "Owners");

            migrationBuilder.DropIndex(
                name: "IX_Owner_VetAccountCreatedAtUtc",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "VetAccountCreatedAtUtc",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "VetAccountCreatedByUserId",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "VetAccountCreatedByRolesCsv",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "VetAccountUpdatedAtUtc",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "VetAccountUpdatedByUserId",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "VetAccountUpdatedByRolesCsv",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "VetCleanupProtected",
                table: "Owners");

            // Remove audit columns from Veterinarians
            migrationBuilder.DropIndex(
                name: "IX_Veterinarian_VetCleanupProtected",
                table: "Veterinarians");

            migrationBuilder.DropIndex(
                name: "IX_Veterinarian_VetAccountCreatedAtUtc",
                table: "Veterinarians");

            migrationBuilder.DropColumn(
                name: "VetAccountCreatedAtUtc",
                table: "Veterinarians");

            migrationBuilder.DropColumn(
                name: "VetAccountCreatedByUserId",
                table: "Veterinarians");

            migrationBuilder.DropColumn(
                name: "VetAccountCreatedByRolesCsv",
                table: "Veterinarians");

            migrationBuilder.DropColumn(
                name: "VetAccountUpdatedAtUtc",
                table: "Veterinarians");

            migrationBuilder.DropColumn(
                name: "VetAccountUpdatedByUserId",
                table: "Veterinarians");

            migrationBuilder.DropColumn(
                name: "VetAccountUpdatedByRolesCsv",
                table: "Veterinarians");

            migrationBuilder.DropColumn(
                name: "VetCleanupProtected",
                table: "Veterinarians");
        }
    }
}
