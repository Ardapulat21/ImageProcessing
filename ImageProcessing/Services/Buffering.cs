using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Models
{
    public class Buffering
    {
        public int Count { get => Buffer.Count; set { } }
        public int BUFFER_SIZE { get => 100; private set { } }

        public static ConcurrentQueue<byte[]> Buffer = new ConcurrentQueue<byte[]>();

        static Buffering buffer;
        private Buffering() { }
        public static Buffering GetInstance()
        {
            if (buffer == null)
                buffer = new Buffering();

            return buffer;
        }
        public void Enqueue(byte[] stream) 
        {
            Buffer.Enqueue(stream);
        }
        public bool TryDequeue(out byte[] matrix)
        {
            if (!Buffer.TryDequeue(out var stream))
            {
                matrix = null;
                return false;
            }
            matrix = stream;
            return true;
        }
    }
}
