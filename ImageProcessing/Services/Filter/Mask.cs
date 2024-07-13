using ImageProcessing.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessing.Services.ImageProcessing
{
    public class Mask
    {
        static VideoProcess Video = VideoProcess.GetInstance();

        public static void MaskByteArray(byte[] data,int XCoordinate,int YCoordinate,int Width,int Height)
        {
            try
            {
                for (int y = Video.Metadata.Height - YCoordinate - Height; y < Video.Metadata.Height - YCoordinate; y++)
                {
                    for (int x = XCoordinate + 20; x < XCoordinate + 20 + Width; x++)
                    {
                        data[3 * y * Video.Metadata.Width + 3 * x] = 0;
                        data[3 * y * Video.Metadata.Width + 3 * x + 1] = 0;
                        data[3 * y * Video.Metadata.Width + 3 * x + 2] = 0;
                    }
                }
            }
            catch { }
            
        }

        public static void MaskBitmap(Bitmap bitmap)
        {
            if (bitmap == null) return;
            // Lock the bitmap's bits.  

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Set every third value to 255. A 24bpp bitmap will look red.  
            for (int counter = 2; counter < rgbValues.Length; counter += 3)
            {
                if(counter / (3 * bitmap.Height) > 200 && 
                   counter / (3 * bitmap.Height) < 300 
                    )
                {
                    rgbValues[counter - 2] = 0x40;
                    rgbValues[counter - 1] = 0x40;
                    rgbValues[counter] = 0x40;
                }
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);

        }
    }
}
