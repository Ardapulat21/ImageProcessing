using ImageProcessing.ViewModels;
using Microsoft.VisualBasic;
using System;
using SplashScreen = ImageProcessing.UserControls.SplashScreen;
namespace ImageProcessing.Helper
{
    public class SplashScreenHelper
    {
        static SplashScreenViewModel _splashScreenViewModel = SplashScreenViewModel.GetInstance();
        public static void Show()
        {
            _splashScreenViewModel.MakeVisible();
        }
        public static void Hide()
        {
            _splashScreenViewModel.MakeHidden();
        }
        public static void Progress(double value)
        {
            Show();
            _splashScreenViewModel.SetProgress(value);
        }
    }
}
