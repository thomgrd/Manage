// OnboardingPage1.xaml.cs
using Microsoft.Maui.Controls;
using System;

namespace ProjectManagerApp.Pages
{
    public partial class OnboardingPage1 : ContentPage
    {
        public OnboardingPage1()
        {
            InitializeComponent();
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            // Naviguer vers la deuxième page d'onboarding
            await Navigation.PushAsync(new OnboardingPage2());
        }
    }
}