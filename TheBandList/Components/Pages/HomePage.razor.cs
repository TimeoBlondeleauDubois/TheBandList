using Microsoft.EntityFrameworkCore;
using TheBandListApplication.Data;

namespace TheBandList.Components.Pages
{
    public partial class HomePage
    {
        private List<Niveau> niveaux;
        private List<Classement> classements;
        private Niveau niveauSelectionne;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                niveaux = await DbContext.Niveaux
                                          .Include(n => n.Rating)
                                              .ThenInclude(r => r.DifficulteRate)
                                          .Include(n => n.Verifieur)
                                          .Include(n => n.Publisher)
                                          .ToListAsync();

                classements = await DbContext.Classements
                                             .OrderBy(c => c.ClassementPosition)
                                             .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des niveaux et classements: {ex.Message}");
            }
        }

        private void AfficherDetailsNiveau(int niveauId)
        {
            niveauSelectionne = niveaux?.FirstOrDefault(n => n.Id == niveauId);
        }

        private string ConvertirDurée(int dureeEnSecondes)
        {
            int minutes = dureeEnSecondes / 60;
            int secondes = dureeEnSecondes % 60;
            return $"{minutes} min {secondes} sec";
        }
    }
}
