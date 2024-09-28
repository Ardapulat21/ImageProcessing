using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.IO;
using ImageProcessing.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace ImageProcessing.Services.Buffers
{
    public class Renderer : IRenderer , IRunner , ICanceller
    {
        private Displayer _displayer { get; set; }
        private IVideoProcess _video { get; set; }
        private static Renderer _renderer { get; set; }
        public void Rendering()
        {
            try
            {
                int sliderValue = 0;
                State.RenderingProcess = Enum.RenderingProcess.Processing;
                while (!CancellationTokenSource.IsCancellationRequested && sliderValue < Metadata.NumberOfFrames)
                {
                    sliderValue = State.SliderValue;
                    if(State.IsPlaying == false)
                    {
                        ConsoleService.WriteLine("Renderer has stopped.",Color.Red);
                        Thread.Sleep(500);
                    }
                    else if (BufferUtils.IsFrameAvailable(sliderValue,out byte[] Frame))
                    {
                        _displayer.Display(Frame);
                        State.SliderValue++;
                        ConsoleService.WriteLine($"{sliderValue}'s frame rendered.",Color.Green);
                    }
                    else
                    {
                        LoggerService.Info($"{sliderValue}'th frame could not be fount either of two buffers.");
                        ConsoleService.WriteLine($"{sliderValue}'s frame is missing in the either of buffers.",Color.Red);
                        Thread.Sleep(400);
                    }
                }
            }
            catch { }
            finally
            {
                State.RenderingProcess = Enum.RenderingProcess.Done;
                ConsoleService.WriteLine("Rendering process is done.", Color.Red);
                _video.Dispose();
            }
        }
        #region Task
        public Task Task;
        CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        CancellationToken CancellationToken = new CancellationToken();
        public async Task Run()
        {
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
            try
            {
                Task = Task.Run(Rendering, CancellationToken);
                await Task;
            }
            catch { }
        }
        public async Task Cancel()
        {
            if (Task.Status == TaskStatus.WaitingForActivation || Task.Status == TaskStatus.Running)
            {
                CancellationTokenSource.Cancel();
                try
                {
                    await Task;
                }
                catch { }
            }
        }
        #endregion
        #region Singleton
        private Renderer()
        {
            _video = VideoProcess.GetInstance();
            _displayer = new Displayer();
        }
        public static Renderer GetInstance()
        {
            if (_renderer == null)
            {
                _renderer = new Renderer();
            }
            return _renderer;
        }
        #endregion
    }
}
