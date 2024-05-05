using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Models
{
    public class Frame
    {
        public byte[] Bitmap { get; set; }
        public int FrameIndex { get; set; }
        public Frame(byte[] Bitmap,int FrameIndex)
        {
            this.Bitmap = Bitmap;
            this.FrameIndex = FrameIndex;
        }

    }
}
