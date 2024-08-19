using FFMediaToolkit;
using ImageProcessing.Models;
using ImageProcessing.MVVM_Helper;
using ImageProcessing.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Decoder = ImageProcessing.Services.Decoder;
namespace ImageProcessing
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Bindings
        private string _playPause = "Play";
        public string PlayPause
        {
            get => _playPause;
            set
            {
                if (_playPause != value)
                {
                    _playPause = value;
                }
                OnPropertyChanged(nameof(PlayPause));
            }
        }

        private int _numberOfFrames;
        public int NumberOfFrames
        {
            get => _numberOfFrames;
            set
            {
                if (_numberOfFrames != value)
                {
                    _numberOfFrames = value;
                }
                OnPropertyChanged(nameof(NumberOfFrames));
            }
        }

        private int _sliderValue = 0;
        public int SliderValue
        {
            get => _sliderValue;
            set
            {
                if (_sliderValue != value)
                {
                    _sliderValue = value;
                }
                OnPropertyChanged(nameof(SliderValue));
            }
        }
        
        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                }
                OnPropertyChanged(nameof(ImageSource)); 
            }
        }
        #endregion

        #region Command
        public ICommand PlayPauseCommand { get; }
        public ICommand OpenFolderCommand { get; }
        #endregion

        VideoProcessing Video;
        Decoder decoder;
        Renderer renderer;

        public MainViewModel()
        {
            FFmpegLoader.FFmpegPath = Path.Combine(PathService.AppDataFolder, "Ffmpeg", "x86_64");
            AllocConsole();
            PlayPauseCommand = new RelayCommand(ExecutePlayPauseCommand);
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolderCommand);
            Video = VideoProcessing.GetInstance();
            decoder = new Decoder();
            renderer = new Renderer();
        }

        private async void ExecutePlayPauseCommand(object parameter)
        {

        }
     
        private async void ExecuteOpenFolderCommand(object parameter)
        {
            if (Video.ProcessType == Enum.ProcessType.Decoding)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Select a File";
            openFileDialog.Filter = "MP4 Files (*.mp4)|*.mp4";
            openFileDialog.InitialDirectory = "C:\\Users\\Arda\\Desktop\\Videos"; // Set initial directory

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string selectedFileName = openFileDialog.FileName;
                Video.Initialize(this, selectedFileName);

                Engine();
            }
            else
            {
                Console.WriteLine("No file selected.");
            }
        }
        public async void Engine()
        {
            _ = Task.Run(() => decoder.Start());
            Thread.Sleep(500);
            _ = Task.Run(() => renderer.Start());
        }

        [DllImport("kernel32")]
        static extern bool AllocConsole();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
