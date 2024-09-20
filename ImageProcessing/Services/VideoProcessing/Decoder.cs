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
namespace ImageProcessing.Services
{
    public class Decoder : IDecoder
    {
        private static VideoProcess _videoProcess;
        private static NextBuffer _nextBuffer;
        private static Decoder _decoder;
        public static Task Task = null;
        public static CancellationTokenSource CancellationTokenSource;
        public static CancellationToken CancellationToken;
        private Decoder()
        {
            _videoProcess = VideoProcess.GetInstance();
            _nextBuffer = NextBuffer.GetInstance();
        }
        public Task Decode(object fromIndex)
        {
            try
            {
                ConsoleService.WriteLine("\n\n\nDecoding has started.\n\n\n", IO.Color.Red);
                State.DecodedFrameIndex = 0;
                State.DecodingProcess = Enum.DecodingProcess.Processing;
                while (_videoProcess.MediaFile.Video.TryGetNextFrame(out var imageData) && !CancellationToken.IsCancellationRequested)
                {
                    if (CancellationToken.IsCancellationRequested)
                    {
                        CancellationToken.ThrowIfCancellationRequested();
                    }
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
            catch 
            {
            }
            State.DecodingProcess = Enum.DecodingProcess.Done;
            return Task.CompletedTask;
        }
        #region Task
        public async Task RunTask()
        {
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
            try
            {
                Task = Task.Run(() => Decode(State.SliderValue),CancellationToken);
                await Task;
            }
            catch (OperationCanceledException)
            {
                ConsoleService.WriteLine("\n\n\nDecoder Task has been canceled.\n\n\n",IO.Color.Red);
            }
            catch { }
        }
        public async Task CancelTask()
        {
            if (Task.Status == TaskStatus.WaitingForActivation)
            {
                CancellationTokenSource.Cancel();
                try
                {
                    await Task;
                }
                catch {}
            }
        }
        public async void Reset()
        {
            await _decoder.CancelTask();
            _videoProcess.Reset();
            _decoder.RunTask();
        }
        #endregion
        #region Singleton
        public static Decoder GetInstance()
        {
            if (_decoder == null)
                _decoder = new Decoder();
            return _decoder;
        }
        #endregion
    }
}
