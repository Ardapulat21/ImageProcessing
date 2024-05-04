﻿using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using ImageProcessing.Interfaces;
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
    public class VideoProcessing : IVideoProperties,IDisposable
    {
        static VideoProcessing _videoProcessing;
        private VideoProcessing() {}

        public static VideoProcessing GetInstance()
        {
            if (_videoProcessing == null)
            {
                _videoProcessing = new VideoProcessing();
            }
            return _videoProcessing;
        }
        public MediaFile mediaFile { get; set; }
        public VideoStreamInfo videoStreamInfo { get; set; }
        public int numberOfFrames { get; set; }

        static bool isDisposed = false;
        static bool isDisposing = false;
        public bool isInitialized = false;

        public bool isStartedDecoding = false;
        public bool isFinishedDecoding = false;
        public void Initialize(MainViewModel _mainViewModel,string videoPath)
        {
            FFmpegLoader.FFmpegPath = Path.Combine(PathService.AppDataFolder, "Ffmpeg", "x86_64");
            mediaFile = MediaFile.Open(videoPath);
            videoStreamInfo = mediaFile.Video.Info;
            numberOfFrames = (int)videoStreamInfo.NumberOfFrames;

            _mainViewModel.NumberOfFrames = numberOfFrames;

            isInitialized = true;
        }
        public void Fill(IVideoProperties obj)
        {
            obj.mediaFile = mediaFile;
            obj.videoStreamInfo = videoStreamInfo;
            obj.numberOfFrames = numberOfFrames;
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
    }
}
