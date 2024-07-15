using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageProcessing.Models;
using ImageProcessing.Services.VideoProcessing;
namespace ImageProcessing.Services.Buffers
{
    public class BufferDealer
    {
        public int SeekFrame { get; set; } = 0;
        public int RenderingFrameIndex { get; set; } = 0;
        PrevBuffer PrevBuffer { get; set; }
        NextBuffer NextBuffer { get; set; }
        public VideoProcess Video { get; set; }
        private static BufferDealer Dealer { get; set; }
        private BufferDealer()
        {
            Video = VideoProcess.GetInstance();
            PrevBuffer = PrevBuffer.GetInstance();
            NextBuffer = NextBuffer.GetInstance();
        }
        public static BufferDealer GetInstance()
        {
            if(Dealer == null)
            {
                Dealer = new BufferDealer();
            }
            return Dealer;
        }
        public void SeekBackward()
        {
            try
            {
                while (SeekFrame < 0)
                {
                    Renderer.Render(PrevBuffer.Queue.ElementAt(SeekFrame));
                    SeekFrame++;
                }
            }
            catch (Exception ex)
            {

            }
           
        }
        public void Flow()
        {
            try
            {
                if (!Video.isInitialized)
                {
                    System.Console.WriteLine("The video has not been initialized yet.");
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
    }
}
