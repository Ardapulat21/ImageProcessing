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
    }
}
