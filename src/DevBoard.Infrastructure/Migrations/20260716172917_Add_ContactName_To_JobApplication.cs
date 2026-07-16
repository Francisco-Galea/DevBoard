using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBoard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_ContactName_To_JobApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Contacts_ContactId",
                table: "JobApplications");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "JobApplications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "JobUrl",
                table: "JobApplications",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentStatus",
                table: "JobApplications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "JobApplications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "JobApplications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Contacts_ContactId",
                table: "JobApplications",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Contacts_ContactId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "JobApplications");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "JobApplications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "JobUrl",
                table: "JobApplications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentStatus",
                table: "JobApplications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "JobApplications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Contacts_ContactId",
                table: "JobApplications",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id");
        }
    }
}
