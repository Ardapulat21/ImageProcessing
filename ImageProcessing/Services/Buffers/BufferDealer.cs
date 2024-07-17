using ImageProcessing.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace ImageProcessing.Services.Buffers
{
    public class BufferDealer 
    {
        private int _seekFrame = 0;
        public int SeekFrame {
            get => _seekFrame;
            set
            {
                _seekFrame = value;
            }
        }
        public int RenderingFrameIndex { get; set; } = 0;
        NextBuffer NextBuffer { get; set; }
        public PrevBuffer PrevBuffer { get; set; }
        public VideoProcess Video { get; set; }
        public void Flow()
        {
            try
            {
                if (!Video.isInitialized)
                {
                    Console.WriteLine("The video has not been initialized yet.");
                    return;
                }
                Video.State.RenderingProcess = Enum.RenderingProcess.Processing;
                while (true)
                {
                    if (NextBuffer.Dequeue(out Frame Frame))
                    {
                        if (SeekFrame == 0)
                        {
                            Renderer.Render(Frame);
                            RenderingFrameIndex = Frame.FrameIndex;
                        }
                        else if (SeekFrame > 0)
                        {
                            SeekFrame--;
                        }
                        else if (SeekFrame < 0)
                        {
                            SeekBackward();
                        }
                    }
                    else
                    {
                        if (Video.State.DecodingProcess == Enum.DecodingProcess.Done)
                        {
                            Video.State.RenderingProcess = Enum.RenderingProcess.Done;
                            Console.WriteLine("Rendering process is done.", ConsoleColor.Green);
                            break;
                        }
                        Console.WriteLine($"Buffer Dealer is being waited for new frames to be added to next Buffer", ConsoleColor.Green);
                        Thread.Sleep(100);
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
        public void SeekBackward()
        {
            while (SeekFrame < 0)
            {
                try
                {
                    Frame frame = PrevBuffer.ElementAt(PrevBuffer.Size + SeekFrame);
                    Renderer.Render(frame);
                    SeekFrame++;
                    RenderingFrameIndex = frame.FrameIndex;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        #region Singleton
        private static BufferDealer Dealer { get; set; }
        private BufferDealer()
        {
            Video = VideoProcess.GetInstance();
            NextBuffer = NextBuffer.GetInstance();
            PrevBuffer = PrevBuffer.GetInstance();
        }
        public static BufferDealer GetInstance()
        {
            if (Dealer == null)
            {
                Dealer = new BufferDealer();
            }
            return Dealer;
        }
        #endregion

    }
}
