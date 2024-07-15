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
        public static int Size { get => Queue.Count; set { } }
        public static ConcurrentQueue<Frame> Queue = new ConcurrentQueue<Frame>();
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
            }
            Queue.Enqueue(frame);
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