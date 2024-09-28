using FFMediaToolkit.Graphics;
using ImageProcessing.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Services.Buffers
{
    public abstract class Buffer
    {
        public ConcurrentDictionary<int, byte[]> Dictionary = new ConcurrentDictionary<int, byte[]>();
        public static int BUFFER_SIZE { get => 100; private set { } }
        public int Size { get => Dictionary.Count; set { } }
        public abstract void Insert(int key, byte[] frame);
        public abstract bool TryGetFrame(int key, out byte[] frame);
        public void Update(int key, byte[] frame)
        {
            if (Dictionary.ContainsKey(key))
                Dictionary[key] = frame;
        }
        public byte[] ElementAt(int key)
        {
            if (Dictionary.ContainsKey(key))
                return Dictionary[key];

            return null;
        }
        public void Clear()
        {
            Dictionary.Clear();
        }
        public void Push(ImageData imageData, int decodedFrameIndex)
        {
            Bitmap bitmap = imageData.ToBitmap();
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                Insert(decodedFrameIndex - 1, stream.ToArray());
            }
            bitmap.Dispose();
            GC.Collect();
        }
    }
}
