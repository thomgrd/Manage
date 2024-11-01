// Code-behind de la page des projets
using ProjectManagerApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MauiButton = Microsoft.Maui.Controls.Button;
using MauiCheckBox = Microsoft.Maui.Controls.CheckBox;
using MauiContentPage = Microsoft.Maui.Controls.ContentPage;
using MauiImageButton = Microsoft.Maui.Controls.ImageButton;
using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Windows.Input;
using ProjectManagerApp.Helpers;

namespace ProjectManagerApp.Pages
{
    public partial class ProjetsPage : MauiContentPage
    {
        public ObservableCollection<Projet> Projets { get; set; } = new ObservableCollection<Projet>();
        private bool _afficherProjetsTermines = false;  // Indicateur pour afficher ou masquer les projets terminés
        public DragAndDropHandler _dragAndDropHandler; // Instance de DragAndDropHandler
        public Projet SelectedProject { get; set; } // Projet sélectionné pour l'édition

        public ProjetsPage()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);

            // Initialisation du DragAndDropHandler
            _dragAndDropHandler = new DragAndDropHandler(Projets);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = this;
            await LoadProjets();  // Charger les projets
            AnimateButtons(); // Ajouter l'animation aux boutons lors de l'affichage de la page
        }

        public async Task LoadProjets()
        {
            var projetsListe = await App.Database.GetProjetsAsync(_afficherProjetsTermines);

            // Trier les projets par la propriété Ordre avant de les ajouter à la collection
            var projetsTries = projetsListe.OrderBy(p => p.Ordre).ToList();

            Projets.Clear();  // Vider la collection observable

            foreach (var projet in projetsTries)
            {
                Projets.Add(projet);
            }
        }



        // Ajouter une animation aux boutons lors de l'affichage de la page
        private void AnimateButtons()
        {
            var buttons = new[] { AjouterProjetButton, AfficherProjetsTermines };
            foreach (var button in buttons)
            {
                button.Opacity = 0;
                button.TranslateTo(0, 0, 500, Easing.CubicOut);
                button.FadeTo(1, 500, Easing.CubicInOut);
            }
        }

        // Gérer la sélection d'un projet
        // Gérer la sélection d'un projet - Rediriger vers la page des tâches
        private async void OnProjectNameTapped(object sender, EventArgs e)
        {
            var label = sender as Label;
            var projet = label?.BindingContext as Projet;

            if (projet != null)
            {
                // Réinitialiser tous les autres projets pour qu'ils ne soient plus sélectionnés
                foreach (var p in Projets)
                {
                    p.IsSelected = false;
                }

                // Marquer le projet comme sélectionné
                projet.IsSelected = true;

                // Naviguer vers la page des tâches du projet
                await Navigation.PushAsync(new TachesPage(projet));
            }
        }



        // Ajouter un nouveau projet
        private async void OnAjouterProjetClicked(object sender, EventArgs e)
        {
            string titre = await DisplayPromptAsync("Nouveau Projet", "Entrez le titre du projet :");
            string description = await DisplayPromptAsync("Description", "Entrez une description pour ce projet :");
            string client = await DisplayPromptAsync("Nom du Client", "Entrez le nom du client (optionnel) :");

            if (!string.IsNullOrWhiteSpace(titre))
            {
                var nouveauProjet = new Projet
                {
                    Nom = titre,
                    Description = description,
                    Client = client,
                    EstTermine = false,
                    DateEcheance = DateTime.Now.AddDays(30)
                };

                await App.Database.SaveProjetAsync(nouveauProjet);
                Projets.Add(nouveauProjet);  // Ajouter le nouveau projet à la liste
            }
        }

        // Supprimer un projet
        private async void OnSupprimerProjetClicked(object sender, EventArgs e)
        {
            var bouton = sender as MauiImageButton;
            var projet = bouton?.CommandParameter as Projet;

            if (projet != null)
            {
                bool confirmation = await DisplayAlert("Confirmer la suppression", $"Voulez-vous supprimer le projet '{projet.Nom}' ?", "Oui", "Non");
                if (confirmation)
                {
                    await App.Database.DeleteProjetAsync(projet);
                    Projets.Remove(projet);  // Supprimer de la collection observable
                }
            }
        }

        // Basculer entre les projets terminés et non terminés
        private async void OnAfficherTerminesClicked(object sender, EventArgs e)
        {
            _afficherProjetsTermines = !_afficherProjetsTermines;
            var bouton = sender as MauiButton;
            bouton.Text = _afficherProjetsTermines ? "Masquer Projets Terminés" : "Afficher Projets Terminés";
            bouton.BackgroundColor = _afficherProjetsTermines ? Colors.Green : Colors.Gray;

            await LoadProjets();
        }

        // Gérer la case à cocher pour marquer un projet comme terminé ou non terminé
        private async void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = sender as MauiCheckBox;
            var projet = checkBox?.BindingContext as Projet;

            if (projet != null)
            {
                projet.EstTermine = e.Value;  // Mettre à jour la propriété dans l'objet
                await App.Database.SaveProjetAsync(projet);  // Sauvegarder l'état du projet

                // Si on n'affiche pas les projets terminés et que le projet est marqué comme terminé, le retirer de la liste
                if (!_afficherProjetsTermines && projet.EstTermine)
                {
                    Projets.Remove(projet);  // Retirer de l'affichage
                }

                // Si on affiche uniquement les projets terminés et qu'un projet est marqué comme non terminé, le retirer de la liste
                if (_afficherProjetsTermines && !projet.EstTermine)
                {
                    Projets.Remove(projet);
                }
            }
        }

        // Recherche de projets dans la barre de recherche
        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue;
            ProjetsListView.ItemsSource = Projets
                .Where(p => p.Nom.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Gérer l'affichage de la barre de recherche au clic sur l'icône
        private void OnSearchIconClicked(object sender, EventArgs e)
        {
            // Toggle the visibility of the SearchBar
            if (CustomSearchBar.IsVisible)
            {
                // Cacher la SearchBar et restaurer l'icône de recherche
                CustomSearchBar.IsVisible = false;
                SearchToggleIcon.Source = "icon_search.svg"; // Restaurer l'icône
            }
            else
            {
                // Afficher la SearchBar et garder l'icône visible à gauche
                CustomSearchBar.IsVisible = true;
            }
        }

        // Si la recherche est effectuée et validée, on peut aussi cacher la barre
        private void OnSearchBarClosed(object sender, EventArgs e)
        {
            CustomSearchBar.IsVisible = false;
            SearchToggleIcon.IsVisible = true;
        }

        // Propriétés pour les commandes de Drag and Drop depuis _dragAndDropHandler
        public ICommand ItemDragged => _dragAndDropHandler.ItemDragged;
        public ICommand ItemDraggedOver => _dragAndDropHandler.ItemDraggedOver;
        public ICommand ItemDragLeave => _dragAndDropHandler.ItemDragLeave;
        public ICommand ItemDropped => _dragAndDropHandler.ItemDropped;

        // Gérer le clic sur l'icône d'édition

        public async void OnEditionIconClicked(object sender, EventArgs e)
        {
            // Naviguer vers la page d'édition avec tous les projets, sans spécifier de projet sélectionné directement
            await Navigation.PushAsync(new EditProjetPage(Projets, null));
        }


    }
}
