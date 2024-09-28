using ImageProcessing.ViewModels;
using System.Windows.Controls;
namespace ImageProcessing.UserControls
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : UserControl
    {
        SplashScreenViewModel _splashScreenViewModel = SplashScreenViewModel.GetInstance();
        public SplashScreen()
        {
            InitializeComponent();
            DataContext = _splashScreenViewModel;
        }
    }
}
