using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Buffer = ImageProcessing.Services.Buffers.Buffer;
namespace ImageProcessing.Services
{
    public class Decoder : IDecoder , IRunner , IResetter , ICanceller
    {
        private VideoProcess _videoProcess;
        private Buffer _nextBuffer;
        private Renderer _renderer;

        public Task Task;
        public CancellationTokenSource CancellationTokenSource;
        public CancellationToken CancellationToken;
        #region Task
        public Task Decode(object fromIndex)
        {
            try
            {
                State.DecodedFrameIndex = 0;
                State.DecodingProcess = Enum.DecodingProcess.Processing;
                while (_videoProcess.MediaFile.Video.TryGetNextFrame(out var imageData) && !CancellationToken.IsCancellationRequested)
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
            }
            catch { }
            State.DecodingProcess = Enum.DecodingProcess.Done;
            return Task.CompletedTask;
        }
        public async Task Run()
        {
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
            try
            {
                Task = Task.Run(() => Decode(State.SliderValue),CancellationToken);
                await Task;
            }
            catch { }
        }
        public async Task Cancel()
        {
            if (Task.Status == TaskStatus.WaitingForActivation)
            {
                CancellationTokenSource.Cancel();
                try
                {
                    await Task;
                }
                catch { }
            }
        }
        public async void Reset()
        {
            _videoProcess.Reset();
            _renderer.Reset();
            await Cancel();
            Run();
        }
        #endregion
        #region Singleton
        private static Decoder _decoder;
        private Decoder()
        {
            _videoProcess = VideoProcess.GetInstance();
            _nextBuffer = NextBuffer.GetInstance();
            _renderer = Renderer.GetInstance();
        }
        public static Decoder GetInstance()
        {
            if (_decoder == null)
                _decoder = new Decoder();
            return _decoder;
        }
        #endregion
    }
}
