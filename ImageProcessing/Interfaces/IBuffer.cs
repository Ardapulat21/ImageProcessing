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
        void Enqueue(Frame frame);
        bool Dequeue(out Frame frame);
    }
}
