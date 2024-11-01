namespace ProjectManagerApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Créer une vue de titre personnalisée
            var titleLabel = new Label
            {
                Text = "Projets",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Colors.White,  // Ajustez la couleur ici
                FontSize = 20              // Ajustez la taille ici
            };

            // Appliquer la vue de titre personnalisée
            NavigationPage.SetTitleView(this, titleLabel);
        }

    }
}
