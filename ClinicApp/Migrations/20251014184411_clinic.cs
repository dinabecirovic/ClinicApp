using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicApp.Migrations
{
    /// <inheritdoc />
    public partial class clinic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffMembers_Clinics_ClinicId",
                table: "StaffMembers");

            migrationBuilder.AlterColumn<int>(
                name: "ClinicId",
                table: "StaffMembers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffMembers_Clinics_ClinicId",
                table: "StaffMembers",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffMembers_Clinics_ClinicId",
                table: "StaffMembers");

            migrationBuilder.AlterColumn<int>(
                name: "ClinicId",
                table: "StaffMembers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffMembers_Clinics_ClinicId",
                table: "StaffMembers",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
