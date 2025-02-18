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
                });
            builder.Services.AddTransient<ExpensePage>();
            builder.Services.AddTransient<UserPage>();
            builder.Services.AddTransient<AccountPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
