using ImageProcessing.Enum;
using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace ImageProcessing.Services.Buffers
{
    public class NextBuffer : IBuffer
    {
        PrevBuffer PrevBuffer;
        public static int BUFFER_SIZE { get => 100; private set { } }
        public int Size { get => Queue.Count; private set { } }
        public Fullness Fullness { get; set; } = Fullness.Empty;
        private ConcurrentQueue<Frame> Queue = new ConcurrentQueue<Frame>();
        public bool Dequeue(out Frame frame)
        {
            if (!Queue.TryDequeue(out var stream))
            {
                frame = null;
                return false;
            }
            PrevBuffer.Enqueue(stream);
            frame = stream;
            return true;
        }

        public void Enqueue(Frame frame)
        {
            while (Size > BUFFER_SIZE)
            {
                Thread.Sleep(100);
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
