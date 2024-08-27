using FFMediaToolkit.Decoding;
using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Controls;
namespace ImageProcessing.Services
{
    public class Decoder : IDecoder
    {
        private static VideoProcess _video;
        private static NextBuffer _nextBuffer;
        private static Decoder _decoder;
        public static Task Task;
        private Decoder()
        {
            _video = VideoProcess.GetInstance();
            _nextBuffer = NextBuffer.GetInstance();
        }
        public void RunTask()
        {
            if (Task != null)
            {
                State.DecoderToken.Cancel();
                Task.Wait();
            }
            State.DecoderToken = new System.Threading.CancellationTokenSource();
            Task = new Task(() => _decoder.Decode(State.SliderValue));
            Task.Start();
        }
        public static Decoder GetInstance()
        {
            if(_decoder == null)
                _decoder = new Decoder();
            return _decoder;
        }

        public void Decode(object fromIndex)
        {
            try
            {
                ConsoleService.WriteLine("Decoding has started.", IO.Color.Red);
                State.DecodedFrameIndex = 0;
                State.DecodingProcess = Enum.DecodingProcess.Processing;
                while (_video.MediaFile.Video.TryGetNextFrame(out var imageData) && !State.DecoderToken.IsCancellationRequested)
                {
                    State.DecodedFrameIndex++;
                    if (State.DecodedFrameIndex < (int)fromIndex)
                    {
                        ConsoleService.WriteLine($"{State.DecodedFrameIndex}'th frame continued", IO.Color.Yellow);
                        continue;
                    }
                    Bitmap bitmap = imageData.ToBitmap();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Bmp);
                        _nextBuffer.Insert(State.DecodedFrameIndex - 1, stream.ToArray());
                    }
                    bitmap.Dispose();
                    GC.Collect();
                    ConsoleService.WriteLine($"{State.DecodedFrameIndex - 1}'s frame decoded.", IO.Color.Green);
                }
                State.DecodingProcess = Enum.DecodingProcess.Done;
                ConsoleService.WriteLine("Decoding has done.", IO.Color.Red);
            }
            catch(Exception ex) 
            {
                ConsoleService.WriteLine(ex.Message,IO.Color.Red);
                throw;
            }
        }
    }
}
