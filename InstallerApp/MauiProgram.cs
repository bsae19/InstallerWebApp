using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using NLog;

namespace InstallerApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
.UseMauiCommunityToolkit()
.ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FontAwesome-Brand.otf", "FontAwesome");
                fonts.AddFont("FontAwesome-Regular.otf", "FontAwesome2");
                fonts.AddFont("FontAwesome-Solid.otf", "FontAwesome3");
            });
        LogManager.Setup().LoadConfigurationFromFile("nlog.config");

        // Create and configure global loggers

        //#if DEBUG
        //		builder.Logging.AddDebug();
        //#endif

        return builder.Build();
    }
}
