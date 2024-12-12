using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBandListApplication.Migrations
{
    /// <inheritdoc />
    public partial class db3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "SoumissionsNiveaux");

            migrationBuilder.AddColumn<string>(
                name: "NomUtilisateur",
                table: "SoumissionsNiveaux",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomUtilisateur",
                table: "SoumissionsNiveaux");

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurId",
                table: "SoumissionsNiveaux",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
