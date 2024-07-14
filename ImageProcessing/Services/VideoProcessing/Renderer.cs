using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
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
        VideoProcess Video;
        NextBuffer NextBuffer;
        public static int renderedFrameIndex = 0;
        public Renderer()
        {
            Video = VideoProcess.GetInstance();
            NextBuffer = NextBuffer.GetInstance();
        }
        public void Render()
        {
            try
            {
                if (!Video.isInitialized)
                {
                    System.Console.WriteLine("The video has not been initialized yet.");
                    return;
                }
                while (true)
                {
                    if (renderedFrameIndex >= Processor.totalProcessedFrames && Video.State.ProcessingProcess != Enum.ProcessingProcess.Done)
                    {
                        Thread.Sleep(50);
                        Console.WriteLine($"Renderer is being waited.Rendered Frame: {renderedFrameIndex} | Processed Frame: {Processor.totalProcessedFrames}",ConsoleColor.Green);
                        continue;
                    }
                    if (NextBuffer.Dequeue(out var Frame))
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
                            Console.WriteLine("Rendering process is done.", ConsoleColor.Green);
                            break;
                        }
                        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} Thread has been waiting for new frames to be added to Buffer.", ConsoleColor.Green);
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            finally
            {
                Video.Dispose();
            }
        }
    }
}
