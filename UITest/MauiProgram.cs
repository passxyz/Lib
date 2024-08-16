using Microsoft.Extensions.Logging;
using UITest.Models;
using UITest.Services;
using UITest.ViewModels;
using UITest.Views;

namespace UITest
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
                    fonts.AddFont("fa-regular-400.ttf", "FontAwesomeRegular");
                    fonts.AddFont("fa-solid-900.ttf", "FontAwesomeSolid");
                    fonts.AddFont("fa-brands-400.ttf", "FontAwesomeBrands");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
    		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif
            builder.Services.AddScoped<ItemsViewModel>();
            builder.Services.AddScoped<ItemsPage>();

            return builder.Build();
        }
    }
}
