using ImageProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Interfaces
{
    public interface IBuffer
    {
        void Insert(int key, byte[] frame);
        bool TryGetFrame(int key, out byte[] frame);
    }
}
