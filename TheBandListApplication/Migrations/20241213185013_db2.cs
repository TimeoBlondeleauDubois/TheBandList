using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBandListApplication.Migrations
{
    /// <inheritdoc />
    public partial class db2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Statut",
                table: "SoumissionsNiveaux");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Statut",
                table: "SoumissionsNiveaux",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
