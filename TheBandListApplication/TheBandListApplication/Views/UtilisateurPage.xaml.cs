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
            UtilisateursListBox.ItemsSource = null;
            UtilisateursListBox.ItemsSource = Utilisateur;
        }


        private void AjouterUnUtilisateurClick(object sender, RoutedEventArgs e)
        {
            string nomUtilisateur = NomUtilisateurTextBox.Text.Trim().ToLower();

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
    }
}
