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
        MainViewModel mainViewModel;
        VideoProcessing Video;
        public MediaFile mediaFile { get; set; }
        public VideoStreamInfo videoStreamInfo { get; set; }
        public int numberOfFrames { get; set; }

        public Renderer(MainViewModel mainViewModel) 
        {
            Video = VideoProcessing.GetInstance();
            Video.Fill(this);
            Buffer = Buffering.GetInstance();
            this.mainViewModel = mainViewModel;
        }

        public void Start()
        {
            MemoryStream memory = new MemoryStream();
            try
            {
                while (true)
                {
                    if (Buffer.TryDequeue(out var matrix))
                    {
                        using (var ms = new MemoryStream(matrix))
                        {
                            var bitmap = (Bitmap)Image.FromStream(ms);
                            BitmapImage bitmapImage = BitmapUtils.Convert(bitmap);
                            bitmapImage.Freeze();
                            Dispatcher.CurrentDispatcher.Invoke(() => mainViewModel.ImageSource = bitmapImage);
                            Thread.Sleep(1000 / (int)videoStreamInfo.AvgFrameRate);
                            mainViewModel.SliderValue++;
                            bitmap.Dispose();
                            bitmapImage = null;
                        }
                    }
                    else
                    {
                        if (Video.isFinishedDecoding)
                            break;

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
