using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Services.Buffers
{
    public class PrevBuffer : IBuffer
    {
        public static int BUFFER_SIZE { get => 100; private set { } }
        public int Size { get => Queue.Count; set { } }
        public ConcurrentQueue<Frame> Queue = new ConcurrentQueue<Frame>();
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
                Console.WriteLine($"New element Removed from Previous Buffer.Element number of Previous Buffer: {Size}", ConsoleColor.Cyan);
                Dequeue(out Frame RemovedFrame);
            }
            Queue.Enqueue(frame);
            Console.WriteLine($"New element Added to Previous Buffer.Element number of Previous Buffer: {Size}",ConsoleColor.Cyan);
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