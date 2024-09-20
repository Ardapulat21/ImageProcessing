using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
namespace ImageProcessing.Services.Buffers
{
    public class BufferDealer
    {
        private Renderer _renderer { get; set; }
        private IBuffer _nextBuffer { get; set; }
        private IBuffer _prevBuffer { get; set; }
        private VideoProcess _video { get; set; }
        private static BufferDealer _bufferDealer { get; set; }
        public static Task Task;
        private BufferDealer()
        {
            _video = VideoProcess.GetInstance();
            _nextBuffer = NextBuffer.GetInstance();
            _prevBuffer = PrevBuffer.GetInstance();
            _renderer = new Renderer();
        }
        public static BufferDealer GetInstance()
        {
            if (_bufferDealer == null)
            {
                _bufferDealer = new BufferDealer();
            }
            return _bufferDealer;
        }
        private bool IsFrameAvailable(int index,out byte[] stream)
        {
            if (_nextBuffer.TryGetFrame(index, out byte[] Frame) || _prevBuffer.TryGetFrame(index, out Frame))
            {
                stream = Frame;
                return true;
            }
            stream = null;
            return false;
        }
        public void RunTask()
        {
            Task = new Task(_bufferDealer.Observer);
            Task.Start();
        }
        /// <summary>
        /// This method takes care of the rendering process and Buffering details.
        /// </summary>
        public void Observer()
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
                        _renderer.Render(Frame);
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
    }
}
