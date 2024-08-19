using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.MotionDetection;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;

namespace ImageProcessing.Services.VideoProcessing
{
    public class Processor
    {
        static VideoProcess Video;
        static NextBuffer NextBuffer;
        static MotionDetector MotionDetection;
        public static int totalProcessedFrames = 0;
        public static int processedFrameIndex = 0;
        private static int frameCount;
        public Processor()
        {
            Video = VideoProcess.GetInstance();
            NextBuffer = NextBuffer.GetInstance();
        }
        public void Process()
        {
            frameCount = Video.Metadata.NumberOfFrames;
            MotionDetection = MotionDetector.GetInstance();

            Video.State.ProcessingProcess = Enum.ProcessingProcess.Processing;
            while (totalProcessedFrames < frameCount)
            {
                if (totalProcessedFrames >= Metadata.DecodedFrameIndex)
                {
                    Thread.Sleep(100);
                    continue;
                }
                try
                {
                    var frame = NextBuffer.ElementAt(processedFrameIndex);
                    var BitmapArray = frame.Bitmap;

                    // bitmapArray -> bitmap -> PROCESSING -> bitmap -> bitmapArray
                    using (MemoryStream ms = new MemoryStream(BitmapArray))
                    {
                        Bitmap bitmap = new Bitmap(ms);
                        MotionDetection.ProcessFrame(bitmap);
                        bitmap.Save(ms, ImageFormat.Bmp);
                        frame.Bitmap = ms.ToArray();
                    }
                    
                    totalProcessedFrames++;
                    if (processedFrameIndex < NextBuffer.BUFFER_SIZE - 1)
                    {
                        processedFrameIndex++;
                    }
                }
                catch { }
            }
            Video.State.ProcessingProcess = Enum.ProcessingProcess.Done;
            Console.WriteLine("Processing has done.");
            return;
        }
    }
}
