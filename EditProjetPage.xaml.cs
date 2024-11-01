using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    public partial class EditProjetPage : ContentPage
    {
        public ObservableCollection<Projet> Projets { get; set; }
        public Projet SelectedProject { get; set; }

        public EditProjetPage(ObservableCollection<Projet> projets, Projet selectedProject = null)
        {
            InitializeComponent();

            Projets = projets;
            SelectedProject = selectedProject;

            BindingContext = this;

            // Si un projet est fourni, afficher les informations dans les champs
            if (SelectedProject != null)
            {
                UpdateFormFields(SelectedProject);
            }
        }

        // Gérer la sélection d'un projet depuis le Picker
        private void OnProjectSelected(object sender, EventArgs e)
        {
            if (ProjectPicker.SelectedItem is Projet selectedProjet)
            {
                SelectedProject = selectedProjet;
                UpdateFormFields(selectedProjet);
            }
        }

        // Méthode pour mettre à jour les champs du formulaire en fonction du projet sélectionné
        private void UpdateFormFields(Projet projet)
        {
            EditProjectName.Text = projet.Nom;
            EditProjectDescription.Text = projet.Description;
            EditProjectClient.Text = projet.Client;

            // Afficher les sections du formulaire directement après la sélection
            PickerSection.IsVisible = false;
            TabsSection.IsVisible = true;
            FormSection.IsVisible = true;
            ValidateButton.IsVisible = true;

            // Par défaut, afficher la section Nom
            SetActiveSection(NomButton);
        }

        // Définir la section active et mettre à jour l'apparence
        private void SetActiveSection(Button activeButton)
        {
            // Réinitialiser l'apparence de tous les boutons
            NomButton.BackgroundColor = Colors.LightGray;
            ClientButton.BackgroundColor = Colors.LightGray;
            DescriptionButton.BackgroundColor = Colors.LightGray;

            // Définir le bouton actif avec une couleur spécifique
            activeButton.BackgroundColor = Colors.Salmon;

            // Masquer toutes les sections
            NomSection.IsVisible = false;
            ClientSection.IsVisible = false;
            DescriptionSection.IsVisible = false;

            // Afficher la section correspondante
            if (activeButton == NomButton)
            {
                NomSection.IsVisible = true;
            }
            else if (activeButton == ClientButton)
            {
                ClientSection.IsVisible = true;
            }
            else if (activeButton == DescriptionButton)
            {
                DescriptionSection.IsVisible = true;
            }
        }

        // Gérer le clic sur le bouton "Nom"
        private void OnNomButtonClicked(object sender, EventArgs e)
        {
            SetActiveSection(NomButton);
        }

        // Gérer le clic sur le bouton "Client"
        private void OnClientButtonClicked(object sender, EventArgs e)
        {
            SetActiveSection(ClientButton);
        }

        // Gérer le clic sur le bouton "Description"
        private void OnDescriptionButtonClicked(object sender, EventArgs e)
        {
            SetActiveSection(DescriptionButton);
        }

        // Valider les modifications du projet
        private async void OnValidateEditClicked(object sender, EventArgs e)
        {
            if (SelectedProject != null)
            {
                // Mettre à jour les informations du projet avec les nouvelles valeurs des champs
                SelectedProject.Nom = EditProjectName.Text;
                SelectedProject.Client = EditProjectClient.Text;
                SelectedProject.Description = EditProjectDescription.Text;

                // Sauvegarder les modifications dans la base de données
                await App.Database.SaveProjetAsync(SelectedProject);

                // Naviguer vers la page précédente après la validation
                await Navigation.PopAsync();
            }
        }
    }
}
