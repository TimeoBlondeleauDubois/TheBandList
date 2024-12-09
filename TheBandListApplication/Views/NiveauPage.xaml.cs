using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logique d'interaction pour NiveauPage.xaml
    /// </summary>
    public partial class NiveauPage : Page
    {
        private List<Niveau> ListeNiveaux = new List<Niveau>();
        private Niveau NiveauSelectionne = null;
        private string miniatureBase64 = null;

        public NiveauPage()
        {
            InitializeComponent();
        }

        private void NiveauPageLoaded(object sender, RoutedEventArgs e)
        {
            ChargerNiveaux();
            MessageNiveauTextBox.Text = "";
            ChargerUtilisateurs();
            ChargerDifficultes();
        }

        private void ChargerNiveaux()
        {
            using (var context = new TheBandListDbContext())
            {
                ListeNiveaux = context.Niveaux.ToList();
                ListeNiveaux.Insert(0, new Niveau { Id = 0, Nom = "Aucun niveau sélectionné" });
                NiveauxComboBox.ItemsSource = ListeNiveaux;
                NiveauxComboBox.SelectedIndex = 0;
            }
        }

        private void AjouterNiveauClick(object sender, RoutedEventArgs e)
        {
            string nomNiveau = NomNiveauTextBox.Text.Trim();
            string motDePasse = MotDePasseTextBox.Text.Trim();
            string idDuNiveauDansLeJeu = IdDuNiveauDansLeJeuTextBox.Text.Trim();
            string urlVerification = UrlVerificationTextBox.Text.Trim();

            int.TryParse(MinutesTextBox.Text.Trim(), out int minutes);
            int.TryParse(SecondesTextBox.Text.Trim(), out int secondes);

            int duree = (minutes * 60) + secondes;

            var verifieurSelectionne = VerifieurComboBox.SelectedItem as Utilisateur;
            var publisherSelectionne = PublisherComboBox.SelectedItem as Utilisateur;
            var ratingSelectionne = RatingComboBox.SelectedItem as DifficulteFeature;

            string erreur = VerifierChamps(nomNiveau, motDePasse, idDuNiveauDansLeJeu, urlVerification, minutes, secondes, verifieurSelectionne, publisherSelectionne, ratingSelectionne);
            if (erreur != null)
            {
                MessageNiveauTextBox.Foreground = Brushes.Red;
                MessageNiveauTextBox.Text = erreur;
                return;
            }

            using (var context = new TheBandListDbContext())
            {
                var nouveauNiveau = new Niveau
                {
                    Nom = nomNiveau,
                    MotDePasse = motDePasse,
                    IdDuNiveauDansLeJeu = idDuNiveauDansLeJeu,
                    UrlIframeSrcVerification = urlVerification,
                    Duree = duree,
                    Miniature = miniatureBase64,
                    DateAjout = DateTime.Now,
                    VerifieurId = verifieurSelectionne.Id,
                    PublisherId = publisherSelectionne.Id,
                    RatingId = ratingSelectionne.Id
                };

                context.Niveaux.Add(nouveauNiveau);
                context.SaveChanges();
            }

            MessageNiveauTextBox.Foreground = Brushes.Green;
            MessageNiveauTextBox.Text = $"Niveau {nomNiveau} ajouté avec succès.";
            ResetFormulaire();
            ChargerNiveaux();
        }

        private string VerifierChamps(string nomNiveau, string motDePasse, string idDuNiveauDansLeJeu, string urlVerification, int minutes, int secondes, Utilisateur verifieur, Utilisateur publisher, DifficulteFeature rating)
        {
            if (string.IsNullOrEmpty(nomNiveau))
                return "Le nom du niveau est requis.";

            if (string.IsNullOrEmpty(motDePasse))
                return "Le mot de passe est requis.";

            if (!int.TryParse(idDuNiveauDansLeJeu, out _))
                return "L'ID du niveau dans le jeu doit être un entier.";

            if (minutes < 0 || secondes < 0 || (minutes == 0 && secondes == 0))
                return "La durée doit être positive et spécifiée en minutes et/ou secondes.";

            if (string.IsNullOrEmpty(miniatureBase64) && (NiveauSelectionne == null || string.IsNullOrEmpty(NiveauSelectionne.Miniature)))
                return "Veuillez sélectionner une miniature.";

            if (string.IsNullOrEmpty(urlVerification))
                return "L'URL de vérification est requise.";

            if (verifieur == null || verifieur.Id == 0)
                return "Veuillez sélectionner un vérificateur.";

            if (publisher == null || publisher.Id == 0)
                return "Veuillez sélectionner un publieur.";

            if (rating == null || rating.Id == 0)
                return "Veuillez sélectionner une difficulté.";

            return null;
        }


        private void ModifierNiveauClick(object sender, RoutedEventArgs e)
        {
            if (NiveauSelectionne != null)
            {
                string nouveauNom = NomNiveauTextBox.Text.Trim();
                string motDePasse = MotDePasseTextBox.Text.Trim();
                string idDuNiveauDansLeJeu = IdDuNiveauDansLeJeuTextBox.Text.Trim();
                string urlVerification = UrlVerificationTextBox.Text.Trim();

                int.TryParse(MinutesTextBox.Text.Trim(), out int minutes);
                int.TryParse(SecondesTextBox.Text.Trim(), out int secondes);

                int duree = (minutes * 60) + secondes;

                var verifieurSelectionne = VerifieurComboBox.SelectedItem as Utilisateur;
                var publisherSelectionne = PublisherComboBox.SelectedItem as Utilisateur;
                var ratingSelectionne = RatingComboBox.SelectedItem as DifficulteFeature;

                string erreur = VerifierChamps(nouveauNom, motDePasse, idDuNiveauDansLeJeu, urlVerification, minutes, secondes, verifieurSelectionne, publisherSelectionne, ratingSelectionne);
                if (erreur != null)
                {
                    MessageNiveauTextBox.Foreground = Brushes.Red;
                    MessageNiveauTextBox.Text = erreur;
                    return;
                }

                using (var context = new TheBandListDbContext())
                {
                    var niveauDb = context.Niveaux.FirstOrDefault(n => n.Id == NiveauSelectionne.Id);
                    if (niveauDb != null)
                    {
                        niveauDb.Nom = nouveauNom;
                        niveauDb.MotDePasse = motDePasse;
                        niveauDb.IdDuNiveauDansLeJeu = idDuNiveauDansLeJeu;
                        niveauDb.UrlIframeSrcVerification = urlVerification;
                        niveauDb.Duree = duree;
                        niveauDb.Miniature = miniatureBase64 ?? niveauDb.Miniature;
                        niveauDb.VerifieurId = verifieurSelectionne.Id;
                        niveauDb.PublisherId = publisherSelectionne.Id;
                        niveauDb.RatingId = ratingSelectionne.Id;

                        context.SaveChanges();
                    }
                }

                MessageNiveauTextBox.Foreground = Brushes.Green;
                MessageNiveauTextBox.Text = $"Niveau {NiveauSelectionne.Nom} modifié avec succès.";
                ResetFormulaire();
                ChargerNiveaux();
            }
        }

        private void ChargerUtilisateurs()
        {
            using (var context = new TheBandListDbContext())
            {
                var utilisateurs = context.Utilisateurs.ToList();
                utilisateurs.Insert(0, new Utilisateur { Id = 0, Nom = "Aucun utilisateur sélectionné" });

                VerifieurComboBox.ItemsSource = utilisateurs;
                PublisherComboBox.ItemsSource = utilisateurs;
                CreateurComboBox.ItemsSource = utilisateurs;

                VerifieurComboBox.SelectedIndex = 0;
                PublisherComboBox.SelectedIndex = 0;
                CreateurComboBox.SelectedIndex = 0;
            }
        }

        private void ChargerDifficultes()
        {
            using (var context = new TheBandListDbContext())
            {
                var difficultes = context.DifficulteFeatures.ToList();
                difficultes.Insert(0, new DifficulteFeature { Id = 0, NomDuFeature = "Aucune difficulté sélectionnée" });

                RatingComboBox.ItemsSource = difficultes;
                RatingComboBox.SelectedIndex = 0;
            }
        }

        private void SupprimerNiveauClick(object sender, RoutedEventArgs e)
        {
            if (NiveauSelectionne != null)
            {
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer le niveau {NiveauSelectionne.Nom} ?",
                    "Confirmation de suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new TheBandListDbContext())
                    {
                        var niveauDb = context.Niveaux.FirstOrDefault(n => n.Id == NiveauSelectionne.Id);
                        if (niveauDb != null)
                        {
                            context.Niveaux.Remove(niveauDb);
                            context.SaveChanges();
                        }
                    }

                    MessageNiveauTextBox.Foreground = Brushes.Green;
                    MessageNiveauTextBox.Text = $"Niveau {NiveauSelectionne.Nom} supprimé.";
                    ResetFormulaire();
                    ChargerNiveaux();
                }
                else
                {
                    MessageNiveauTextBox.Foreground = Brushes.Orange;
                    MessageNiveauTextBox.Text = "Suppression annulée.";
                }
            }
        }

        private void ResetFormulaire()
        {
            NomNiveauTextBox.Text = string.Empty;
            MotDePasseTextBox.Text = string.Empty;
            MinutesTextBox.Text = string.Empty;
            IdDuNiveauDansLeJeuTextBox.Text = string.Empty;
            SecondesTextBox.Text = string.Empty;
            UrlVerificationTextBox.Text = string.Empty;
            SelectedMiniaturePreview.Source = null;
            SelectedMiniaturePreview.Visibility = Visibility.Collapsed;
            miniatureBase64 = null;
            VerifieurComboBox.SelectedIndex = 0;
            PublisherComboBox.SelectedIndex = 0;
            NiveauxComboBox.SelectedIndex = 0;
            RatingComboBox.SelectedIndex = 0;
            MessageNiveauTextBox.Text = string.Empty;
            AjouterNiveauButton.Visibility = Visibility.Visible;
            ModifierNiveauButton.Visibility = Visibility.Collapsed;
            AnnulerModificationNiveauButton.Visibility = Visibility.Collapsed;
            SupprimerNiveauButton.Visibility = Visibility.Collapsed;
            ListeCreateur.Text = "Liste des créateurs :";
        }

        private void MiniatureSelectionButtonClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Images (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                byte[] imageBytes = File.ReadAllBytes(filePath);
                miniatureBase64 = Convert.ToBase64String(imageBytes);

                SelectedMiniaturePreview.Source = new BitmapImage(new Uri(filePath));
                SelectedMiniaturePreview.Visibility = Visibility.Visible;
            }
        }

        private void NiveauxComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NiveauSelectionne = NiveauxComboBox.SelectedItem as Niveau;
            if (NiveauSelectionne != null && NiveauSelectionne.Id != 0)
            {
                using (var context = new TheBandListDbContext())
                {
                    var createurs = context.CreateursNiveaux
                        .Where(cn => cn.NiveauId == NiveauSelectionne.Id)
                        .Select(cn => cn.Createur)
                        .ToList();
                    CreateursListBox.ItemsSource = createurs;
                }

                ListeCreateur.Text = $"Liste des créateurs du niveau {NiveauSelectionne.Nom} :";
                NomNiveauTextBox.Text = NiveauSelectionne.Nom;
                MotDePasseTextBox.Text = NiveauSelectionne.MotDePasse;
                UrlVerificationTextBox.Text = NiveauSelectionne.UrlIframeSrcVerification;
                IdDuNiveauDansLeJeuTextBox.Text = NiveauSelectionne.IdDuNiveauDansLeJeu;

                int totalSeconds = NiveauSelectionne.Duree;
                MinutesTextBox.Text = (totalSeconds / 60).ToString();
                SecondesTextBox.Text = (totalSeconds % 60).ToString();

                VerifieurComboBox.SelectedValue = NiveauSelectionne.VerifieurId;
                PublisherComboBox.SelectedValue = NiveauSelectionne.PublisherId;
                RatingComboBox.SelectedValue = NiveauSelectionne.RatingId;

                if (!string.IsNullOrEmpty(NiveauSelectionne.Miniature))
                {
                    byte[] imageBytes = Convert.FromBase64String(NiveauSelectionne.Miniature);
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        SelectedMiniaturePreview.Source = bitmap;
                        SelectedMiniaturePreview.Visibility = Visibility.Visible;
                    }
                }

                AjouterNiveauButton.Visibility = Visibility.Collapsed;
                ModifierNiveauButton.Visibility = Visibility.Visible;
                AnnulerModificationNiveauButton.Visibility = Visibility.Visible;
                SupprimerNiveauButton.Visibility = Visibility.Visible;
            }
            else
            {
                ResetFormulaire();
                CreateursListBox.ItemsSource = null;
            }
        }

        private void AnnulerModificationNiveauClick(object sender, RoutedEventArgs e)
        {
            ResetFormulaire();
        }

        private void AjouterCreateurButtonClick(object sender, RoutedEventArgs e)
        {
            if (NiveauSelectionne == null || NiveauSelectionne.Id == 0)
            {
                MessageNiveauTextBoxCreateur.Foreground = Brushes.Red;
                MessageNiveauTextBoxCreateur.Text = "Veuillez sélectionner un niveau.";
                return;
            }

            var createurSelectionne = CreateurComboBox.SelectedItem as Utilisateur;

            if (createurSelectionne == null || createurSelectionne.Id == 0)
            {
                MessageNiveauTextBoxCreateur.Foreground = Brushes.Red;
                MessageNiveauTextBoxCreateur.Text = "Veuillez sélectionner un créateur.";
                return;
            }

            using (var context = new TheBandListDbContext())
            {
                var createurNiveauExist = context.CreateursNiveaux
                    .Any(cn => cn.CreateurId == createurSelectionne.Id && cn.NiveauId == NiveauSelectionne.Id);

                if (!createurNiveauExist)
                {
                    var createurNiveau = new CreateurNiveau
                    {
                        CreateurId = createurSelectionne.Id,
                        NiveauId = NiveauSelectionne.Id
                    };

                    context.CreateursNiveaux.Add(createurNiveau);
                    context.SaveChanges();

                    var createurs = context.CreateursNiveaux
                        .Where(cn => cn.NiveauId == NiveauSelectionne.Id)
                        .Select(cn => cn.Createur)
                        .ToList();
                    CreateursListBox.ItemsSource = createurs;

                    MessageNiveauTextBoxCreateur.Foreground = Brushes.Green;
                    MessageNiveauTextBoxCreateur.Text = $"Créateur {createurSelectionne.Nom} ajouté au niveau {NiveauSelectionne.Nom}.";
                }
                else
                {
                    MessageNiveauTextBoxCreateur.Foreground = Brushes.Red;
                    MessageNiveauTextBoxCreateur.Text = "Ce créateur est déjà associé à ce niveau.";
                }

            }
        }

        private void SupprimerCreateurButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var stackPanel = button.Parent as StackPanel;
                if (stackPanel != null)
                {
                    var createur = stackPanel.DataContext as Utilisateur;
                    if (createur != null && NiveauSelectionne != null)
                    {
                        using (var context = new TheBandListDbContext())
                        {
                            var createurNiveau = context.CreateursNiveaux
                                .FirstOrDefault(cn => cn.CreateurId == createur.Id && cn.NiveauId == NiveauSelectionne.Id);

                            if (createurNiveau != null)
                            {
                                context.CreateursNiveaux.Remove(createurNiveau);
                                context.SaveChanges();

                                var createurs = context.CreateursNiveaux
                                    .Where(cn => cn.NiveauId == NiveauSelectionne.Id)
                                    .Select(cn => cn.Createur)
                                    .ToList();

                                CreateursListBox.ItemsSource = createurs;

                                MessageNiveauTextBoxCreateur.Foreground = Brushes.Green;
                                MessageNiveauTextBoxCreateur.Text = $"Le créateur {createur.Nom} a été supprimé du niveau {NiveauSelectionne.Nom}.";
                            }
                            else
                            {
                                MessageNiveauTextBoxCreateur.Foreground = Brushes.Red;
                                MessageNiveauTextBoxCreateur.Text = "Cette association créateur-niveau n'existe pas.";
                            }
                        }
                    }
                }
            }
        }
    }
}
