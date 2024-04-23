using GoogleMessage.Services;
using GoogleMessage.ViewModels;
using GoogleMessage.Windows;
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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Log.Logger = Serilogger.ConfigureLogger();

            var services = new ServiceCollection()
                .AddLogging()
                .AddSerilog()
                .RegisterSerivces()
                .RegisterViewModels()
                .RegisterWindows();

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
