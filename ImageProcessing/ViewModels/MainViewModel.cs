#define CONSOLE
using FFMediaToolkit;
using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.MVVM_Helper;
using ImageProcessing.Services;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.IO;
using ImageProcessing.Services.MotionDetection;
using ImageProcessing.Services.VideoProcessing;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
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

        private int _numberOfFrames = 0;
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
                State.SliderValue = value;
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
        #region Commands
        public ICommand FirstFrameCommand { get; }
        public ICommand BackwardCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ForwardCommand { get; }
        public ICommand LastFrameCommand { get; }
        public ICommand OpenFolderCommand { get; }

        private void ExecuteFirstFrameCommand(object parameter)
        {
            State.ProcessedFrameIndex = 0;
            SliderValue = 0;
            _decoder.Reset();
        }
        private void ExecuteBackwardCommand(object parameter)
        {
            State.SliderValue -= 5;
        }
        private void ExecuteStopCommand(object parameter)
        {
        }
        private void ExecutePlayCommand(object parameter)
        {
        }
        private void ExecuteForwardCommand(object parameter)
        {
            SliderValue += 5;
        }
        private async void ExecuteLastFrameCommand(object parameter)
        {
            SliderValue = Metadata.NumberOfFrames - 10;
            State.ProcessedFrameIndex = SliderValue;
            _decoder.Reset();
        }
        private async void ExecuteOpenFolderCommand(object parameter)
        {
            if (State.DecodingProcess == Enum.DecodingProcess.Processing || State.RenderingProcess == Enum.RenderingProcess.Processing)
                return;

            SliderValue = 0;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Select a File";
            openFileDialog.Filter = "MP4 Files (*.mp4)|*.mp4";

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                Metadata.FilePath = openFileDialog.FileName;
                _videoProcess.Initialize(this);
                MotionDetector.Initialize();
                KickOff();
            }
            else
            {
                ConsoleService.WriteLine("No file selected.",Services.IO.Color.Red);
            }
        }

        #endregion
        IVideoProcess _videoProcess;
        Decoder _decoder;
        Processor _processor;
        Renderer _renderer;
        public MainViewModel()
        {
            FFmpegLoader.FFmpegPath = Path.Combine(PathService.FFMPEGFolder, "x86_64");
#if CONSOLE
            AllocConsole();
#endif

            FirstFrameCommand = new RelayCommand(ExecuteFirstFrameCommand);
            BackwardCommand = new RelayCommand(ExecuteBackwardCommand);
            StopCommand = new RelayCommand(ExecuteStopCommand);
            PlayCommand = new RelayCommand(ExecutePlayCommand);
            ForwardCommand = new RelayCommand(ExecuteForwardCommand);
            LastFrameCommand = new RelayCommand(ExecuteLastFrameCommand);
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolderCommand);

            _videoProcess = VideoProcess.GetInstance();
            _renderer = Renderer.GetInstance();
            _decoder = Decoder.GetInstance();
            _processor = Processor.GetInstance();
        }
       
        public async void KickOff()
        {
            if (!_videoProcess.IsInitialized)
            {
                ConsoleService.WriteLine("The video has not been initialized yet.",Services.IO.Color.Red);
                return;
            }
            _decoder.Run();
            _processor.Run();
            _renderer.Run();
        }

        #region DLL32
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
