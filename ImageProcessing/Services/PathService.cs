using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Services
{
    public static class PathService
    {
        public static string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        public static string videoPath = @"C:\Users\Arda\Desktop\Videos\FrameCounterShort.mp4";

    }
}
