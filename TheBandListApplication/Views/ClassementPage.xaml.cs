using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TheBandListApplication.Data;

namespace TheBandListApplication.Views
{
    public partial class ClassementPage : Page
    {
        public ObservableCollection<Classement> Classements { get; set; }
        public ObservableCollection<Niveau> NiveauxSansPlacement { get; set; }
        public Visibility NoPlacementVisibility { get; set; } = Visibility.Visible;

        public ClassementPage()
        {
            InitializeComponent();
            Classements = new ObservableCollection<Classement>();
            DataContext = this;
        }

        private void ClassementPageLoaded(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            using (var context = new TheBandListDbContext())
            {
                var classementsAvecNiveau = context.Classements
                    .Include(c => c.Niveau)
                    .Where(c => c.ClassementPosition > 0)
                    .OrderBy(c => c.ClassementPosition)
                    .ToList();

                Classements.Clear();
                foreach (var classement in classementsAvecNiveau)
                {
                    Classements.Add(classement);
                }

                var niveauxSansPlacement = context.Niveaux
                    .Where(n => !context.Classements.Any(c => c.NiveauId == n.Id))
                    .ToList();

                NiveauxSansPlacement = new ObservableCollection<Niveau>(niveauxSansPlacement);
                NoPlacementVisibility = NiveauxSansPlacement.Any() ? Visibility.Visible : Visibility.Collapsed;

                DataContext = null;
                DataContext = this;
            }
        }

        private void OnValidateClick(object sender, RoutedEventArgs e)
        {
            if (DataGridClassements.CommitEdit(DataGridEditingUnit.Row, true) == false)
            {
                SetMessage("Erreur lors de la validation de l'édition.", Colors.Red);
                return;
            }

            if (sender is Button button && button.CommandParameter is Classement updatedClassement)
            {
                if (updatedClassement.ClassementPosition <= 0)
                {
                    SetMessage("Le placement doit être un entier positif non nul.", Colors.Red);
                    return;
                }

                using (var context = new TheBandListDbContext())
                {
                    var allClassements = context.Classements.Include(c => c.Niveau)
                        .Where(c => c.ClassementPosition > 0)
                        .ToList();

                    var classementInDb = allClassements.FirstOrDefault(c => c.Id == updatedClassement.Id);
                    if (classementInDb == null) return;

                    classementInDb.ClassementPosition = updatedClassement.ClassementPosition;

                    var reorderedClassements = allClassements
                        .OrderBy(c => c.ClassementPosition)
                        .Select((c, index) =>
                        {
                            c.ClassementPosition = index + 1;
                            c.Points = CalculerPoints(c.ClassementPosition, allClassements.Count);
                            return c;
                        })
                        .ToList();

                    context.Classements.UpdateRange(reorderedClassements);
                    context.SaveChanges();
                    RecalculerPoints();
                    ClassementPageLoaded(this, null);
                    SetMessage($"Classement mis à jour pour le niveau {updatedClassement.Niveau.Nom}.", Colors.Green);
                }
            }
        }

        private int CalculerPoints(int classementPosition, int totalNiveaux)
        {
            if (totalNiveaux <= 1) return 1000;

            double pointsTop1 = 1000;

            double decayFactor = 0.9;
            double minPoint = 1;

            double points = pointsTop1 * Math.Pow(decayFactor, classementPosition - 1);

            double adjustmentFactor = 1 + ((totalNiveaux - 1) * 0.002);
            points *= adjustmentFactor;

            points = Math.Max(minPoint, points);

            return (int)Math.Round(points);
        }

        private void RecalculerPoints()
        {
            using (var context = new TheBandListDbContext())
            {
                var classements = context.Classements
                    .Include(c => c.Niveau)
                    .Where(c => c.ClassementPosition > 0)
                    .OrderBy(c => c.ClassementPosition)
                    .ToList();

                int totalNiveaux = classements.Count;

                foreach (var classement in classements)
                {
                    classement.Points = CalculerPoints(classement.ClassementPosition, totalNiveaux);
                }

                context.Classements.UpdateRange(classements);
                context.SaveChanges();
            }

            LoadDataGrid();
            SetMessage("Les points ont été recalculés pour tous les niveaux.", Colors.Green);
        }

        private void SetMessage(string message, Color color)
        {
            StatusMessage.Text = message;
            StatusMessage.Foreground = new SolidColorBrush(color);
        }

        private void OnAttribuerClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Niveau selectedNiveau)
            {
                if (selectedNiveau.Placement == null || selectedNiveau.Placement <= 0)
                {
                    SetMessage("Veuillez saisir un placement valide (entier positif).", Colors.Red);
                    return;
                }

                using (var context = new TheBandListDbContext())
                {
                    var allClassements = context.Classements.Include(c => c.Niveau)
                        .Where(c => c.ClassementPosition > 0)
                        .OrderBy(c => c.ClassementPosition)
                        .ToList();

                    int nouvellePosition = selectedNiveau.Placement.Value;

                    if (nouvellePosition > allClassements.Count + 1)
                    {
                        nouvellePosition = allClassements.Count + 1;
                    }

                    foreach (var classement in allClassements.Where(c => c.ClassementPosition >= nouvellePosition))
                    {
                        classement.ClassementPosition++;
                        classement.Points = CalculerPoints(classement.ClassementPosition, allClassements.Count);
                        context.Entry(classement).State = EntityState.Modified;
                    }

                    var classementToAdd = new Classement
                    {
                        NiveauId = selectedNiveau.Id,
                        ClassementPosition = nouvellePosition,
                        Points = CalculerPoints(nouvellePosition, allClassements.Count)
                    };

                    context.Classements.Add(classementToAdd);
                    context.SaveChanges();
                    RecalculerPoints();
                    ClassementPageLoaded(this, null);
                    SetMessage($"Classement mis à jour pour le niveau {selectedNiveau.Nom}.", Colors.Green);
                    LoadDataGrid();
                }
            }
        }
    }
}