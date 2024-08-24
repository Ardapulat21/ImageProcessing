using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.IO;
using ImageProcessing.Services.MotionDetection;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace ImageProcessing.Services.VideoProcessing
{
    public class Processor : IProcessor
    {
        static NextBuffer _nextBuffer;
        static MotionDetector _motionDetector;
        public Processor()
        {
            _nextBuffer = NextBuffer.GetInstance();
            _motionDetector = new MotionDetector();
        }
        public void Process() 
        {
            State.ProcessingProcess = Enum.ProcessingProcess.Processing;

            while (State.TotalProcessedFrames < Metadata.NumberOfFrames)
            {
                if (State.TotalProcessedFrames >= State.DecodedFrameIndex)
                {
                    ConsoleService.WriteLine("Processor is waiting",IO.Color.Yellow);
                    Thread.Sleep(100);
                    continue;
                }
                try
                {
                    var frame = _nextBuffer.ElementAt(State.TotalProcessedFrames);
                    var BitmapArray = frame;

                    using (MemoryStream ms = new MemoryStream(BitmapArray))
                    {
                        Bitmap bitmap = new Bitmap(ms);
                        _motionDetector.ProcessFrame(bitmap);
                        bitmap.Save(ms, ImageFormat.Bmp);
                        bitmap.Dispose();
                        _nextBuffer.Update(State.TotalProcessedFrames, ms.ToArray());
                        GC.Collect();
                    }
                    State.TotalProcessedFrames++;
                }
                catch { }
            }
            State.ProcessingProcess = Enum.ProcessingProcess.Done;
            ConsoleService.WriteLine("Processing has done.",IO.Color.Green);
            return;
        }
    }
}
