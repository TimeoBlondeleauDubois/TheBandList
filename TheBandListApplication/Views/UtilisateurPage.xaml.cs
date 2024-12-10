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
using System.Windows.Shapes;
using TheBandListApplication.Data;

namespace TheBandListApplication.Views
{
    /// <summary>
    /// Logique d'interaction pour UtilisateurPage.xaml
    /// </summary>
    public partial class UtilisateurPage : Page
    {
        private Utilisateur UtilisateurSelectionne;
        private List<Utilisateur> Utilisateur;

        public UtilisateurPage()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private void UtilisateurPageLoaded(object sender, RoutedEventArgs e)
        {
            ChargerUtilisateurs();
            MessageTextBox.Text = "";
        }

        private void ChargerUtilisateurs()
        {
            using (var context = new TheBandListDbContext())
            {
                Utilisateur = context.Utilisateurs.ToList();
            }
            RafraichirListe();
        }

        private void RafraichirListe()
        {
            Utilisateur = Utilisateur.Where(u => !string.IsNullOrEmpty(u.Nom?.Trim())).ToList();
            UtilisateursDataGrid.ItemsSource = null;
            UtilisateursDataGrid.ItemsSource = Utilisateur;
        }

        private void AjouterUnUtilisateurClick(object sender, RoutedEventArgs e)
        {
            string nomUtilisateur = NomUtilisateurTextBox.Text.Trim();

            if (string.IsNullOrEmpty(nomUtilisateur))
            {
                MessageTextBox.Foreground = Brushes.Red;
                MessageTextBox.Text = "Veuillez entrer un nom d'utilisateur.";
                return;
            }

            using (var context = new TheBandListDbContext())
            {
                if (context.Utilisateurs.Any(u => u.Nom == nomUtilisateur))
                {
                    MessageTextBox.Foreground = Brushes.Red;
                    MessageTextBox.Text = "Cet utilisateur existe déjà.";
                    return;
                }

                var nouvelUtilisateur = new Utilisateur { Nom = nomUtilisateur };
                context.Utilisateurs.Add(nouvelUtilisateur);
                context.SaveChanges();
            }

            NomUtilisateurTextBox.Text = string.Empty;
            MessageTextBox.Foreground = Brushes.Green;
            MessageTextBox.Text = $"Utilisateur {nomUtilisateur} ajouté avec succès !";
            ChargerUtilisateurs();
        }

        private void ModifierUtilisateurClick(object sender, RoutedEventArgs e)
        {
            if (UtilisateursDataGrid.SelectedItem is Utilisateur utilisateur)
            {
                UtilisateurSelectionne = utilisateur;
                NomUtilisateurTextBox.Text = utilisateur.Nom;
                TextBlockTitre.Text = $"Modifier l'utilisateur {utilisateur.Nom}";

                AjouterUtilisateurButton.Visibility = Visibility.Collapsed;
                ModifierUtilisateurButton.Visibility = Visibility.Visible;
                AnnulerModificationButton.Visibility = Visibility.Visible;
            }
        }

        private void ConfirmerModificationClick(object sender, RoutedEventArgs e)
        {
            if (UtilisateurSelectionne != null)
            {
                string nouveauNom = NomUtilisateurTextBox.Text.Trim();

                if (string.IsNullOrEmpty(nouveauNom))
                {
                    MessageTextBox.Foreground = Brushes.Red;
                    MessageTextBox.Text = "Le nom ne peut pas être vide.";
                    return;
                }

                if (nouveauNom == UtilisateurSelectionne.Nom.ToLower())
                {
                    MessageTextBox.Foreground = Brushes.Red;
                    MessageTextBox.Text = "Le nouveau nom est identique à l'ancien.";
                    return;
                }

                using (var context = new TheBandListDbContext())
                {
                    if (context.Utilisateurs.Any(u => u.Nom.ToLower() == nouveauNom))
                    {
                        MessageTextBox.Foreground = Brushes.Red;
                        MessageTextBox.Text = "Un autre utilisateur possède déjà ce nom.";
                        return;
                    }

                    var utilisateurDb = context.Utilisateurs.FirstOrDefault(u => u.Id == UtilisateurSelectionne.Id);
                    if (utilisateurDb != null)
                    {
                        utilisateurDb.Nom = nouveauNom;
                        context.SaveChanges();

                        MessageTextBox.Foreground = Brushes.Green;
                        MessageTextBox.Text = $"Utilisateur {UtilisateurSelectionne.Nom} modifié avec succès en {nouveauNom}.";
                    }
                }

                UtilisateurSelectionne = null;
                NomUtilisateurTextBox.Text = string.Empty;
                TextBlockTitre.Text = "Entrer le nom d'un utilisateur:";
                AjouterUtilisateurButton.Visibility = Visibility.Visible;
                ModifierUtilisateurButton.Visibility = Visibility.Collapsed;
                AnnulerModificationButton.Visibility = Visibility.Collapsed;

                ChargerUtilisateurs();
            }
        }

        private void AnnulerModificationClick(object sender, RoutedEventArgs e)
        {
            UtilisateurSelectionne = null;
            NomUtilisateurTextBox.Text = string.Empty;
            TextBlockTitre.Text = "Entrer le nom d'un utilisateur:";
            AjouterUtilisateurButton.Visibility = Visibility.Visible;
            ModifierUtilisateurButton.Visibility = Visibility.Collapsed;
            AnnulerModificationButton.Visibility = Visibility.Collapsed;
            MessageTextBox.Text = string.Empty;
        }

        private void SupprimerUtilisateurClick(object sender, RoutedEventArgs e)
        {
            if (UtilisateursDataGrid.SelectedItem is Utilisateur utilisateurASupprimer)
            {
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer l'utilisateur {utilisateurASupprimer.Nom} ?",
                    "Confirmation de suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new TheBandListDbContext())
                    {
                        var utilisateurDb = context.Utilisateurs.FirstOrDefault(u => u.Id == utilisateurASupprimer.Id);
                        if (utilisateurDb != null)
                        {
                            context.Utilisateurs.Remove(utilisateurDb);
                            context.SaveChanges();

                            MessageTextBox.Foreground = Brushes.Green;
                            MessageTextBox.Text = $"Utilisateur {utilisateurASupprimer.Nom} supprimé avec succès.";
                        }
                        else
                        {
                            MessageTextBox.Foreground = Brushes.Red;
                            MessageTextBox.Text = "Erreur : utilisateur introuvable dans la base de données.";
                        }
                    }

                    ChargerUtilisateurs();
                }
            }
            else
            {
                MessageTextBox.Foreground = Brushes.Red;
                MessageTextBox.Text = "Veuillez sélectionner un utilisateur à supprimer.";
            }
        }
    }
}
