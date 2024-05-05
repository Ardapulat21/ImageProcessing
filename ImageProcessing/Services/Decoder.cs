using FFMediaToolkit.Decoding;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ImageProcessing.Models;
using ImageProcessing.Interfaces;

namespace ImageProcessing.Services
{
    public class Decoder : ICoder,IVideoProperties
    {
        VideoProcessing Video;
        Buffering Buffer;
        public Decoder()
        {
            Video = VideoProcessing.GetInstance();
            Video.Fill(this);
            Buffer = Buffering.GetInstance();
        }
        public MediaFile mediaFile { get; set; }
        public VideoStreamInfo videoStreamInfo { get; set; }
        public int numberOfFrames { get; set; }
        
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
                Video.ProcessType = Enum.ProcessType.Processing;
                while (mediaFile.Video.TryGetNextFrame(out var imageData))
                {
                    while (Buffer.Count > Buffer.BUFFER_SIZE)
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
                        Buffer.Enqueue(stream.ToArray());
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
