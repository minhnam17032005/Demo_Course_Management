using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo_Course_Management.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsActiveFromUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
