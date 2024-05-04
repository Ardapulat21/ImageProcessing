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
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using static System.Net.WebRequestMethods;

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
        public ICommand PlayPauseCommand { get; }
        #endregion

        VideoProcessing Video;
        ICoder _renderer;
        ICoder _decoder;
        public MainViewModel()
        {
            AllocConsole();
            PlayPauseCommand = new RelayCommand(ExecutePlayPauseCommand);

            Video = VideoProcessing.GetInstance();
            Video.Initialize(this,PathService.videoPath);

            _decoder = new Services.Decoder(this);
            _renderer = new Services.Renderer(this);
        }
            
        private async void ExecutePlayPauseCommand(object parameter)
        {
            Console.WriteLine("Play button has called.");
            Task.Run(() => _decoder.Start());
            Thread.Sleep(1000);
            Task.Run(() => _renderer.Start());
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
