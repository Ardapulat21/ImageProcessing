using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Services
{
    public static class PathService
    {
        public static string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        public static string MotionDetectorFolder = Path.Combine(AppDataFolder, "MotionDetector");
        public static string FFMPEGFolder = Path.Combine(MotionDetectorFolder, "FFMPEG");
        public static string LogFolder = Path.Combine(MotionDetectorFolder, "Logs");
        public static string CreateFile(string folderPath,string fileName)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var appropriateFile = GetAppropriatePath(folderPath, fileName);
            var filePath = Path.Combine(folderPath, appropriateFile);

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();
            return appropriateFile;
        }
        private static string GetAppropriatePath(string folderPath, string fileName)
        {
            string filePath;
            int counter = 0;
            string fullFileName;
            while (true)
            {
                fullFileName = $"{fileName}_{counter}.txt";
                filePath = Path.Combine(folderPath, fullFileName);
                counter++;
                if (!File.Exists(filePath))
                    return filePath;
            }
        }
    }
}
