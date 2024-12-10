using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TheBandListApplication.Migrations
{
    /// <inheritdoc />
    public partial class db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NiveauxDifficulteRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomDeLaDifficulte = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NiveauxDifficulteRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Packs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    PointsBonus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DifficulteFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomDuFeature = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false),
                    DifficulteRateId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DifficulteFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DifficulteFeatures_NiveauxDifficulteRates_DifficulteRateId",
                        column: x => x.DifficulteRateId,
                        principalTable: "NiveauxDifficulteRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReussitesPack",
                columns: table => new
                {
                    UtilisateurId = table.Column<int>(type: "integer", nullable: false),
                    PackId = table.Column<int>(type: "integer", nullable: false),
                    DateReussite = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReussitesPack", x => new { x.UtilisateurId, x.PackId });
                    table.ForeignKey(
                        name: "FK_ReussitesPack_Packs_PackId",
                        column: x => x.PackId,
                        principalTable: "Packs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReussitesPack_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Niveaux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdDuNiveauDansLeJeu = table.Column<string>(type: "text", nullable: false),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    MotDePasse = table.Column<string>(type: "text", nullable: false),
                    UrlIframeSrcVerification = table.Column<string>(type: "text", nullable: false),
                    Miniature = table.Column<string>(type: "text", nullable: false),
                    Duree = table.Column<int>(type: "integer", nullable: false),
                    DateAjout = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Placement = table.Column<int>(type: "integer", nullable: true),
                    VerifieurId = table.Column<int>(type: "integer", nullable: false),
                    PublisherId = table.Column<int>(type: "integer", nullable: false),
                    RatingId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Niveaux", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Niveaux_DifficulteFeatures_RatingId",
                        column: x => x.RatingId,
                        principalTable: "DifficulteFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Niveaux_Utilisateurs_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Niveaux_Utilisateurs_VerifieurId",
                        column: x => x.VerifieurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClassementPosition = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    NiveauId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classements_Niveaux_NiveauId",
                        column: x => x.NiveauId,
                        principalTable: "Niveaux",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreateursNiveaux",
                columns: table => new
                {
                    CreateurId = table.Column<int>(type: "integer", nullable: false),
                    NiveauId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateursNiveaux", x => new { x.CreateurId, x.NiveauId });
                    table.ForeignKey(
                        name: "FK_CreateursNiveaux_Niveaux_NiveauId",
                        column: x => x.NiveauId,
                        principalTable: "Niveaux",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreateursNiveaux_Utilisateurs_CreateurId",
                        column: x => x.CreateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackNiveaux",
                columns: table => new
                {
                    PackId = table.Column<int>(type: "integer", nullable: false),
                    NiveauId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackNiveaux", x => new { x.PackId, x.NiveauId });
                    table.ForeignKey(
                        name: "FK_PackNiveaux_Niveaux_NiveauId",
                        column: x => x.NiveauId,
                        principalTable: "Niveaux",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackNiveaux_Packs_PackId",
                        column: x => x.PackId,
                        principalTable: "Packs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReussitesNiveaux",
                columns: table => new
                {
                    UtilisateurId = table.Column<int>(type: "integer", nullable: false),
                    NiveauId = table.Column<int>(type: "integer", nullable: false),
                    Video = table.Column<string>(type: "text", nullable: false),
                    Statut = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReussitesNiveaux", x => new { x.UtilisateurId, x.NiveauId });
                    table.ForeignKey(
                        name: "FK_ReussitesNiveaux_Niveaux_NiveauId",
                        column: x => x.NiveauId,
                        principalTable: "Niveaux",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReussitesNiveaux_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classements_NiveauId",
                table: "Classements",
                column: "NiveauId");

            migrationBuilder.CreateIndex(
                name: "IX_CreateursNiveaux_NiveauId",
                table: "CreateursNiveaux",
                column: "NiveauId");

            migrationBuilder.CreateIndex(
                name: "IX_DifficulteFeatures_DifficulteRateId",
                table: "DifficulteFeatures",
                column: "DifficulteRateId");

            migrationBuilder.CreateIndex(
                name: "IX_Niveaux_PublisherId",
                table: "Niveaux",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Niveaux_RatingId",
                table: "Niveaux",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_Niveaux_VerifieurId",
                table: "Niveaux",
                column: "VerifieurId");

            migrationBuilder.CreateIndex(
                name: "IX_PackNiveaux_NiveauId",
                table: "PackNiveaux",
                column: "NiveauId");

            migrationBuilder.CreateIndex(
                name: "IX_ReussitesNiveaux_NiveauId",
                table: "ReussitesNiveaux",
                column: "NiveauId");

            migrationBuilder.CreateIndex(
                name: "IX_ReussitesPack_PackId",
                table: "ReussitesPack",
                column: "PackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Classements");

            migrationBuilder.DropTable(
                name: "CreateursNiveaux");

            migrationBuilder.DropTable(
                name: "PackNiveaux");

            migrationBuilder.DropTable(
                name: "ReussitesNiveaux");

            migrationBuilder.DropTable(
                name: "ReussitesPack");

            migrationBuilder.DropTable(
                name: "Niveaux");

            migrationBuilder.DropTable(
                name: "Packs");

            migrationBuilder.DropTable(
                name: "DifficulteFeatures");

            migrationBuilder.DropTable(
                name: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "NiveauxDifficulteRates");
        }
    }
}
