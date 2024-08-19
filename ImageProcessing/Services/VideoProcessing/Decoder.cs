using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace ImageProcessing.Services
{
    public class Decoder : IDecoder
    {
        static VideoProcess Video;
        static NextBuffer NextBuffer;
        public Decoder()
        {
            Video = VideoProcess.GetInstance();
            NextBuffer = NextBuffer.GetInstance();
        }
        public void Decode()   
        {
            try
            {
                Video.State.DecodingProcess = Enum.DecodingProcess.Processing;
                while (Video.MediaFile.Video.TryGetNextFrame(out var imageData))
                {
                    Bitmap bitmap = imageData.ToBitmap();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Bmp);
                        NextBuffer.Enqueue(new Frame(stream.ToArray(),Metadata.DecodedFrameIndex));
                    }
                    Metadata.DecodedFrameIndex++;
                    bitmap.Dispose();
                }
                Video.State.DecodingProcess = Enum.DecodingProcess.Done;
                Console.WriteLine("All the frames have been decoded.");
            }
            catch { }
        }
    }
}
