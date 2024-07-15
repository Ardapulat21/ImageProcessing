using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
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
        static VideoProcess Video;
        NextBuffer NextBuffer;
        
        public static int decodedFrameIndex = 0;
        public Decoder()
        {
            Video = VideoProcess.GetInstance();
            NextBuffer = NextBuffer.GetInstance();
        }
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
                    Bitmap bitmap = imageData.ToBitmap();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Bmp);
                        NextBuffer.Enqueue(new Frame(stream.ToArray(),decodedFrameIndex));
                    }
                    //Console.WriteLine($"{decodedFrameIndex}th frame decoded.");
                    decodedFrameIndex++;
                    bitmap.Dispose();
                }
                Video.State.DecodingProcess = Enum.DecodingProcess.Done;
                Console.WriteLine("All the frames have been decoded.");
            }
            catch (Exception e)
            {
            }
        }
    }
}
