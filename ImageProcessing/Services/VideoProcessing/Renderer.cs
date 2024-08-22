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
    public class Renderer : IRenderer
    {
        VideoProcess Video;
        public Renderer()
        {
            Video = VideoProcess.GetInstance();
        }
        public void Render(Frame Frame)
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
                GC.Collect();
            }
        }
    }
}
