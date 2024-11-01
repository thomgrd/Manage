// OnboardingPage3.xaml.cs
using Microsoft.Maui.Controls;
using System;

namespace ProjectManagerApp.Pages
{
    public partial class OnboardingPage3 : ContentPage
    {
        public OnboardingPage3()
        {
            InitializeComponent();
        }

        private async void OnFinishButtonClicked(object sender, EventArgs e)
        {
            // Marquer le tutoriel comme terminé
            Preferences.Set("HasSeenOnboarding", true);

            // Naviguer vers la page principale
            await Navigation.PushAsync(new MainPage());

            // Supprimer les pages de tutoriel de la pile de navigation
            Navigation.RemovePage(this);
        }

    }
}