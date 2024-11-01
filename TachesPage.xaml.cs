using ProjectManagerApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace ProjectManagerApp.Pages
{
    public partial class TachesPage : ContentPage
    {
        public Projet Projet { get; set; }
        public ObservableCollection<Tache> Taches { get; set; } = new ObservableCollection<Tache>();
        private bool _afficherTachesTerminees = false;

        public TachesPage(Projet projet)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Projet = projet;
            Title = $"Tâches pour {Projet.Nom}";
            NomProjet.Text = Projet.Nom;
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTaches();
            AnimateButtons();
        }

        public async Task LoadTaches()
        {
            var tachesListe = await App.Database.GetTachesAsync(Projet.Id, _afficherTachesTerminees);
            Taches.Clear();

            foreach (var tache in tachesListe)
            {
                Taches.Add(tache);
            }
        }

        private void AnimateButtons()
        {
            var buttons = new[] { AjouterTacheButton, AfficherTermineesButton };
            foreach (var button in buttons)
            {
                button.Opacity = 0;
                button.TranslateTo(0, 0, 500, Easing.CubicOut);
                button.FadeTo(1, 500, Easing.CubicInOut);
            }
        }

        private async void OnAfficherTermineesClicked(object sender, EventArgs e)
        {
            _afficherTachesTerminees = !_afficherTachesTerminees;
            var bouton = sender as Button;
            bouton.Text = _afficherTachesTerminees ? "Masquer Tâches Terminées" : "Afficher Tâches Terminées";
            bouton.BackgroundColor = _afficherTachesTerminees ? Colors.Green : Colors.Gray;

            await LoadTaches();
        }

        private async void OnTaskNameTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Tache tache)
            {
                tache.IsSelected = true;
                await DisplayAlert("Description", tache.Description, "OK");
                tache.IsSelected = false;
            }
        }

        private async void OnAjouterTacheClicked(object sender, EventArgs e)
        {
            string titre = await DisplayPromptAsync("Nouvelle Tâche", "Entrez le titre de la tâche :");
            string description = await DisplayPromptAsync("Description", "Entrez une description pour cette tâche :");

            if (!string.IsNullOrWhiteSpace(titre))
            {
                var nouvelleTache = new Tache
                {
                    ProjetId = Projet.Id,
                    Nom = titre,
                    Description = description,
                    EstTerminee = false,
                    DateEcheance = DateTime.Now.AddDays(7)
                };

                await App.Database.SaveTacheAsync(nouvelleTache);
                Taches.Add(nouvelleTache);
            }
        }

        private async void OnSupprimerTacheClicked(object sender, EventArgs e)
        {
            var bouton = sender as ImageButton;
            var tache = bouton?.CommandParameter as Tache;

            if (tache != null)
            {
                bool confirmation = await DisplayAlert("Confirmer la suppression", $"Voulez-vous supprimer la tâche '{tache.Nom}' ?", "Oui", "Non");
                if (confirmation)
                {
                    await App.Database.DeleteTacheAsync(tache);
                    Taches.Remove(tache);
                }
            }
        }

        private async void OnRetourProjetClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var tache = checkBox?.BindingContext as Tache;

            if (tache != null)
            {
                tache.EstTerminee = e.Value;
                await App.Database.SaveTacheAsync(tache);

                if (!_afficherTachesTerminees && tache.EstTerminee)
                {
                    Taches.Remove(tache);
                }
                else if (_afficherTachesTerminees && !tache.EstTerminee)
                {
                    Taches.Remove(tache);
                }
            }
        }
    }
}