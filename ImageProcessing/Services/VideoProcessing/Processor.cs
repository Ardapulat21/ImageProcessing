using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.IO;
using ImageProcessing.Services.MotionDetection;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;

namespace ImageProcessing.Services.VideoProcessing
{
    public class Processor : IProcessor
    {
        static VideoProcess _video;
        static NextBuffer _nextBuffer;
        static MotionDetector _motionDetector;
        public Processor()
        {
            _video = VideoProcess.GetInstance();
            _nextBuffer = NextBuffer.GetInstance();
            _motionDetector = new MotionDetector();

        }
        public void Process() 
        {
            _video.State.ProcessingProcess = Enum.ProcessingProcess.Processing;

            while (Metadata.TotalProcessedFrames < Metadata.NumberOfFrames)
            {
                if (Metadata.TotalProcessedFrames >= Metadata.DecodedFrameIndex)
                {
                    ConsoleService.WriteLine("Processor is waiting",IO.Color.Yellow);
                    Thread.Sleep(100);
                    continue;
                }
                try
                {
                    var frame = _nextBuffer.ElementAt(Metadata.ProcessedFrameIndex);
                    var BitmapArray = frame.Bitmap;

                    // bitmapArray -> bitmap -> PROCESSING -> bitmap -> bitmapArray
                    using (MemoryStream ms = new MemoryStream(BitmapArray))
                    {
                        Bitmap bitmap = new Bitmap(ms);
                        _motionDetector.ProcessFrame(bitmap);
                        bitmap.Save(ms, ImageFormat.Bmp);
                        bitmap.Dispose();
                        frame.Bitmap = ms.ToArray();
                        GC.Collect();
                    }
                    
                    Metadata.TotalProcessedFrames++;
                    if (Metadata.ProcessedFrameIndex < NextBuffer.BUFFER_SIZE - 1)
                        Metadata.ProcessedFrameIndex++;
                }
                catch { }
            }
            _video.State.ProcessingProcess = Enum.ProcessingProcess.Done;
            ConsoleService.WriteLine("Processing has done.",IO.Color.Green);
            return;
        }
    }
}
