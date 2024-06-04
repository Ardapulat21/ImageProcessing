using System.Collections.Concurrent;

namespace ImageProcessing.Models
{
    public class Buffering
    {
        public int Size { get => Buffer.Count; set { } }
        public int BUFFER_SIZE { get => 100; private set { } }

        public static ConcurrentQueue<Frame> Buffer = new ConcurrentQueue<Frame>();

        static Buffering buffer;
        private Buffering() { }
        public static Buffering GetInstance()
        {
            if (buffer == null)
                buffer = new Buffering();

            return buffer;
        }
        public void Enqueue(Frame frame) 
        {
            Buffer.Enqueue(frame);
        }
        public bool Dequeue(out Frame frame)
        {
            if (!Buffer.TryDequeue(out var stream))
            {
                frame = null;
                return false;
            }
            frame = stream;
            return true;
        }
    }
}
