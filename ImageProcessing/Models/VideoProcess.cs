using FFMediaToolkit.Decoding;
using ImageProcessing.Enum;
using ImageProcessing.Services.MotionDetection;
using ImageProcessing.Services.VideoProcessing;
using System;

namespace ImageProcessing.Models
{
    /// <summary>
    /// SINGLETON
    /// </summary>
    public class VideoProcess : IDisposable
    {
        static VideoProcess VideoProcessing { get; set; }
        public MainViewModel MainViewModel { get; set; }
        public MediaFile MediaFile { get; set; }
        public VideoStreamInfo VideoStreamInfo { get; set; }
        public Metadata Metadata { get; set; }
        static bool isDisposed = false;
        public bool isInitialized = false;
        public State State { get; set; }
        private VideoProcess()
        {
            State = new State();
        }
        public void Initialize(MainViewModel _mainViewModel,string videoPath)
        {
            MediaFile = MediaFile.Open(videoPath);
            VideoStreamInfo = MediaFile.Video.Info;
            Metadata = new Metadata(VideoStreamInfo.FrameSize.Width,VideoStreamInfo.FrameSize.Height,(int)VideoStreamInfo.AvgFrameRate,(int)VideoStreamInfo.NumberOfFrames);
            MainViewModel = _mainViewModel;
            isInitialized = true;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!isDisposed)
                {
                    MediaFile.Dispose();
                    isDisposed = true;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public static VideoProcess GetInstance()
        {
            if (VideoProcessing == null)
            {
                VideoProcessing = new VideoProcess();
            }
            return VideoProcessing;
        }

    }
}
