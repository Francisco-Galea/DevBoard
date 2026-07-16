using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBoard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_SoftDelete_To_Contact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Contacts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Contacts");
        }
    }
}
