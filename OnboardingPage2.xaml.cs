// OnboardingPage2.xaml.cs
using Microsoft.Maui.Controls;
using System;

namespace ProjectManagerApp.Pages
{
    public partial class OnboardingPage2 : ContentPage
    {
        public OnboardingPage2()
        {
            InitializeComponent();
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            // Naviguer vers la troisième page d'onboarding
            await Navigation.PushAsync(new OnboardingPage3());
        }
    }
}