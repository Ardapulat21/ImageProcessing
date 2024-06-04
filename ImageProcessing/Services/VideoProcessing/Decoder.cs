using ImageProcessing.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace ImageProcessing.Services
{
    public class Decoder
    {
        VideoProcessing Video = VideoProcessing.GetInstance();
        Buffering Buffer = Buffering.GetInstance();
        public void Start()   
        {
            try
            {
                if (!Video.isInitialized)
                {
                    Console.WriteLine("The video has not been initialized yet.");
                    return;
                }
                int loopCounter = 0;
                Video.ProcessType = Enum.ProcessType.Decoding;
                while (Video.mediaFile.Video.TryGetNextFrame(out var imageData))
                {
                    while (Buffer.Size > Buffer.BUFFER_SIZE)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} is been waiting the frame in the buffer to be rendered and removed.");
                        Thread.Sleep(100);
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Bitmap bitmap = imageData.ToBitmap();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Bmp);
                        Buffer.Enqueue(new Frame(stream.ToArray(),loopCounter));
                    }
                    Console.WriteLine($"{loopCounter}th frame decoded.");
                    loopCounter++;
                    bitmap.Dispose();
                }
                Video.ProcessType = Enum.ProcessType.Done;
                Console.WriteLine("All the frames have been decoded.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
