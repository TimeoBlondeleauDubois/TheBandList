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
    /// Logique d'interaction pour DifficultePage.xaml
    /// </summary>
    public partial class DifficultePage : Page
    {
        private NiveauDifficulteRate DifficulteSelectionnee;
        private DifficulteFeature FeatureSelectionnee;
        private List<NiveauDifficulteRate> ListeNiveauxDifficulte;
        private List<DifficulteFeature> ListeFeatures;

        public DifficultePage()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private void DifficultePageLoaded(object sender, RoutedEventArgs e)
        {
            ChargerDifficulte();
            ChargerFeatures();
            MessageTextBox.Text = "";
            MessageFeatureTextBox.Text = "";
            ChargerDifficultesDansComboBox();
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
            string nomDifficulte = NomDifficulteTextBox.Text.Trim();

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
            ChargerDifficultesDansComboBox();
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
                ChargerDifficultesDansComboBox();
            }
        }

        private void ConfirmerModificationClick(object sender, RoutedEventArgs e)
        {
            if (DifficulteSelectionnee != null)
            {
                string nouveauNom = NomDifficulteTextBox.Text.Trim();

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
                    ChargerDifficultesDansComboBox();
                    ChargerFeatures();
                }
            }
            else
            {
                MessageTextBox.Foreground = Brushes.Red;
                MessageTextBox.Text = "Veuillez sélectionner une difficulté à supprimer.";
            }
        }

        private void ChargerFeatures()
        {
            using (var context = new TheBandListDbContext())
            {
                ListeFeatures = context.DifficulteFeatures.ToList();

                ListeFeatures.Insert(0, new DifficulteFeature { Id = 0, NomDuFeature = "Aucune feature sélectionnée" });

                FeaturesComboBox.ItemsSource = ListeFeatures;
                FeaturesComboBox.SelectedIndex = 0;
            }
        }

        private void ChargerDifficultesDansComboBox()
        {
            using (var context = new TheBandListDbContext())
            {
                var difficulteList = context.NiveauxDifficulteRates.ToList();
                difficulteList.Insert(0, new NiveauDifficulteRate { Id = 0, NomDeLaDifficulte = "Aucune difficulté" });
                DifficulteComboBox.ItemsSource = difficulteList;
                DifficulteComboBox.SelectedIndex = 0;
            }
        }

        private void AjouterFeatureClick(object sender, RoutedEventArgs e)
        {
            string nomFeature = NomFeatureTextBox.Text.Trim();
            var difficulteSelectionnee = DifficulteComboBox.SelectedItem as NiveauDifficulteRate;

            if (string.IsNullOrEmpty(nomFeature))
            {
                MessageFeatureTextBox.Foreground = Brushes.Red;
                MessageFeatureTextBox.Text = "Veuillez entrer un nom pour le feature.";
                return;
            }

            if (difficulteSelectionnee == null || difficulteSelectionnee.Id == 0)
            {
                MessageFeatureTextBox.Foreground = Brushes.Red;
                MessageFeatureTextBox.Text = "Veuillez sélectionner une difficulté valide.";
                return;
            }

            if (string.IsNullOrEmpty(imageBase64))
            {
                MessageFeatureTextBox.Foreground = Brushes.Red;
                MessageFeatureTextBox.Text = "Veuillez sélectionner une image.";
                return;
            }

            using (var context = new TheBandListDbContext())
            {
                var nouveauFeature = new DifficulteFeature
                {
                    NomDuFeature = nomFeature,
                    Image = imageBase64,
                    DifficulteRateId = difficulteSelectionnee.Id
                };
                context.DifficulteFeatures.Add(nouveauFeature);
                context.SaveChanges();
            }

            MessageFeatureTextBox.Foreground = Brushes.Green;
            MessageFeatureTextBox.Text = $"Feature '{nomFeature}' ajouté avec succès !";

            ResetFormulaire();
            ChargerFeatures();
            imageBase64 = null;
            DifficulteComboBox.SelectedIndex = 0;
            FeaturesComboBox.SelectedIndex = 0;
        }

        private void ConfirmerModificationFeatureClick(object sender, RoutedEventArgs e)
        {
            if (FeatureSelectionnee != null)
            {
                string nouveauNom = NomFeatureTextBox.Text.Trim();
                var difficulteSelectionnee = DifficulteComboBox.SelectedItem as NiveauDifficulteRate;

                if (string.IsNullOrEmpty(nouveauNom))
                {
                    MessageFeatureTextBox.Foreground = Brushes.Red;
                    MessageFeatureTextBox.Text = "Le nom du feature ne peut pas être vide.";
                    return;
                }

                if (difficulteSelectionnee == null || difficulteSelectionnee.Id == 0)
                {
                    MessageFeatureTextBox.Foreground = Brushes.Red;
                    MessageFeatureTextBox.Text = "Veuillez sélectionner une difficulté valide.";
                    return;
                }

                using (var context = new TheBandListDbContext())
                {
                    var featureDb = context.DifficulteFeatures
                        .FirstOrDefault(f => f.Id == FeatureSelectionnee.Id);

                    if (featureDb != null)
                    {
                        featureDb.NomDuFeature = nouveauNom;
                        featureDb.Image = imageBase64 ?? featureDb.Image;
                        featureDb.DifficulteRateId = difficulteSelectionnee.Id;

                        context.SaveChanges();

                        MessageFeatureTextBox.Foreground = Brushes.Green;
                        MessageFeatureTextBox.Text = $"Feature '{FeatureSelectionnee.NomDuFeature}' modifié avec succès.";
                    }
                    else
                    {
                        MessageFeatureTextBox.Foreground = Brushes.Red;
                        MessageFeatureTextBox.Text = "Erreur : le feature n'existe plus dans la base de données.";
                    }
                }

                ResetFormulaire();
                ChargerFeatures();
                AjouterFeatureButton.Visibility = Visibility.Visible;
                ModifierFeatureButton.Visibility = Visibility.Collapsed;
                AnnulerFeatureModificationButton.Visibility = Visibility.Collapsed;
                SupprimerFeatureButton.Visibility = Visibility.Collapsed;
                imageBase64 = null;
                DifficulteComboBox.SelectedIndex = 0;
                FeaturesComboBox.SelectedIndex = 0;
            }
        }

        private void AnnulerFeatureModificationClick(object sender, RoutedEventArgs e)
        {
            FeatureSelectionnee = null;
            NomFeatureTextBox.Text = string.Empty;
            SelectedImagePreview.Source = null;
            SelectedImagePreview.Visibility = Visibility.Collapsed;

            AjouterFeatureButton.Visibility = Visibility.Visible;
            ModifierFeatureButton.Visibility = Visibility.Collapsed;
            AnnulerFeatureModificationButton.Visibility = Visibility.Collapsed;
            SupprimerFeatureButton.Visibility = Visibility.Collapsed;
            MessageFeatureTextBox.Text = string.Empty;
            imageBase64 = null;
            DifficulteComboBox.SelectedIndex = 0;
            FeaturesComboBox.SelectedIndex = 0;
        }

        private string imageBase64 = null;

        private void ImageSelectionButtonClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Images (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;

                byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                imageBase64 = Convert.ToBase64String(imageBytes);

                SelectedImagePreview.Source = new BitmapImage(new Uri(filePath));
                SelectedImagePreview.Visibility = Visibility.Visible;
            }
        }

        private void FeaturesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FeaturesComboBox.SelectedItem is DifficulteFeature feature && feature.Id != 0)
            {
                FeatureSelectionnee = feature;

                NomFeatureTextBox.Text = feature.NomDuFeature;

                if (!string.IsNullOrEmpty(feature.Image))
                {
                    byte[] imageBytes = Convert.FromBase64String(feature.Image);
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        SelectedImagePreview.Source = bitmap;
                        SelectedImagePreview.Visibility = Visibility.Visible;
                    }
                    imageBase64 = feature.Image;
                }
                else
                {
                    SelectedImagePreview.Source = null;
                    SelectedImagePreview.Visibility = Visibility.Collapsed;
                    imageBase64 = null;
                }

                DifficulteComboBox.SelectedItem = DifficulteComboBox.Items
                    .Cast<NiveauDifficulteRate>()
                    .FirstOrDefault(d => d.Id == feature.DifficulteRateId);

                AjouterFeatureButton.Visibility = Visibility.Collapsed;
                ModifierFeatureButton.Visibility = Visibility.Visible;
                SupprimerFeatureButton.Visibility = Visibility.Visible;
                AnnulerFeatureModificationButton.Visibility = Visibility.Visible;
            }
            else
            {
                FeatureSelectionnee = null;
                ResetFormulaire();
                AjouterFeatureButton.Visibility = Visibility.Visible;
                ModifierFeatureButton.Visibility = Visibility.Collapsed;
                SupprimerFeatureButton.Visibility = Visibility.Collapsed;
                AnnulerFeatureModificationButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SupprimerFeatureClick(object sender, RoutedEventArgs e)
        {
            if (FeatureSelectionnee != null)
            {
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer le feature '{FeatureSelectionnee.NomDuFeature}' ?",
                    "Confirmation de suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new TheBandListDbContext())
                    {
                        var featureDb = context.DifficulteFeatures
                            .FirstOrDefault(f => f.Id == FeatureSelectionnee.Id);

                        if (featureDb != null)
                        {
                            context.DifficulteFeatures.Remove(featureDb);
                            context.SaveChanges();

                            MessageFeatureTextBox.Foreground = Brushes.Green;
                            MessageFeatureTextBox.Text = $"Feature '{FeatureSelectionnee.NomDuFeature}' supprimée avec succès.";
                        }
                        else
                        {
                            MessageFeatureTextBox.Foreground = Brushes.Red;
                            MessageFeatureTextBox.Text = "Erreur : le feature n'existe plus dans la base de données.";
                        }
                    }

                    ResetFormulaire();
                    ChargerFeatures();
                    AjouterFeatureButton.Visibility = Visibility.Visible;
                    ModifierFeatureButton.Visibility = Visibility.Collapsed;
                    AnnulerFeatureModificationButton.Visibility = Visibility.Collapsed;
                    SupprimerFeatureButton.Visibility = Visibility.Collapsed;
                    imageBase64 = null;
                    DifficulteComboBox.SelectedIndex = 0;
                    FeaturesComboBox.SelectedIndex = 0;
                }
            }
        }

        private void ResetFormulaire()
        {
            NomFeatureTextBox.Text = string.Empty;
            SelectedImagePreview.Source = null;
            SelectedImagePreview.Visibility = Visibility.Collapsed;
            imageBase64 = null;
            DifficulteComboBox.SelectedIndex = 0;
            MessageFeatureTextBox.Text = string.Empty;
        }
    }
}