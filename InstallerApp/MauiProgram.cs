using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

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

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
