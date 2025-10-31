using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank_app_mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddBankAndAccountTypeToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "Transactions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "Transactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Bank",
                table: "Transactions");
        }
    }
}
