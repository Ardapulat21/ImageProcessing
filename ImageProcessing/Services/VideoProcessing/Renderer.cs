using FFMediaToolkit.Decoding;
using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ImageProcessing.Services
{
    public class Renderer : IVideoProperties,ICoder
    {
        Buffering Buffer;
        MainViewModel _mainViewModel;
        VideoProcessing Video;
        public MediaFile mediaFile { get; set; }
        public VideoStreamInfo videoStreamInfo { get; set; }

        public Renderer() 
        {
            Video = VideoProcessing.GetInstance();
            Video.Fill(this);
            _mainViewModel = Video._mainViewModel;
            Buffer = Buffering.GetInstance();
        }

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
                            Dispatcher.CurrentDispatcher.Invoke(() => _mainViewModel.ImageSource = bitmapImage);
                            Thread.Sleep(1000 / (int)videoStreamInfo.AvgFrameRate);
                            _mainViewModel.SliderValue++;
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
