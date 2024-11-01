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
        private bool _afficherProjetsTermines = false;  // Indicateur pour afficher ou masquer les projets termin�s
        public DragAndDropHandler _dragAndDropHandler; // Instance de DragAndDropHandler
        public Projet SelectedProject { get; set; } // Projet s�lectionn� pour l'�dition

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

            // Trier les projets par la propri�t� Ordre avant de les ajouter � la collection
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

        // G�rer la s�lection d'un projet
        // G�rer la s�lection d'un projet - Rediriger vers la page des t�ches
        private async void OnProjectNameTapped(object sender, EventArgs e)
        {
            var label = sender as Label;
            var projet = label?.BindingContext as Projet;

            if (projet != null)
            {
                // R�initialiser tous les autres projets pour qu'ils ne soient plus s�lectionn�s
                foreach (var p in Projets)
                {
                    p.IsSelected = false;
                }

                // Marquer le projet comme s�lectionn�
                projet.IsSelected = true;

                // Naviguer vers la page des t�ches du projet
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
                Projets.Add(nouveauProjet);  // Ajouter le nouveau projet � la liste
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

        // Basculer entre les projets termin�s et non termin�s
        private async void OnAfficherTerminesClicked(object sender, EventArgs e)
        {
            _afficherProjetsTermines = !_afficherProjetsTermines;
            var bouton = sender as MauiButton;
            bouton.Text = _afficherProjetsTermines ? "Masquer Projets Termin�s" : "Afficher Projets Termin�s";
            bouton.BackgroundColor = _afficherProjetsTermines ? Colors.Green : Colors.Gray;

            await LoadProjets();
        }

        // G�rer la case � cocher pour marquer un projet comme termin� ou non termin�
        private async void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = sender as MauiCheckBox;
            var projet = checkBox?.BindingContext as Projet;

            if (projet != null)
            {
                projet.EstTermine = e.Value;  // Mettre � jour la propri�t� dans l'objet
                await App.Database.SaveProjetAsync(projet);  // Sauvegarder l'�tat du projet

                // Si on n'affiche pas les projets termin�s et que le projet est marqu� comme termin�, le retirer de la liste
                if (!_afficherProjetsTermines && projet.EstTermine)
                {
                    Projets.Remove(projet);  // Retirer de l'affichage
                }

                // Si on affiche uniquement les projets termin�s et qu'un projet est marqu� comme non termin�, le retirer de la liste
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

        // G�rer l'affichage de la barre de recherche au clic sur l'ic�ne
        private void OnSearchIconClicked(object sender, EventArgs e)
        {
            // Toggle the visibility of the SearchBar
            if (CustomSearchBar.IsVisible)
            {
                // Cacher la SearchBar et restaurer l'ic�ne de recherche
                CustomSearchBar.IsVisible = false;
                SearchToggleIcon.Source = "icon_search.svg"; // Restaurer l'ic�ne
            }
            else
            {
                // Afficher la SearchBar et garder l'ic�ne visible � gauche
                CustomSearchBar.IsVisible = true;
            }
        }

        // Si la recherche est effectu�e et valid�e, on peut aussi cacher la barre
        private void OnSearchBarClosed(object sender, EventArgs e)
        {
            CustomSearchBar.IsVisible = false;
            SearchToggleIcon.IsVisible = true;
        }

        // Propri�t�s pour les commandes de Drag and Drop depuis _dragAndDropHandler
        public ICommand ItemDragged => _dragAndDropHandler.ItemDragged;
        public ICommand ItemDraggedOver => _dragAndDropHandler.ItemDraggedOver;
        public ICommand ItemDragLeave => _dragAndDropHandler.ItemDragLeave;
        public ICommand ItemDropped => _dragAndDropHandler.ItemDropped;

        // G�rer le clic sur l'ic�ne d'�dition

        public async void OnEditionIconClicked(object sender, EventArgs e)
        {
            // Naviguer vers la page d'�dition avec tous les projets, sans sp�cifier de projet s�lectionn� directement
            await Navigation.PushAsync(new EditProjetPage(Projets, null));
        }


    }
}
