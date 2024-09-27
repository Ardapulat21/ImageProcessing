using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.IO;
using ImageProcessing.Services.MotionDetection;
using ImageProcessing.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Buffer = ImageProcessing.Services.Buffers.Buffer;

namespace ImageProcessing.Services.VideoProcessing
{
    public class Processor : IProcessor ,IRunner ,ICanceller 
    {
        private static MotionDetector _motionDetector;
        public void Process() 
        {
            State.ProcessingProcess = Enum.ProcessingProcess.Processing;
            int numberOfFrames = Metadata.NumberOfFrames;
            int processedFrameIndex = State.ProcessedFrameIndex;
            while (!CancellationTokenSource.IsCancellationRequested && processedFrameIndex < numberOfFrames)
            {
                try
                {
                    processedFrameIndex = State.ProcessedFrameIndex;
                    if (processedFrameIndex >= State.DecodedFrameIndex)
                    {
                        ConsoleService.WriteLine("Processor is waiting", IO.Color.Yellow);
                        Thread.Sleep(100);
                        continue;
                    }
                    ProcessFrame(processedFrameIndex);
                    State.ProcessedFrameIndex++;
                }
                catch { }
            }
            State.ProcessingProcess = Enum.ProcessingProcess.Done;
            ConsoleService.WriteLine("Processing has done.",IO.Color.Red);
        }
        private static void ProcessFrame(int processedFrameIndex)
        {
            var bitmapArray = BufferUtils.ElementAt(processedFrameIndex);
            using (MemoryStream ms = new MemoryStream(bitmapArray))
            {
                Bitmap bitmap = new Bitmap(ms);
                _motionDetector.ProcessFrame(bitmap);
                bitmap.Save(ms, ImageFormat.Bmp);
                BufferUtils.Update(processedFrameIndex, ms.ToArray());
                bitmap.Dispose();
                GC.Collect();
            }
        }
        #region Task
        public Task Task;
        public CancellationTokenSource CancellationTokenSource;
        public CancellationToken CancellationToken;
        public async Task Run()
        {
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
            try
            {
                Task = Task.Run(Process, CancellationToken);
                await Task;
            }
            catch { }
        }
        public async Task Cancel()
        {
            if (Task.Status == TaskStatus.WaitingForActivation || Task.Status == TaskStatus.Running)
            {
                CancellationTokenSource.Cancel();
                try
                {
                    await Task;
                }
                catch { }
            }
        }
        #endregion
        #region Singleton
        private static Processor _processor;
        private Processor()
        {
            _motionDetector = new MotionDetector();
        }
        public static Processor GetInstance()
        {
            if (_processor == null)
                _processor = new Processor();

            return _processor;
        }
        #endregion
    }
}
