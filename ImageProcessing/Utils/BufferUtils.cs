using ImageProcessing.Services.Buffers;
using Buffer = ImageProcessing.Services.Buffers.Buffer;

namespace ImageProcessing.Utils
{
    public static class BufferUtils
    {
        private static Buffer _nextBuffer = NextBuffer.GetInstance();
        private static Buffer _prevBuffer = PrevBuffer.GetInstance();
        public static void Reset()
        {
            _nextBuffer.Clear();
            _prevBuffer.Clear();
        }
        public static bool IsFrameAvailable(int index, out byte[] stream)
        {
            if (_nextBuffer.TryGetFrame(index, out byte[] Frame) || _prevBuffer.TryGetFrame(index, out Frame))
            {
                stream = Frame;
                return true;
            }
            stream = null;
            return false;
        }
        public static void Update(int key, byte[] Frame)
        {
            _nextBuffer.Update(key, Frame);
            _prevBuffer.Update(key, Frame);
        }
        public static byte[] ElementAt(int key)
        {
            return _nextBuffer.ElementAt(key) ?? _prevBuffer.ElementAt(key);
        }
    }
}
