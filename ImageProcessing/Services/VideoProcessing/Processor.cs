using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.ImageProcessing;
using ImageProcessing.Services.MotionDetection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            System.Console.WriteLine(frameCount);
            if (!Video.isInitialized)
            {
                Console.WriteLine("The video has not been initialized yet.", ConsoleColor.Red);
                return;
            }
            Video.State.ProcessingProcess = Enum.ProcessingProcess.Processing;
            while (true)
            {
                if(totalProcessedFrames >= frameCount)
                {
                    Video.State.ProcessingProcess = Enum.ProcessingProcess.Done;
                    Console.WriteLine("Processing has done.", ConsoleColor.Green);
                    return;
                }
                if (totalProcessedFrames >= Decoder.decodedFrameIndex)
                {
                    Thread.Sleep(100);
                    continue;
                }
                try
                {
                    var frame = NextBuffer.Queue.ElementAt(processedFrameIndex);
                    var BitmapArray = frame.Bitmap;

                    // This code snippet has to be arrenged or revised.
                    using (MemoryStream ms = new MemoryStream(BitmapArray))
                    {
                        Bitmap bitmap = new Bitmap(ms);
                        MotionDetection.ProcessFrame(bitmap);
                        bitmap.Save(ms, ImageFormat.Bmp);
                        frame.Bitmap = ms.ToArray();
                    }
                    // This code snippet has to be arrenged or revised.

                    //Mask.MaskByteArray(BitmapArray, 0, 0, 150, 150);
                    totalProcessedFrames++;
                    if (processedFrameIndex < NextBuffer.BUFFER_SIZE - 1)
                    {
                        processedFrameIndex++;
                    }
                }
                catch { }
            }
        }
    }
}
