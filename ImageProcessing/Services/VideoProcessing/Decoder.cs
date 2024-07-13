using ImageProcessing.Models;
using ImageProcessing.Services.ImageProcessing;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
namespace ImageProcessing.Services
{
    public class Decoder
    {
        static VideoProcess Video = VideoProcess.GetInstance();
        Buffering Buffer = Buffering.GetInstance();
        public static int decodedFrameIndex = 0;
        public void Decode()   
        {
            try
            {
                if (!Video.isInitialized)
                {
                    Console.WriteLine("The video has not been initialized yet.");
                    return;
                }
                Video.State.DecodingProcess = Enum.DecodingProcess.Processing;
                while (Video.MediaFile.Video.TryGetNextFrame(out var imageData))
                {
                    while (Buffer.Size > Buffer.BUFFER_SIZE)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} is waiting for the frame to be rendered");
                        Thread.Sleep(100);
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Bitmap bitmap = imageData.ToBitmap();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Bmp);
                        Buffer.Enqueue(new Frame(stream.ToArray(),decodedFrameIndex));
                    }
                    Console.WriteLine($"{decodedFrameIndex}th frame decoded.");
                    decodedFrameIndex++;
                    bitmap.Dispose();
                }
                Video.State.DecodingProcess = Enum.DecodingProcess.Done;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("All the frames have been decoded.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
