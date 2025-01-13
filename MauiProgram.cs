using DotnetCoursework.Model;
using ExpenseTrackerApp.Services; // Make sure to include the correct namespace for your services
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace DotnetCourseowork
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

            builder.Services.AddMauiBlazorWebView();
            

            // Add MudBlazor services
            builder.Services.AddMudServices();
           
            // Console.WriteLine("all expenses:"+expenseService.GetAllExpenses());
                

            // Register ExpenseService as a Singleton or Transient
            builder.Services.AddSingleton<ExpenseService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddScoped<UserContext>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}