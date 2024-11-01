using Microsoft.Maui.Controls;

namespace ProjectManagerApp
{
    public partial class HeaderView : ContentView
    {
        public HeaderView()
        {
            InitializeComponent();
        }
        private async void OnLogoTapped(object sender, EventArgs e)
        {
            // Navigation vers la MainPage
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }
    }
}
