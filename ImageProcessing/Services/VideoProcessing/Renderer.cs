using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.IO;
using System.Threading;
using System.Threading.Tasks;
namespace ImageProcessing.Services.Buffers
{
    public class Renderer : IRenderer , IRunner , IResetter
    {
        private Displayer _displayer { get; set; }
        private Buffer _nextBuffer { get; set; }
        private Buffer _prevBuffer { get; set; }
        private IVideoProcess _video { get; set; }
        private static Renderer _renderer { get; set; }
        public Task Task;
        public async Task Run()
        {
            Task = new Task(_renderer.Rendering);
            Task.Start();
        }
        public async void Reset()
        {
            _nextBuffer.Clear();
            _prevBuffer.Clear();
        }
        public bool IsFrameAvailable(int index,out byte[] stream)
        {
            if (_nextBuffer.TryGetFrame(index, out byte[] Frame) || _prevBuffer.TryGetFrame(index, out Frame))
            {
                stream = Frame;
                return true;
            }
            stream = null;
            return false;
        }
       
        public void Rendering()
        {
            try
            {
                int sliderValue;
                State.RenderingProcess = Enum.RenderingProcess.Processing;
                while (true)
                {
                    sliderValue = State.SliderValue;
                    if (IsFrameAvailable(sliderValue,out byte[] Frame))
                    {
                        _displayer.Display(Frame);
                        State.SliderValue++;
                        ConsoleService.WriteLine($"{sliderValue}'s frame rendered.",Color.Green);
                    }
                    else
                    {
                        if (sliderValue == Metadata.NumberOfFrames)
                        {
                            State.RenderingProcess = Enum.RenderingProcess.Done;
                            ConsoleService.WriteLine("Rendering process is done.",Color.Red);
                            _video.Dispose();
                            break;
                        }
                        LoggerService.Info($"{sliderValue}'th frame could not be fount either of two buffers.");
                        ConsoleService.WriteLine($"{sliderValue}'s frame is missing in the either of buffers.",Color.Red);
                        Thread.Sleep(100);
                    }
                }
            }
            catch { }
        }
        #region Singleton
        private Renderer()
        {
            _video = VideoProcess.GetInstance();
            _nextBuffer = NextBuffer.GetInstance();
            _prevBuffer = PrevBuffer.GetInstance();
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
