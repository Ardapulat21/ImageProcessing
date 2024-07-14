using ImageProcessing.Models;
using ImageProcessing.Services.VideoProcessing;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ImageProcessing.Services
{
    public class Renderer 
    {
        VideoProcess Video = VideoProcess.GetInstance();
        Buffering Buffer = Buffering.GetInstance();
        public static int renderedFrameIndex = 0;
        public void Render()
        {
            try
            {
                if (!Video.isInitialized)
                {
                    Console.WriteLine("The video has not been initialized yet.");
                    return;
                }
                while (true)
                {
                    if (renderedFrameIndex >= Processor.totalProcessedFrames && Video.State.ProcessingProcess != Enum.ProcessingProcess.Done)
                    {
                        Thread.Sleep(50);
                        Console.WriteLine($"Renderer is being waited.Rendered Frame: {renderedFrameIndex} | Processed Frame: {Processor.totalProcessedFrames}");
                        continue;
                    }
                    if (Buffer.Dequeue(out var Frame))
                    {
                        using (var ms = new MemoryStream(Frame.Bitmap))
                        {
                            var bitmap = (Bitmap)Image.FromStream(ms);
                            BitmapImage bitmapImage = BitmapUtils.Convert(bitmap);
                            bitmapImage.Freeze();
                            Dispatcher.CurrentDispatcher.Invoke(() => Video.MainViewModel.ImageSource = bitmapImage);
                            Thread.Sleep(1000 / (int)Video.VideoStreamInfo.AvgFrameRate);
                            Video.MainViewModel.SliderValue++;
                            bitmap.Dispose();
                            bitmapImage = null;
                            renderedFrameIndex++;
                            Processor.processedFrameIndex--;
                        }
                    }
                    else
                    {
                        if (Video.State.DecodingProcess == Enum.DecodingProcess.Done)
                        {
                            Video.State.RenderingProcess = Enum.RenderingProcess.Done;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Rendering process is done.");
                            break;
                        }
                        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} Thread has been waiting for new frames to be added to Buffer.");
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Video.Dispose();
            }
        }
    }
}
