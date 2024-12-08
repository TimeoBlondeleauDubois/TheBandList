using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheBandListApplication.Data;

namespace TheBandListApplication.Views
{
    /// <summary>
    /// Logique d'interaction pour DifficultePage.xaml
    /// </summary>
    public partial class DifficultePage : Page
    {
        private NiveauDifficulteRate DifficulteSelectionnee;
        private List<NiveauDifficulteRate> ListeNiveauxDifficulte;

        public DifficultePage()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private void DifficultePageLoaded(object sender, RoutedEventArgs e)
        {
            ChargerDifficulte();
            MessageTextBox.Text = "";
        }

        private void ChargerDifficulte()
        {
            using (var context = new TheBandListDbContext())
            {
                ListeNiveauxDifficulte = context.NiveauxDifficulteRates.ToList();
            }
            RafraichirListe();
        }

        private void RafraichirListe()
        {
            ListeNiveauxDifficulte = ListeNiveauxDifficulte
                .Where(n => !string.IsNullOrEmpty(n.NomDeLaDifficulte?.Trim()))
                .ToList();
            DifficulteDataGrid.ItemsSource = null;
            DifficulteDataGrid.ItemsSource = ListeNiveauxDifficulte;
        }

        private void AjouterDifficulteClick(object sender, RoutedEventArgs e)
        {
            string nomDifficulte = NomDifficulteTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(nomDifficulte))
            {
                MessageTextBox.Foreground = Brushes.Red;
                MessageTextBox.Text = "Veuillez entrer le nom de la difficulté.";
                return;
            }

            using (var context = new TheBandListDbContext())
            {
                if (context.NiveauxDifficulteRates.Any(n => n.NomDeLaDifficulte == nomDifficulte))
                {
                    MessageTextBox.Foreground = Brushes.Red;
                    MessageTextBox.Text = "Cette difficulté existe déjà.";
                    return;
                }

                var nouvelleDifficulte = new NiveauDifficulteRate { NomDeLaDifficulte = nomDifficulte };
                context.NiveauxDifficulteRates.Add(nouvelleDifficulte);
                context.SaveChanges();
            }

            NomDifficulteTextBox.Text = string.Empty;
            MessageTextBox.Foreground = Brushes.Green;
            MessageTextBox.Text = $"Difficulté '{nomDifficulte}' ajoutée avec succès !";
            ChargerDifficulte();
        }

        private void ModifierDifficulteClick(object sender, RoutedEventArgs e)
        {
            if (DifficulteDataGrid.SelectedItem is NiveauDifficulteRate niveauDifficulteRate)
            {
                DifficulteSelectionnee = niveauDifficulteRate;
                NomDifficulteTextBox.Text = niveauDifficulteRate.NomDeLaDifficulte;
                TextBlockTitre.Text = $"Modifier la difficulté {niveauDifficulteRate.NomDeLaDifficulte}";

                AjouterDifficulteButton.Visibility = Visibility.Collapsed;
                ModifierDifficulteButton.Visibility = Visibility.Visible;
                AnnulerModificationButton.Visibility = Visibility.Visible;
            }
        }

        private void ConfirmerModificationClick(object sender, RoutedEventArgs e)
        {
            if (DifficulteSelectionnee != null)
            {
                string nouveauNom = NomDifficulteTextBox.Text.Trim().ToLower();

                if (string.IsNullOrEmpty(nouveauNom))
                {
                    MessageTextBox.Foreground = Brushes.Red;
                    MessageTextBox.Text = "Le nom ne peut pas être vide.";
                    return;
                }

                if (nouveauNom == DifficulteSelectionnee.NomDeLaDifficulte.ToLower())
                {
                    MessageTextBox.Foreground = Brushes.Red;
                    MessageTextBox.Text = "Le nouveau nom est identique à l'ancien.";
                    return;
                }

                using (var context = new TheBandListDbContext())
                {
                    if (context.NiveauxDifficulteRates.Any(u => u.NomDeLaDifficulte.ToLower() == nouveauNom))
                    {
                        MessageTextBox.Foreground = Brushes.Red;
                        MessageTextBox.Text = "Une autre difficulté possède déjà ce nom.";
                        return;
                    }

                    var difficulteDb = context.NiveauxDifficulteRates
                        .FirstOrDefault(u => u.Id == DifficulteSelectionnee.Id);
                    if (difficulteDb != null)
                    {
                        difficulteDb.NomDeLaDifficulte = nouveauNom;
                        context.SaveChanges();

                        MessageTextBox.Foreground = Brushes.Green;
                        MessageTextBox.Text = $"Difficulté '{DifficulteSelectionnee.NomDeLaDifficulte}' modifiée avec succès en '{nouveauNom}'.";
                    }
                }

                DifficulteSelectionnee = null;
                NomDifficulteTextBox.Text = string.Empty;
                TextBlockTitre.Text = "Entrer le nom d'une difficulté :";
                AjouterDifficulteButton.Visibility = Visibility.Visible;
                ModifierDifficulteButton.Visibility = Visibility.Collapsed;
                AnnulerModificationButton.Visibility = Visibility.Collapsed;

                ChargerDifficulte();
            }
        }

        private void AnnulerModificationClick(object sender, RoutedEventArgs e)
        {
            DifficulteSelectionnee = null;
            NomDifficulteTextBox.Text = string.Empty;
            TextBlockTitre.Text = "Entrer le nom d'une difficulté :";
            AjouterDifficulteButton.Visibility = Visibility.Visible;
            ModifierDifficulteButton.Visibility = Visibility.Collapsed;
            AnnulerModificationButton.Visibility = Visibility.Collapsed;
            MessageTextBox.Text = string.Empty;
        }

        private void SupprimerDifficulteClick(object sender, RoutedEventArgs e)
        {
            if (DifficulteDataGrid.SelectedItem is NiveauDifficulteRate difficulteASupprimer)
            {
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer la difficulté '{difficulteASupprimer.NomDeLaDifficulte}' ?",
                    "Confirmation de suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new TheBandListDbContext())
                    {
                        var difficulteDb = context.NiveauxDifficulteRates
                            .FirstOrDefault(u => u.Id == difficulteASupprimer.Id);
                        if (difficulteDb != null)
                        {
                            context.NiveauxDifficulteRates.Remove(difficulteDb);
                            context.SaveChanges();

                            MessageTextBox.Foreground = Brushes.Green;
                            MessageTextBox.Text = $"Difficulté '{difficulteASupprimer.NomDeLaDifficulte}' supprimée avec succès.";
                        }
                        else
                        {
                            MessageTextBox.Foreground = Brushes.Red;
                            MessageTextBox.Text = "Erreur : difficulté introuvable dans la base de données.";
                        }
                    }

                    ChargerDifficulte();
                }
            }
            else
            {
                MessageTextBox.Foreground = Brushes.Red;
                MessageTextBox.Text = "Veuillez sélectionner une difficulté à supprimer.";
            }
        }
    }
}