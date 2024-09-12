using FFMediaToolkit.Decoding;
using System;

namespace ImageProcessing.Models
{
    /// <summary>
    /// SINGLETON
    /// </summary>
    public class VideoProcess : IDisposable
    {
        static bool _isDisposed { get; set; } = false;
        public bool isInitialized { get; set; } = false;
        static VideoProcess _videoProcessing { get; set; }
        public MainViewModel MainViewModel { get; set; }
        public MediaFile MediaFile { get; set; }
        public VideoStreamInfo VideoStreamInfo { get; set; }
        private VideoProcess()
        {
        }
        public static VideoProcess GetInstance()
        {
            if (_videoProcessing == null)
            {
                _videoProcessing = new VideoProcess();
            }
            return _videoProcessing;
        }
        public void Initialize(MainViewModel _mainViewModel,string videoPath)
        {
            MediaFile = MediaFile.Open(videoPath);
            VideoStreamInfo = MediaFile.Video.Info;
            Metadata.Initialize(VideoStreamInfo);
            MainViewModel = _mainViewModel;
            MainViewModel.NumberOfFrames = Metadata.NumberOfFrames;
            isInitialized = true;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_isDisposed)
                {
                    MediaFile.Dispose();
                    _isDisposed = true;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
