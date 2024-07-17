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
        public static int Size { get => Queue.Count; set { } }
        public Fullness Fullness { get; set; } = Fullness.Empty;
        private static ConcurrentQueue<Frame> Queue = new ConcurrentQueue<Frame>();
        public string Text = "Is working.";
        public bool Dequeue(out Frame frame)
        {
            if (!Queue.TryDequeue(out var stream))
            {
                frame = null;
                return false;
            }
            frame = stream;
            return true;
        }
        public void Enqueue(Frame frame)
        {
            if (Size >= BUFFER_SIZE)
            {
                Dequeue(out Frame RemovedFrame);
                Fullness = Fullness.Full;
            }
            Queue.Enqueue(frame);
        }
        public Frame ElementAt(int index)
        {
            return Queue.ElementAt(index);
        }
        public void Discharge(ConcurrentQueue<Frame> queue)
        {
            while (queue.Count < 100)
            {
                Dequeue(out Frame frame);
                queue.Enqueue(frame);
            }
            Queue = queue;
            queue = null;
        }
        public void SetQueue(ConcurrentQueue<Frame> queue)
        {
            Queue = queue;
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