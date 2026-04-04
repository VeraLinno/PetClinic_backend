using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetClinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminTrackingFieldsAndInvoicePaymentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Owners"" ADD COLUMN IF NOT EXISTS ""LastLoginAt"" timestamp with time zone;");

            migrationBuilder.Sql(@"ALTER TABLE ""Veterinarians"" ADD COLUMN IF NOT EXISTS ""IsActive"" boolean;");
            migrationBuilder.Sql(@"UPDATE ""Veterinarians"" SET ""IsActive"" = TRUE WHERE ""IsActive"" IS NULL;");
            migrationBuilder.Sql(@"ALTER TABLE ""Veterinarians"" ALTER COLUMN ""IsActive"" SET DEFAULT TRUE;");
            migrationBuilder.Sql(@"ALTER TABLE ""Veterinarians"" ALTER COLUMN ""IsActive"" SET NOT NULL;");

            migrationBuilder.Sql(@"ALTER TABLE ""Invoices"" ADD COLUMN IF NOT EXISTS ""Status"" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql(@"ALTER TABLE ""Invoices"" ADD COLUMN IF NOT EXISTS ""PaidAt"" timestamp with time zone;");
            migrationBuilder.Sql(@"ALTER TABLE ""Invoices"" ADD COLUMN IF NOT EXISTS ""DueDate"" timestamp with time zone;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Invoices"" DROP COLUMN IF EXISTS ""DueDate"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Invoices"" DROP COLUMN IF EXISTS ""PaidAt"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Invoices"" DROP COLUMN IF EXISTS ""Status"";");

            migrationBuilder.Sql(@"ALTER TABLE ""Veterinarians"" DROP COLUMN IF EXISTS ""IsActive"";");

            migrationBuilder.Sql(@"ALTER TABLE ""Owners"" DROP COLUMN IF EXISTS ""LastLoginAt"";");
        }
    }
}
