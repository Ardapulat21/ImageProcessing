using ImageProcessing.Helper;
using ImageProcessing.Services.IO;
using System.ComponentModel;
using System.Security.Principal;
using System.Windows;
using System.Windows.Threading;

namespace ImageProcessing.ViewModels
{
    public class SplashScreenViewModel : INotifyPropertyChanged
    {
        private Visibility _visibility = Visibility.Hidden;
        public Visibility Visibility { 
            get => _visibility; 
            set
            {
                _visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }
        private string _loadingText = "Loading..";
        public string LoadingText 
        { 
            get => _loadingText;
            set
            {
                _loadingText = value;
                OnPropertyChanged(nameof(LoadingText));
            }
        }
        private double _progressValue = 0;
        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
                LoadingText = $"Loading.. %{100 * _progressValue:n0}";
                if (value >= 0.99)
                    SplashScreenHelper.Hide();
            }
        }
        public void SetProgress(double value)
        {
            Dispatcher.CurrentDispatcher.Invoke(() => ProgressValue = value);
        }
        #region Singleton
        static SplashScreenViewModel _splashScreenViewModel;
        public static SplashScreenViewModel GetInstance()
        {
            if (_splashScreenViewModel == null)
                _splashScreenViewModel = new SplashScreenViewModel();
            return _splashScreenViewModel;
        }
        private SplashScreenViewModel()
        {

        }
        #endregion
        #region MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
    