using ImageProcessing.Models;
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
        static Buffering Buffer;
        static MotionDetector MotionDetection;
        public static int totalProcessedFrames = 0;
        public static int processedFrameIndex = 0;
        private static int frameCount;
        public Processor()
        {
            Video = VideoProcess.GetInstance();
            Buffer = Buffering.GetInstance();
        }
        public void Process()
        {
            frameCount = Video.Metadata.NumberOfFrames;
            MotionDetection = MotionDetector.GetInstance();
            Console.WriteLine(frameCount);
            try
            {
                if (!Video.isInitialized)
                {
                    Console.WriteLine("The video has not been initialized yet.");
                    return;
                }
                Video.State.ProcessingProcess = Enum.ProcessingProcess.Processing;
                while (true)
                {
                    if(totalProcessedFrames >= frameCount)
                    {
                        Video.State.ProcessingProcess = Enum.ProcessingProcess.Done;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Processing has done.");
                        return;
                    }
                    if (totalProcessedFrames >= Decoder.decodedFrameIndex)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    if(Buffering.Buffer.ElementAt(processedFrameIndex) != null)
                    {
                        var frame = Buffering.Buffer.ElementAt(processedFrameIndex);
                        var BitmapArray = frame.Bitmap;

                        // This code snippet has to be arrenged or revised.
                        using(MemoryStream  ms = new MemoryStream(BitmapArray))
                        {
                            Bitmap bitmap = new Bitmap(ms);
                            MotionDetection.ProcessFrame(bitmap);
                            bitmap.Save(ms, ImageFormat.Bmp);
                            frame.Bitmap = ms.ToArray();
                        }
                        // This code snippet has to be arrenged or revised.

                        //Mask.MaskByteArray(BitmapArray, 0, 0, 150, 150);
                        totalProcessedFrames++;
                        if (processedFrameIndex < Buffer.BUFFER_SIZE - 1)
                        {
                            processedFrameIndex++;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n\n\n\n\n");
                Console.WriteLine(processedFrameIndex);
                Console.WriteLine(ex.Message);
                Console.WriteLine("###############\n###############\n###############\nProcessor has done.\n###############\n###############\n###############\n");
                Console.WriteLine("\n\n\n\n\n");
            }
        }
    }
}
