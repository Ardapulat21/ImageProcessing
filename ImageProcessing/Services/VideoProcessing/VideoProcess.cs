using FFMediaToolkit.Decoding;
using ImageProcessing.Interfaces;
using ImageProcessing.Services.Buffers;
using System;

namespace ImageProcessing.Models
{
    public class VideoProcess : IVideoProcess ,IResetter ,IDisposable
    {
        static bool _isDisposed { get; set; } = false;
        public bool IsInitialized { get; set; } = false;
        static VideoProcess _videoProcessing { get; set; }
        public MainViewModel MainViewModel { get; set; }
        public MediaFile MediaFile { get; set; }
        public VideoStreamInfo VideoStreamInfo { get; set; }
        public void Initialize(MainViewModel _mainViewModel)
        {
            OpenVideo();
            MainViewModel = _mainViewModel;
            MainViewModel.NumberOfFrames = Metadata.NumberOfFrames;
            IsInitialized = true;
        }
        public void OpenVideo()
        {
            MediaFile = MediaFile.Open(Metadata.FilePath); 
            VideoStreamInfo = MediaFile.Video.Info;
            Metadata.Initialize(VideoStreamInfo);
        }
        public void Reset()
        {
            Dispose();
            OpenVideo();
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
        #region Singleton
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
        #endregion
    }
}
