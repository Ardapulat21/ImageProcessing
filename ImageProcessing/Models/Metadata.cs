using FFMediaToolkit.Decoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Models
{
    public static class Metadata
    {
        public static int DecodedFrameIndex { get; set; } = 0;
        public static int RenderedFrameIndex { get; set; } = 0;
        public static int TotalProcessedFrames { get; set; } = 0;
        public static int ProcessedFrameIndex { get; set; } = 0;
        public static int NumberOfFrames { get; set; }
        public static int Width { get; set; }
        public static int Height { get; set; }
        public static int FPS { get; set; }

        public static void Initialize(VideoStreamInfo videoStreamInfo)
        {
            DecodedFrameIndex = 0;
            RenderedFrameIndex = 0;
            ProcessedFrameIndex = 0;
            TotalProcessedFrames = 0;
            Width = videoStreamInfo.FrameSize.Width;
            Height = videoStreamInfo.FrameSize.Height;
            FPS = (int)videoStreamInfo.AvgFrameRate;
            NumberOfFrames = (int)videoStreamInfo.NumberOfFrames;
        }
    }
}
