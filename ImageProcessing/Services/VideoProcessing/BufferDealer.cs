using ImageProcessing.Models;
using ImageProcessing.Services.IO;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace ImageProcessing.Services.Buffers
{
    public class BufferDealer
    {
        public int SeekFrame = 0;
        public int RenderingFrameIndex { get; set; } = 0;
        private Renderer _renderer { get; set; }
        private NextBuffer _nextBuffer { get; set; }
        private PrevBuffer _prevBuffer { get; set; }
        private VideoProcess _video { get; set; }
        private static BufferDealer _dealer { get; set; }
        private BufferDealer()
        {
            _video = VideoProcess.GetInstance();
            _nextBuffer = NextBuffer.GetInstance();
            _prevBuffer = PrevBuffer.GetInstance();
            _renderer = new Renderer();
        }
        public static BufferDealer GetInstance()
        {
            if (_dealer == null)
            {
                _dealer = new BufferDealer();
            }
            return _dealer;
        }
        /// <summary>
        /// This method takes care of the rendering process and Buffering details.
        /// </summary>
        public void Observer()
        {
            try
            {
                _video.State.RenderingProcess = Enum.RenderingProcess.Processing;
                while (true)
                {
                    if (_nextBuffer.Dequeue(out Frame Frame))
                    {
                        _renderer.Render(Frame);
                        RenderingFrameIndex = Frame.FrameIndex;
                        Metadata.RenderedFrameIndex++;
                        ConsoleService.WriteLine($"{Metadata.RenderedFrameIndex}'s frame rendered.",Color.Green);
                    }
                    else
                    {
                        if (_video.State.DecodingProcess == Enum.DecodingProcess.Done)
                        {
                            _video.State.RenderingProcess = Enum.RenderingProcess.Done;
                            ConsoleService.WriteLine("Rendering process is done.",Color.Green);
                            _video.Dispose();
                            break;
                        }
                        ConsoleService.WriteLine($"Buffer Dealer is being waited for new frames to be added to next Buffer",Color.Yellow);
                        Thread.Sleep(100);
                    }
                }
            }
            catch { }
        }
    }
}
