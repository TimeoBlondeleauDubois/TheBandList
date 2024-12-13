using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TheBandListApplication.Data;

namespace TheBandListApplication.Views
{
    public partial class SubmissionPage : Page
    {
        private readonly TheBandListDbContext _dbContext;

        public SubmissionPage()
        {
            InitializeComponent();
            _dbContext = new TheBandListDbContext();
        }

        private void SubmissionPageLoaded(object sender, RoutedEventArgs e)
        {
            LoadSoumissions();
            StatutComboBox.SelectedIndex = 0;
        }

        private void LoadSoumissions()
        {
            var soumissions = _dbContext.SoumissionsNiveaux
                .OrderBy(s => s.DateSoumission)
                .ToList();
            SoumissionListView.ItemsSource = soumissions;
        }

        private void SoumissionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SoumissionListView.SelectedItem is SoumissionNiveau selectedSoumission)
            {
                NomNiveauTextBox.Text = selectedSoumission.NomNiveau;
                NomUtilisateurTextBox.Text = selectedSoumission.NomUtilisateur;
                UrlVideoTextBox.Text = selectedSoumission.UrlVideo;

                VerifierEtVerrouillerChamps();
                ErrorTextBlock.Text = "";
            }
        }

        private void VerifierEtVerrouillerChamps()
        {
            var niveauExiste = _dbContext.Niveaux
                .Any(n => n.Nom.ToLower() == NomNiveauTextBox.Text.ToLower());
            NomNiveauTextBox.IsReadOnly = niveauExiste;
            EditNomNiveauButton.IsEnabled = niveauExiste;

            var utilisateurExiste = _dbContext.Utilisateurs
                .Any(u => u.Nom.ToLower() == NomUtilisateurTextBox.Text.ToLower());
            NomUtilisateurTextBox.IsReadOnly = utilisateurExiste;
            EditNomUtilisateurButton.IsEnabled = utilisateurExiste;
        }

        private void NomNiveauTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var niveauExiste = _dbContext.Niveaux
                .Any(n => n.Nom.ToLower() == NomNiveauTextBox.Text.ToLower());
            NomNiveauTextBox.IsReadOnly = niveauExiste;
            EditNomNiveauButton.IsEnabled = niveauExiste;
        }

        private void NomUtilisateurTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var utilisateurExiste = _dbContext.Utilisateurs
                .Any(u => u.Nom.ToLower() == NomUtilisateurTextBox.Text.ToLower());
            NomUtilisateurTextBox.IsReadOnly = utilisateurExiste;
            EditNomUtilisateurButton.IsEnabled = utilisateurExiste;
        }

        private void EditNomNiveauButton_Click(object sender, RoutedEventArgs e)
        {
            NomNiveauTextBox.IsReadOnly = false;
            NomNiveauTextBox.Clear();
            EditNomNiveauButton.IsEnabled = false;
        }

        private void EditNomUtilisateurButton_Click(object sender, RoutedEventArgs e)
        {
            NomUtilisateurTextBox.IsReadOnly = false;
            NomUtilisateurTextBox.Clear();
            EditNomUtilisateurButton.IsEnabled = false;
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var currentTextBox = sender as TextBox;

                currentTextBox?.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NomNiveauTextBox.Text) ||
                string.IsNullOrWhiteSpace(NomUtilisateurTextBox.Text) ||
                string.IsNullOrWhiteSpace(UrlVideoTextBox.Text))
            {
                if (string.IsNullOrWhiteSpace(NomNiveauTextBox.Text))
                {
                    ErrorTextBlock.Text = "Le nom du niveau doit être rempli.";
                }
                else if (string.IsNullOrWhiteSpace(NomUtilisateurTextBox.Text))
                {
                    ErrorTextBlock.Text = "Le nom de l'utilisateur doit être rempli.";
                }
                else if (string.IsNullOrWhiteSpace(UrlVideoTextBox.Text))
                {
                    ErrorTextBlock.Text = "Le champ url de la vidéo doit être rempli.";
                }
                return;
            }

            if (SoumissionListView.SelectedItem is SoumissionNiveau selectedSoumission)
            {
                var utilisateur = _dbContext.Utilisateurs
                    .FirstOrDefault(u => u.Nom.ToLower() == NomUtilisateurTextBox.Text.ToLower());
                var niveau = _dbContext.Niveaux
                    .FirstOrDefault(n => n.Nom.ToLower() == NomNiveauTextBox.Text.ToLower());

                if (utilisateur == null)
                {
                    ErrorTextBlock.Text = "L'utilisateur n'existe pas dans la base de données.";
                    return;
                }
                else if (niveau == null)
                {
                    ErrorTextBlock.Text = "Le niveau n'existe pas dans la base de données.";
                    return;
                }

                var existeReussite = _dbContext.ReussitesNiveaux.Any(r =>
                    r.UtilisateurId == utilisateur.Id &&
                    r.NiveauId == niveau.Id);

                if (existeReussite)
                {
                    ErrorTextBlock.Text = "Cette réussite existe déjà dans la base de données.";
                    return;
                }

                using var transaction = _dbContext.Database.BeginTransaction();
                try
                {
                    var reussite = new ReussiteNiveau
                    {
                        UtilisateurId = utilisateur.Id,
                        NiveauId = niveau.Id,
                        Video = UrlVideoTextBox.Text,
                        Statut = (StatutComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()
                    };

                    _dbContext.ReussitesNiveaux.Add(reussite);
                    _dbContext.SoumissionsNiveaux.Remove(selectedSoumission);
                    _dbContext.SaveChanges();
                    transaction.Commit();

                    ErrorTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                    ErrorTextBlock.Text = "La réussite a été enregistrée avec succès.";
                    LoadSoumissions();
                    NomNiveauTextBox.Clear();
                    NomUtilisateurTextBox.Clear();
                    UrlVideoTextBox.Clear();
                    StatutComboBox.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorTextBlock.Text = $"Une erreur est survenue : {ex.Message}";
                }
            }
            else
            {
                ErrorTextBlock.Text = "Veuillez sélectionner une soumission.";
            }
        }
    }
}
