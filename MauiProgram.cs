using CommunityToolkit.Maui;
using Microcharts.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;
using DentalApp.Pages;
using Microsoft.Extensions.Logging;

namespace DentalApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()  //For Toolkit
                .UseMicrocharts()           //For Microchart
                .UseSkiaSharp()            // <-- Add this line to register SkiaSharp

                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fa-solid-900.ttf", "FASolid");
                    fonts.AddFont("fa-regular-400.ttf", "FARegular");
                    fonts.AddFont("fa-brands-400.ttf", "FABrands");
                });
                
            builder.Services.AddTransient<ExpensePage>();
            builder.Services.AddTransient<UserListPage>();
            builder.Services.AddTransient<UserDetailsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
