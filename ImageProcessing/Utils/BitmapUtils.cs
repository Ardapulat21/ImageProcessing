using FFMediaToolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;
using System.Drawing.Imaging;
namespace ImageProcessing
{
    public static class BitmapUtils
    {
        public static unsafe Bitmap ToBitmap(this ImageData bitmap)
        {
            fixed (byte* p = bitmap.Data)
            {
                return new Bitmap(bitmap.ImageSize.Width, bitmap.ImageSize.Height, bitmap.Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, new IntPtr(p));
            }
        }
        public static BitmapImage Convert(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream memory = new MemoryStream())
            {
                // Convert Bitmap to memory stream
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                // Create BitmapImage
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
    }
}
