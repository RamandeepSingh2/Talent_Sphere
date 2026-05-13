using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentSphere.Migrations
{
    /// <inheritdoc />
    public partial class Updatemigration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TargetDate",
                table: "SuccessionPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetPosition",
                table: "SuccessionPlans",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetDate",
                table: "SuccessionPlans");

            migrationBuilder.DropColumn(
                name: "TargetPosition",
                table: "SuccessionPlans");
        }
    }
}
