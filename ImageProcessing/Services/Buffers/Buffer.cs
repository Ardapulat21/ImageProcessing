using ImageProcessing.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Services.Buffers
{
    public abstract class Buffer
    {
        public ConcurrentDictionary<int, byte[]> Dictionary = new ConcurrentDictionary<int, byte[]>();
        public static int BUFFER_SIZE { get => 100; private set { } }
        public int Size { get => Dictionary.Count; set { } }
        public abstract void Insert(int key, byte[] frame);
        public abstract bool TryGetFrame(int key, out byte[] frame);
        public virtual void Update(int key, byte[] frame)
        {
            if (Dictionary.ContainsKey(key))
                Dictionary[key] = frame;
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
    }
}
