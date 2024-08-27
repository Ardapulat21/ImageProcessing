using ImageProcessing.Enum;
using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using System.Collections.Concurrent;
using System.Linq;

namespace ImageProcessing.Services.Buffers
{
    public class PrevBuffer : IBuffer
    {
        public static int BUFFER_SIZE { get => 100; private set { } }
        public static int Size { get => Dictionary.Count; set { } }
        private static ConcurrentDictionary<int, byte[]> Dictionary = new ConcurrentDictionary<int, byte[]>();
        public bool TryGetFrame(int key, out byte[] frame)
        {
            if (Dictionary.TryRemove(key, out byte[] stream))
            {
                frame = stream;
                return true;
            }
            frame = null;
            return false;
        }
        public void Insert(int key, byte[] frame)
        {
            if (Size >= BUFFER_SIZE)
            {
                TryGetFrame(0, out byte[] RemovedFrame);
            }
            Dictionary.TryAdd(key, frame);
        }
        public byte[] ElementAt(int index)
        {
            return Dictionary[index];
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