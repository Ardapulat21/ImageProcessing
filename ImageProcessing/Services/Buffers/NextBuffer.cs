using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageProcessing.Services.Buffers
{
    public class NextBuffer : IBuffer
    {
        PrevBuffer PrevBuffer;
        public static int BUFFER_SIZE { get => 100; private set { } }
        public int Size { get => Queue.Count; private set { } }
        public ConcurrentQueue<Frame> Queue = new ConcurrentQueue<Frame>();
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
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} is waiting for the frame to be rendered", ConsoleColor.Red);
                Thread.Sleep(100);
            }
            Queue.Enqueue(frame);
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
