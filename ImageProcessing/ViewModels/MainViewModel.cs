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
        private bool _isPlayPauseEnabled = false;
        public bool IsPlayPauseEnabled
        {
            get => _isPlayPauseEnabled;
            set
            {
                _isPlayPauseEnabled = value;
                OnPropertyChanged(nameof(IsPlayPauseEnabled));
            }
        }

        private int _numberOfFrames = 0;
        public int NumberOfFrames
        {
            get => _numberOfFrames;
            set
            {
                _numberOfFrames = value;
                OnPropertyChanged(nameof(NumberOfFrames));
            }
        }
        public int SliderValue
        {
            get => State.SliderValue;
            set
            {
                State.SliderValue = value;
                OnPropertyChanged(nameof(SliderValue));
            }
        }

        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }
        #endregion
        #region Commands
        public ICommand FirstFrameCommand { get; }
        public ICommand BackwardCommand { get; }
        public ICommand PlayPauseCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ForwardCommand { get; }
        public ICommand LastFrameCommand { get; }
        public ICommand OpenFolderCommand { get; }
        #endregion
        #region Actions
        private void ExecuteFirstFrameCommand(object parameter)
        {
            if (!_videoProcess.IsInitialized)
                return;

            State.ProcessedFrameIndex = 0;
            SliderValue = 0;
            _decoder.Reset();
        }
        private void ExecuteBackwardCommand(object parameter)
        {
            if (!_videoProcess.IsInitialized)
                return;

            State.ProcessedFrameIndex -= 5;
            SliderValue -= 5;
        }
        private void ExecutePlayPauseCommand(object parameter)
        {
            if (_videoProcess.IsInitialized == false)
                return;

            State.IsPlaying = !State.IsPlaying;
        }
        private void ExecuteForwardCommand(object parameter)
        {
            if (!_videoProcess.IsInitialized)
                return;

            State.ProcessedFrameIndex += 5;
            SliderValue += 5;
        }
        private async void ExecuteLastFrameCommand(object parameter)
        {
            if (!_videoProcess.IsInitialized)
                return;

            SliderValue = Metadata.NumberOfFrames - 10;
            State.ProcessedFrameIndex = SliderValue;
            _decoder.Reset();
        }
        private async void ExecuteOpenFolderCommand(object parameter)
        {
            if (State.DecodingProcess == Enum.DecodingProcess.Processing || State.RenderingProcess == Enum.RenderingProcess.Processing)
                return;

            SliderValue = 0;
            IsPlayPauseEnabled = true;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Select a File";
            openFileDialog.Filter = "MP4 Files (*.mp4)|*.mp4";

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                State.IsPlaying = true;
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
        #region Dependencies
        IVideoProcess _videoProcess;
        Decoder _decoder;
        Processor _processor;
        Renderer _renderer;
        #endregion
        public MainViewModel()
        {
            FFmpegLoader.FFmpegPath = Path.Combine(PathService.FFMPEGFolder, "x86_64");
#if CONSOLE
            AllocConsole();
#endif

            FirstFrameCommand = new RelayCommand(ExecuteFirstFrameCommand);
            BackwardCommand = new RelayCommand(ExecuteBackwardCommand);
            PlayPauseCommand = new RelayCommand(ExecutePlayPauseCommand);
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
