using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TheBandListApplication.Migrations
{
    /// <inheritdoc />
    public partial class db2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoumissionsNiveaux",
                columns: table => new
                {
                    IdSoumission = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomNiveau = table.Column<string>(type: "text", nullable: false),
                    UrlVideo = table.Column<string>(type: "text", nullable: false),
                    UtilisateurId = table.Column<int>(type: "integer", nullable: false),
                    Statut = table.Column<string>(type: "text", nullable: false),
                    DateSoumission = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoumissionsNiveaux", x => x.IdSoumission);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoumissionsNiveaux");
        }
    }
}
