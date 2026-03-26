using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HQSOFT.Order.Migrations
{
    /// <inheritdoc />
    public partial class add_Deparment_In_Users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "AbpUsers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                defaultValue: "NON");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "AbpUsers");
        }
    }
}
