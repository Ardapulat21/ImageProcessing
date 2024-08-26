using ImageProcessing.Services;
using ImageProcessing.Services.IO;
using System.IO;
using System.Windows;

namespace ImageProcessing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LoggerService.LogFolderPath = Path.Combine(PathService.AppDataFolder,"Log");
            base.OnStartup(e);
        }
    }
}
