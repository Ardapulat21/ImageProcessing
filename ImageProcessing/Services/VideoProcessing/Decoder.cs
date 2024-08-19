using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace ImageProcessing.Services
{
    public class Decoder : IDecoder
    {
        static VideoProcess _video;
        static NextBuffer _nextBuffer;
        public Decoder()
        {
            _video = VideoProcess.GetInstance();
            _nextBuffer = NextBuffer.GetInstance();
        }
        public void Decode()
        {
            try
            {
                _video.State.DecodingProcess = Enum.DecodingProcess.Processing;
                while (_video.MediaFile.Video.TryGetNextFrame(out var imageData))
                {
                    Bitmap bitmap = imageData.ToBitmap();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Bmp);
                        _nextBuffer.Enqueue(new Frame(stream.ToArray(),Metadata.DecodedFrameIndex));
                    }
                    Metadata.DecodedFrameIndex++;
                    ConsoleService.WriteLine($"{Metadata.DecodedFrameIndex}'s frame decoded.",IO.Color.Green);
                    bitmap.Dispose();
                }
                _video.State.DecodingProcess = Enum.DecodingProcess.Done;
                ConsoleService.WriteLine("All the frames have been decoded.",IO.Color.Green);
            }
            catch { }
        }
    }
}
