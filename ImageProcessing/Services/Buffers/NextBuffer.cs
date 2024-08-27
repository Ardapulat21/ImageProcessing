using ImageProcessing.Interfaces;
using ImageProcessing.Services.IO;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace ImageProcessing.Services.Buffers
{
    public class NextBuffer : IBuffer
    {
        PrevBuffer PrevBuffer;
        public static int BUFFER_SIZE { get => 100; private set { } }
        public int Size { get => Dictionary.Count; private set { } }

        private ConcurrentDictionary<int,byte[]> Dictionary = new ConcurrentDictionary<int, byte[]>();
        public bool TryGetFrame(int key,out byte[] frame)
        {
            if (Dictionary.TryRemove(key, out var stream))
            {
                frame = stream;
                PrevBuffer.Insert(PrevBuffer.Size, frame);
                return true;
            }
            LoggerService.Info($"{key} can not be found in dictionary");
            frame = null;
            return false;
        }
        public void Update(int key, byte[] frame)
        {
            Dictionary[key] = frame;
        }
        public void Insert(int key, byte[] frame)
        {
            while (Size > BUFFER_SIZE)
            {
                Thread.Sleep(100);
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
        static NextBuffer Buffer;
        public static NextBuffer GetInstance()
        {
            if (Buffer == null)
            {
                Buffer = new NextBuffer();
            }
            return Buffer;
        }
        private NextBuffer()
        {
            PrevBuffer = PrevBuffer.GetInstance();
        }
        #endregion
    }
}
