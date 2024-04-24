using GoogleMessage.Services;
using GoogleMessage.ViewModels;
using GoogleMessage.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Windows;

namespace GoogleMessage
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; set; }
        public static IConfiguration Configuration { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Log.Logger = Serilogger.ConfigureLogger();

            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection()
                .AddScoped<IConfiguration>(config => Configuration)
                .AddLogging()
                .AddSerilog()
                .RegisterSerivces()
                .RegisterViewModels()
                .RegisterWindows();

            services.AddConfidentialFactory((option) =>
            {
                option.TenentId = "77b00969-8278-4fec-8997-1f3181fd089a";
                option.ApplicationId = "d89e0285-c44a-4e8d-b99d-dffb94ec11d9";
                option.ClientSecret = "4Y-8Q~KgZY.Xal76NyOGrXCdwv47tOShKwpb8cMP";
            });

            ServiceProvider = services.BuildServiceProvider();
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            App.Current.MainWindow = mainWindow;
            App.Current.MainWindow.Show();
        }                
    }

    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterWindows(this IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            return services;
        }

        public static IServiceCollection RegisterViewModels(this IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            return services;
        }

        public static IServiceCollection RegisterSerivces(this IServiceCollection services)
        {
            services.AddSingleton<IHtmlParseService, HtmlParseService>();
            services.AddSingleton<AutoForwardWorker>();
            return services;
        }
    }
}
