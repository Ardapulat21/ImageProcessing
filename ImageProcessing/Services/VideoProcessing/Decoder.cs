using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
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
        public void Decode(int fromIndex)
        {
            try
            {
                State.DecodedFrameIndex = 0;
                State.DecodingProcess = Enum.DecodingProcess.Processing;
                while (_video.MediaFile.Video.TryGetNextFrame(out var imageData) && State.DecodingProcess == Enum.DecodingProcess.Processing)
                {
                    State.DecodedFrameIndex++;
                    if (State.DecodedFrameIndex < fromIndex)
                    {
                        ConsoleService.WriteLine($"{State.DecodedFrameIndex}'th frame continued",IO.Color.Yellow);
                        continue;
                    }
                    Bitmap bitmap = imageData.ToBitmap();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Bmp);
                        _nextBuffer.Insert(State.DecodedFrameIndex - 1,stream.ToArray());
                    }
                    bitmap.Dispose();
                    GC.Collect();
                    ConsoleService.WriteLine($"{State.DecodedFrameIndex}'s frame decoded.",IO.Color.Green);
                }
                State.DecodingProcess = Enum.DecodingProcess.Done;
                ConsoleService.WriteLine("Decoding has done.",IO.Color.Green);
            }
            catch { }
        }
    }
}
