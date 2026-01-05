using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank_Project.Migrations
{
    /// <inheritdoc />
    public partial class FixAccountCustomerFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Customers_CUID",
                table: "Accounts");

            migrationBuilder.AlterColumn<string>(
                name: "AccountType",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Customers",
                table: "Accounts",
                column: "CUID",
                principalTable: "Customers",
                principalColumn: "CUID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Customers",
                table: "Accounts");

            migrationBuilder.AlterColumn<string>(
                name: "AccountType",
                table: "Accounts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Customers_CUID",
                table: "Accounts",
                column: "CUID",
                principalTable: "Customers",
                principalColumn: "CUID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
