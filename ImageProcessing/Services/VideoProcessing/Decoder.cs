using FFMediaToolkit.Graphics;
using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.IO;
using ImageProcessing.Utils;
using ImageProcessing.ViewModels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Buffer = ImageProcessing.Services.Buffers.Buffer;
namespace ImageProcessing.Services
{
    public class Decoder : IDecoder , IRunner , IResetter , ICanceller
    {
        private SplashScreenViewModel _splashScreenViewModel;
        private VideoProcess _videoProcess;
        private Buffer _nextBuffer;
        private Buffer _prevBuffer;
        Buffer pointerBuffer;
        public Task Decode(object fromIndex)
        {
            try
            {
                double index = (int)fromIndex;
                int decodedFrameIndex = 0;
                State.DecodedFrameIndex = 0;
                State.DecodingProcess = Enum.DecodingProcess.Processing;
                while (_videoProcess.MediaFile.Video.TryGetNextFrame(out var imageData) && !CancellationToken.IsCancellationRequested)
                {
                    State.DecodedFrameIndex++;
                    decodedFrameIndex++;
                    if (decodedFrameIndex < index - 100)
                    {
                        _splashScreenViewModel.SetProgress(decodedFrameIndex / index);
                        ConsoleService.WriteLine($"{decodedFrameIndex}'th frame continued", IO.Color.Yellow);
                        continue;
                    }
                    PushBuffer(imageData,decodedFrameIndex,index);
                    ConsoleService.WriteLine($"{decodedFrameIndex - 1}'s frame decoded.", IO.Color.Green);
                }
            }
            catch { }
            State.DecodingProcess = Enum.DecodingProcess.Done;
            return Task.CompletedTask;
        }
        private void PushBuffer(ImageData imageData,int decodedFrameIndex,double fromIndex)
        {
            if (decodedFrameIndex >= fromIndex - 100 && decodedFrameIndex < fromIndex)
            {
                _splashScreenViewModel.SetProgress(decodedFrameIndex / fromIndex);
                pointerBuffer = _prevBuffer;
            }
            else
            {
                _splashScreenViewModel.Hide();
                pointerBuffer = _nextBuffer;
            }
            pointerBuffer.Push(imageData, decodedFrameIndex);
        }
        #region Task
        public Task Task;
        public CancellationTokenSource CancellationTokenSource;
        public CancellationToken CancellationToken;
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
            BufferUtils.Reset();
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
            _prevBuffer = PrevBuffer.GetInstance();
            _splashScreenViewModel = SplashScreenViewModel.GetInstance();
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
