using ImageProcessing.Interfaces;
using ImageProcessing.Services.IO;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace ImageProcessing.Services.Buffers
{
    public class NextBuffer : IBuffer
    {
        static NextBuffer _buffer;
        PrevBuffer _prevBuffer;
        public static int BUFFER_SIZE { get => 100; private set { } }
        public int Size { get => Dictionary.Count; private set { } }

        private ConcurrentDictionary<int,byte[]> Dictionary = new ConcurrentDictionary<int, byte[]>();
        public bool TryGetFrame(int key,out byte[] frame)
        {
            if (Dictionary.TryRemove(key, out var stream))
            {
                frame = stream;
                _prevBuffer.Insert(key, frame);
                return true;
            }
            LoggerService.Info($"{key} can not be found in dictionary");
            frame = null;
            return false;
        }
        public void Update(int key, byte[] frame)
        {
            if (Dictionary.ContainsKey(key))
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
        #region Singleton
        public static NextBuffer GetInstance()
        {
            if (_buffer == null)
            {
                _buffer = new NextBuffer();
            }
            return _buffer;
        }
        private NextBuffer()
        {
            _prevBuffer = PrevBuffer.GetInstance();
        }
        #endregion
    }
}
