using ImageProcessing.Enum;
using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.IO;
using System.Collections.Concurrent;
using System.Linq;

namespace ImageProcessing.Services.Buffers
{
    public class PrevBuffer : IBuffer
    {
        private ConcurrentDictionary<int, byte[]> Dictionary = new ConcurrentDictionary<int, byte[]>();
        public static int BUFFER_SIZE { get => 100; private set { } }
        public int Size { get => Dictionary.Count; set { } }
        public bool TryGetFrame(int key, out byte[] frame)
        {
            if (Dictionary.TryGetValue(key, out byte[] stream))
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
                int minKey = Dictionary.Keys.Min();
                Dictionary.TryRemove(minKey, out byte[] stream);
                ConsoleService.WriteLine($"{minKey} is removed from Prev Buffer|Prev buffer size: {Size}", Color.Red);
            }
            Dictionary.TryAdd(key, frame);
        }
        public byte[] ElementAt(int index)
        {
            return Dictionary[index];
        }
        public void Clear()
        {
            Dictionary.Clear();
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