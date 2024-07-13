using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Models
{
    public class Metadata
    {
        public int NumberOfFrames { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int FPS { get; set; }

        public Metadata(int width, int height, int fPS,int numberOfFrames)
        {
            NumberOfFrames = numberOfFrames;
            Width = width;
            Height = height;
            FPS = fPS;
        }
    }
}
