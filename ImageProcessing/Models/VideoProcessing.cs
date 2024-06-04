using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using ImageProcessing.Enum;
using ImageProcessing.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace ImageProcessing.Models
{
    /// <summary>
    /// SINGLETON
    /// </summary>
    public class VideoProcessing : IDisposable
    {
        public Buffering Buffer;
        public MainViewModel _mainViewModel;
        static VideoProcessing _videoProcessing;
        public MediaFile mediaFile { get; set; }
        public VideoStreamInfo videoStreamInfo { get; set; }
        public int numberOfFrames { get; set; }

        static bool isDisposed = false;
        public bool isInitialized = false;
        
        public ProcessType ProcessType { get; set; }

        public void Initialize(MainViewModel _mainViewModel,string videoPath)
        {
            mediaFile = MediaFile.Open(videoPath);
            Buffer = Buffering.GetInstance();
            videoStreamInfo = mediaFile.Video.Info;
            numberOfFrames = (int)videoStreamInfo.NumberOfFrames;
            this._mainViewModel = _mainViewModel;
            this._mainViewModel.NumberOfFrames = numberOfFrames;
            isInitialized = true;
        }
        public virtual void Start()
        {

        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!isDisposed)
                {
                    mediaFile.Dispose();
                    isDisposed = true;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public static VideoProcessing GetInstance()
        {
            if (_videoProcessing == null)
            {
                _videoProcessing = new VideoProcessing();
            }
            return _videoProcessing;
        }

    }
}
