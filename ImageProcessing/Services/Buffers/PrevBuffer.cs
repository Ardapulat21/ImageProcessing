using ImageProcessing.Enum;
using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.IO;
using System.Collections.Concurrent;
using System.Linq;

namespace ImageProcessing.Services.Buffers
{
    public class PrevBuffer : Buffer
    {
        public override bool TryGetFrame(int key, out byte[] frame)
        {
            if (Dictionary.TryGetValue(key, out byte[] stream))
            {
                frame = stream;
                return true;
            }
            frame = null;
            return false;
        }
        public override void Insert(int key, byte[] frame)
        {
            if (Size >= BUFFER_SIZE)
            {
                int minKey = Dictionary.Keys.Min();
                Dictionary.TryRemove(minKey, out byte[] stream);
            }
            Dictionary.TryAdd(key, frame);
        }
        #region Singleton
        static PrevBuffer Buffer;
        public static PrevBuffer GetInstance()
        {
            if (Buffer == null)
            {
                Buffer = new PrevBuffer();
            }
            return Buffer;
        }
        private PrevBuffer()
        {
        }
        #endregion
    }
}