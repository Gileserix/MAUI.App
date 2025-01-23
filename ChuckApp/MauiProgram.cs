using System.Net.Http;
using ChuckApp;
using Microsoft.Extensions.Logging;

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
            });

        // Configurar HttpClient
        builder.Services.AddHttpClient("ChuckApiClient", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7216/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Registrar MainPage con HttpClient inyectado
        builder.Services.AddTransient<MainPage>(sp =>
        {
            var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("ChuckApiClient");
            return new MainPage(httpClient);
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}