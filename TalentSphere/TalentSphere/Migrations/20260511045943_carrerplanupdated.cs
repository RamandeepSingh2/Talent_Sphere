using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentSphere.Migrations
{
    /// <inheritdoc />
    public partial class carrerplanupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timeline",
                table: "CareerPlans");

            migrationBuilder.AddColumn<string>(
                name: "AreasToImprove",
                table: "PerformanceReviews",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewPeriod",
                table: "PerformanceReviews",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Goals",
                table: "CareerPlans",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ReviewID",
                table: "CareerPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TargetDate",
                table: "CareerPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetRole",
                table: "CareerPlans",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CareerPlans_ReviewID",
                table: "CareerPlans",
                column: "ReviewID");

            migrationBuilder.AddForeignKey(
                name: "FK_CareerPlans_PerformanceReviews_ReviewID",
                table: "CareerPlans",
                column: "ReviewID",
                principalTable: "PerformanceReviews",
                principalColumn: "ReviewID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CareerPlans_PerformanceReviews_ReviewID",
                table: "CareerPlans");

            migrationBuilder.DropIndex(
                name: "IX_CareerPlans_ReviewID",
                table: "CareerPlans");

            migrationBuilder.DropColumn(
                name: "AreasToImprove",
                table: "PerformanceReviews");

            migrationBuilder.DropColumn(
                name: "ReviewPeriod",
                table: "PerformanceReviews");

            migrationBuilder.DropColumn(
                name: "ReviewID",
                table: "CareerPlans");

            migrationBuilder.DropColumn(
                name: "TargetDate",
                table: "CareerPlans");

            migrationBuilder.DropColumn(
                name: "TargetRole",
                table: "CareerPlans");

            migrationBuilder.AlterColumn<string>(
                name: "Goals",
                table: "CareerPlans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<string>(
                name: "Timeline",
                table: "CareerPlans",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
