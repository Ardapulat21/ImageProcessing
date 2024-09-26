using ImageProcessing.Models;
using ImageProcessing.ViewModels;
using Microsoft.VisualBasic;
using System;
using System.Threading.Tasks;
using SplashScreen = ImageProcessing.UserControls.SplashScreen;
namespace ImageProcessing.Helper
{
    public class SplashScreenHelper
    {
        static SplashScreenViewModel _splashScreenViewModel = SplashScreenViewModel.GetInstance();
        public static void Display()
        {
            _splashScreenViewModel.Visibility = System.Windows.Visibility.Visible;
        }
        public static void Hide()
        {
            _splashScreenViewModel.Visibility = System.Windows.Visibility.Hidden;
        }
        public static void SetProgress(double value)
        {
            _splashScreenViewModel.SetProgress(value);
        }
    }
}
