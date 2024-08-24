using ImageProcessing.Models;
using ImageProcessing.Services.IO;
using System.Threading;
namespace ImageProcessing.Services.Buffers
{
    public class BufferDealer
    {
        private Renderer _renderer { get; set; }
        private NextBuffer _nextBuffer { get; set; }
        private VideoProcess _video { get; set; }
        private static BufferDealer _dealer { get; set; }
        private BufferDealer()
        {
            _video = VideoProcess.GetInstance();
            _nextBuffer = NextBuffer.GetInstance();
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
                State.RenderingProcess = Enum.RenderingProcess.Processing;
                while (true)
                {
                    if (_nextBuffer.TryGetFrame(State.RenderedFrameIndex,out byte[] Frame))
                    {
                        _renderer.Render(Frame);
                        State.RenderedFrameIndex++;
                        ConsoleService.WriteLine($"{State.RenderedFrameIndex}'s frame rendered.",Color.Green);
                    }
                    else
                    {
                        if (State.DecodingProcess == Enum.DecodingProcess.Done)
                        {
                            State.RenderingProcess = Enum.RenderingProcess.Done;
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
