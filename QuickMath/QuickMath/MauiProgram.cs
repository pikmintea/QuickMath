using Microsoft.Extensions.Logging;
using QuickMath.Services;
using QuickMath.Shared.Services;

namespace QuickMath
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
                });

            // Add device-specific services used by the QuickMath.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#if ANDROID
            Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
#endif
#endif

            return builder.Build();
        }
    }
}
