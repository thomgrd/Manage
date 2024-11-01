using ProjectManagerApp.Services;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using ProjectManagerApp.Pages;

namespace ProjectManagerApp
{
    public partial class App : Application
    {
        // Instance du service de base de données
        public static DatabaseService Database { get; private set; }

        public App()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            // Instancier le service de base de données
            Database = new DatabaseService();

            // Initialiser la base de données
            InitializeAsync();

            if (Preferences.Get("HasSeenOnboarding", false))
            {
                MainPage = new NavigationPage(new MainPage())
                {
                    BarBackgroundColor = Color.FromArgb("#6200EE"),
                    BarTextColor = Colors.White,
                };
            }
            else
            {
                MainPage = new NavigationPage(new OnboardingPage1())
                {
                    BarBackgroundColor = Color.FromArgb("#6200EE"),
                    BarTextColor = Colors.White,
                };
            }

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                // Gérer les exceptions non prises en charge ici
                var exception = e.ExceptionObject as Exception;
                Console.WriteLine(exception?.Message);
            };
        }



        private async void InitializeAsync()
        {
            // Attendre l'initialisation de la base de données
            await Database.InitializeDatabaseAsync();
        }
    }
}