using FFMediaToolkit.Decoding;
using ImageProcessing.Enum;
using System;

namespace ImageProcessing.Models
{
    /// <summary>
    /// SINGLETON
    /// </summary>
    public class VideoProcessing : IDisposable
    {
        public MainViewModel MainViewModel;
        static VideoProcessing videoProcessing;
        public MediaFile mediaFile { get; set; }
        public VideoStreamInfo videoStreamInfo { get; set; }
        public int numberOfFrames { get; set; }

        static bool isDisposed = false;
        public bool isInitialized = false;
        
        public ProcessType ProcessType { get; set; }

        public void Initialize(MainViewModel _mainViewModel,string videoPath)
        {
            mediaFile = MediaFile.Open(videoPath);
            videoStreamInfo = mediaFile.Video.Info;
            numberOfFrames = (int)videoStreamInfo.NumberOfFrames;
            this.MainViewModel = _mainViewModel;
            this.MainViewModel.NumberOfFrames = numberOfFrames;
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
            if (videoProcessing == null)
            {
                videoProcessing = new VideoProcessing();
            }
            return videoProcessing;
        }

    }
}
