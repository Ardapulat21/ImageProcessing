using ImageProcessing.Models;
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
        VideoProcessing Video = VideoProcessing.GetInstance();
        Buffering Buffer = Buffering.GetInstance();
        public void Start()
        {
            MemoryStream memory = new MemoryStream();
            try
            {
                while (true)
                {
                    if (Buffer.Dequeue(out var Frame))
                    {
                        using (var ms = new MemoryStream(Frame.Bitmap))
                        {
                            var bitmap = (Bitmap)Image.FromStream(ms);
                            BitmapImage bitmapImage = BitmapUtils.Convert(bitmap);
                            bitmapImage.Freeze();
                            Dispatcher.CurrentDispatcher.Invoke(() => Video.MainViewModel.ImageSource = bitmapImage);
                            Thread.Sleep(1000 / (int)Video.videoStreamInfo.AvgFrameRate);
                            Video.MainViewModel.SliderValue++;
                            bitmap.Dispose();
                            bitmapImage = null;
                        }
                    }
                    else
                    {
                        if (Video.ProcessType == Enum.ProcessType.Done)
                        {
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
                memory.Dispose();
                Video.Dispose();
            }
        }
    }
}
