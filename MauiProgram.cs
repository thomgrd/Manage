using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.LifecycleEvents;

namespace ProjectManagerApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                    // Police titre Projet
                    fonts.AddFont("PlayfairDisplay-Regular.ttf", "PlayfairRegular");
                    fonts.AddFont("PlayfairDisplay-Bold.ttf", "PlayfairBold");

                    // Déclaration des polices Lato pour les boutons
                    fonts.AddFont("Lato-Bold.ttf", "LatoBold");
                    fonts.AddFont("Lato-Regular.ttf", "LatoRegular");
                    fonts.AddFont("Lato-Light.ttf", "LatoLight");

                    // Déclaration liste projet 
                    fonts.AddFont("NunitoSans_10pt_Condensed-Bold.ttf", "NunitoCondensedBold");
                    fonts.AddFont("NunitoSans_10pt_Condensed-Regular.ttf", "NunitoCondensedRegular");
                    fonts.AddFont("NunitoSans_10pt_Condensed-Light.ttf", "NunitoCondensedLight");
                    fonts.AddFont("NunitoSans_10pt_Condensed-Italic.ttf", "NunitoCondensedItalic");
                    fonts.AddFont("NunitoSans_10pt_Condensed-SemiBold.ttf", "NunitoCondensedSemiBold");
                });

            // Configuration Android spécifique pour la StatusBar
            builder.ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android
                    .OnCreate((activity, bundle) =>
                    {
                        // Définir la couleur de la StatusBar
                        activity.Window.SetStatusBarColor(Android.Graphics.Color.Purple);
                    }));
#endif
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}