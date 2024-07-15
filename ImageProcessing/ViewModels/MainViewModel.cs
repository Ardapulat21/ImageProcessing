using FFMediaToolkit;
using ImageProcessing.Models;
using ImageProcessing.MVVM_Helper;
using ImageProcessing.Services;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.ImageProcessing;
using ImageProcessing.Services.VideoProcessing;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        public ObservableCollection<Rectangle> Rectangles { get; set; }

        private Rectangle _selectedItem;
        public Rectangle SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                Text = $"Rectangle Coordinates: {_selectedItem.X} | {_selectedItem.Y} | {_selectedItem.Width} | {_selectedItem.Height}";
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

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
        public ICommand FirstCommand { get; }
        public ICommand BackwardCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ForwardCommand { get; }
        public ICommand LastCommand { get; }
        public ICommand OpenFolderCommand { get; }

        private async void ExecuteFirstCommand(object parameter)
        {

        }
        private async void ExecuteBackwardCommand(object parameter)
        {
            BufferDealer.SeekFrame -= 5;
        }
        private async void ExecuteStopCommand(object parameter)
        {

        }
        private async void ExecutePlayCommand(object parameter)
        {

        }
        private async void ExecuteForwardCommand(object parameter)
        {
            BufferDealer.SeekFrame += 5;

        }
        private async void ExecuteLastCommand(object parameter)
        {

        }

        private async void ExecuteOpenFolderCommand(object parameter)
        {
            if (Video.State.DecodingProcess == Enum.DecodingProcess.Processing)
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
                System.Console.WriteLine("No file selected.");
            }
        }

        #endregion

        VideoProcess Video;
        Decoder Decoder;
        Renderer Renderer;
        Processor Processor;
        BufferDealer BufferDealer;
        public MainViewModel(MainWindow mainWindow)
        {
            FFmpegLoader.FFmpegPath = Path.Combine(PathService.AppDataFolder, "Ffmpeg", "x86_64");
            AllocConsole();

            FirstCommand = new RelayCommand(ExecuteFirstCommand);
            BackwardCommand = new RelayCommand(ExecuteBackwardCommand);
            StopCommand = new RelayCommand(ExecuteStopCommand);
            PlayCommand = new RelayCommand(ExecutePlayCommand);
            ForwardCommand = new RelayCommand(ExecuteForwardCommand);
            LastCommand = new RelayCommand(ExecuteLastCommand);
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolderCommand);

            Video = VideoProcess.GetInstance();
            BufferDealer = BufferDealer.GetInstance();
            Decoder = new Decoder();
            Renderer = new Renderer();
            Processor = new Processor();
            Rectangles = mainWindow.Rectangles;
        }
        
        
        public async void Engine()
        {
            _ = Task.Run(() => Decoder.Decode());
            Thread.Sleep(500);
            //_ = Task.Run(() => Processor.Process());
            _ = Task.Run(() => BufferDealer.Flow());
            //_ = Task.Run(() => Renderer.Render());
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
