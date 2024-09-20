using ImageProcessing.Enum;
using System.Threading;
namespace ImageProcessing.Models
{
    public class State
    {
        private static int _totalProcessedFrames = 0;
        public static DecodingProcess DecodingProcess = DecodingProcess.None;
        public static RenderingProcess RenderingProcess = RenderingProcess.None;
        public static ProcessingProcess ProcessingProcess = ProcessingProcess.None;
        public static int DecodedFrameIndex { get; set; } = 0;
        public static int ProcessedFrameIndex
        {
            get
            {
                if(SliderValue > _totalProcessedFrames)
                {
                    _totalProcessedFrames = SliderValue;
                }
                return _totalProcessedFrames;
            }
            set
            {
                _totalProcessedFrames = value;
            }
        }
        public static int SliderValue { get; set; } = 0;
    }
}
