using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.MVVM_Helper;
using ImageProcessing.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using static System.Net.WebRequestMethods;
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
        ICoder _renderer;
        ICoder _decoder;

        public MainViewModel()
        {
            AllocConsole();
            PlayPauseCommand = new RelayCommand(ExecutePlayPauseCommand);
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolderCommand);

            Video = VideoProcessing.GetInstance();
        }
        
        private async void ExecutePlayPauseCommand(object parameter)
        {
            if (Video.ProcessType == Enum.ProcessType.Processing || Video.isInitialized == false)
                return;

            _decoder = new Decoder();
            _renderer = new Renderer();

            _ = Task.Run(() => _decoder.Start());
            Thread.Sleep(1000);
            _ = Task.Run(() => _renderer.Start());
        }
        private void ExecuteOpenFolderCommand(object parameter)
        {
            if (Video.ProcessType == Enum.ProcessType.Processing)
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
            }
            else
            {
                Console.WriteLine("No file selected.");
            }
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
