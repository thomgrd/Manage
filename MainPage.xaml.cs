using Microsoft.Maui.Storage;
using System;
using Microsoft.Maui.Controls;

namespace ProjectManagerApp
{
    public partial class MainPage : ContentPage
    {
        private const string CompanyNameKey = "CompanyName";

        public MainPage()
        {
            InitializeComponent();
            CheckAndLoadCompanyName();
        }

        private void CheckAndLoadCompanyName()
        {
            // Vérifier si le nom de l'entreprise est déjà enregistré
            if (Preferences.ContainsKey(CompanyNameKey))
            {
                // Si le nom de l'entreprise est enregistré, on l'affiche et on masque le formulaire
                string companyName = Preferences.Get(CompanyNameKey, string.Empty);
                CompanyNameLabel.Text = companyName;
                CompanyForm.IsVisible = false;
                MainContent.IsVisible = true;
            }
            else
            {
                // Sinon, on affiche le formulaire d'enregistrement
                CompanyForm.IsVisible = true;
                MainContent.IsVisible = false;
            }
        }

        private void OnSaveCompanyNameClicked(object sender, EventArgs e)
        {
            // Enregistrer le nom de l'entreprise
            string companyName = CompanyNameEntry.Text;
            if (!string.IsNullOrEmpty(companyName))
            {
                Preferences.Set(CompanyNameKey, companyName);
                CompanyNameLabel.Text = companyName;
                CompanyForm.IsVisible = false;
                MainContent.IsVisible = true;
                AnimateButton(); // Lancer l'animation après l'enregistrement du nom
            }
            else
            {
                DisplayAlert("Erreur", "Veuillez entrer un nom d'entreprise valide.", "OK");
            }
        }

        private void OnNavigateToProjetPageClicked(object sender, EventArgs e)
        {
            // Naviguer vers la page des projets
            Navigation.PushAsync(new Pages.ProjetsPage());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await AnimatePageAppearance(); // Animation douce à l'ouverture
            AnimateLogo();
        }

        private async void AnimateLogo()
        {
            if (LogoImage != null)
            {
                // Animation du logo plus rapide (200 ms)
                await LogoImage.ScaleTo(1.1, 200, Easing.CubicInOut);
                await LogoImage.ScaleTo(1, 200, Easing.CubicInOut);
            }
        }

        private async void AnimateButton()
        {
            // Animation du bouton Projets (apparition progressive plus rapide)
            if (ProjectsButton != null)
            {
                ProjectsButton.Opacity = 0;
                await ProjectsButton.FadeTo(1, 400, Easing.CubicInOut);
            }
        }

        private async System.Threading.Tasks.Task AnimatePageAppearance()
        {
            // Rendre la page transparente avant de commencer l'animation
            this.Opacity = 0;
            // Appliquer une animation de fondu pour apparaître progressivement (700 ms)
            await this.FadeTo(1, 700, Easing.CubicInOut);
        }
    }
}
