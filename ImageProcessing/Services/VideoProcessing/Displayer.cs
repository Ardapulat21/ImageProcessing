using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ImageProcessing.Services
{
    public class Displayer
    {
        IVideoProcess Video;
        public Displayer()
        {
            Video = VideoProcess.GetInstance();
        }
        public void Display(byte[] Frame)
        {
            using (var ms = new MemoryStream(Frame))
            {
                var bitmap = (Bitmap)Image.FromStream(ms);
                BitmapImage bitmapImage = BitmapUtils.Convert(bitmap);
                bitmapImage.Freeze();
                Dispatcher.CurrentDispatcher.Invoke(() => Video.MainViewModel.ImageSource = bitmapImage);
                Thread.Sleep(1000 / (int)Video.VideoStreamInfo.AvgFrameRate);
                Video.MainViewModel.SliderValue++;
                bitmap.Dispose();
                bitmapImage = null;
                GC.Collect();
            }
        }
    }
}
