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

        // G�rer la s�lection d'un projet depuis le Picker
        private void OnProjectSelected(object sender, EventArgs e)
        {
            if (ProjectPicker.SelectedItem is Projet selectedProjet)
            {
                SelectedProject = selectedProjet;
                UpdateFormFields(selectedProjet);
            }
        }

        // M�thode pour mettre � jour les champs du formulaire en fonction du projet s�lectionn�
        private void UpdateFormFields(Projet projet)
        {
            EditProjectName.Text = projet.Nom;
            EditProjectDescription.Text = projet.Description;
            EditProjectClient.Text = projet.Client;

            // Afficher les sections du formulaire directement apr�s la s�lection
            PickerSection.IsVisible = false;
            TabsSection.IsVisible = true;
            FormSection.IsVisible = true;
            ValidateButton.IsVisible = true;

            // Par d�faut, afficher la section Nom
            SetActiveSection(NomButton);
        }

        // D�finir la section active et mettre � jour l'apparence
        private void SetActiveSection(Button activeButton)
        {
            // R�initialiser l'apparence de tous les boutons
            NomButton.BackgroundColor = Colors.LightGray;
            ClientButton.BackgroundColor = Colors.LightGray;
            DescriptionButton.BackgroundColor = Colors.LightGray;

            // D�finir le bouton actif avec une couleur sp�cifique
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

        // G�rer le clic sur le bouton "Nom"
        private void OnNomButtonClicked(object sender, EventArgs e)
        {
            SetActiveSection(NomButton);
        }

        // G�rer le clic sur le bouton "Client"
        private void OnClientButtonClicked(object sender, EventArgs e)
        {
            SetActiveSection(ClientButton);
        }

        // G�rer le clic sur le bouton "Description"
        private void OnDescriptionButtonClicked(object sender, EventArgs e)
        {
            SetActiveSection(DescriptionButton);
        }

        // Valider les modifications du projet
        private async void OnValidateEditClicked(object sender, EventArgs e)
        {
            if (SelectedProject != null)
            {
                // Mettre � jour les informations du projet avec les nouvelles valeurs des champs
                SelectedProject.Nom = EditProjectName.Text;
                SelectedProject.Client = EditProjectClient.Text;
                SelectedProject.Description = EditProjectDescription.Text;

                // Sauvegarder les modifications dans la base de donn�es
                await App.Database.SaveProjetAsync(SelectedProject);

                // Naviguer vers la page pr�c�dente apr�s la validation
                await Navigation.PopAsync();
            }
        }
    }
}
