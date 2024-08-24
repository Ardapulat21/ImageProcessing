using ImageProcessing.Enum;
namespace ImageProcessing.Models
{
    public class State
    {
        private static int _totalProcessedFrames = 0;
        public static DecodingProcess DecodingProcess = DecodingProcess.None;
        public static RenderingProcess RenderingProcess = RenderingProcess.None;
        public static ProcessingProcess ProcessingProcess = ProcessingProcess.None;
        public static int DecodedFrameIndex { get; set; } = 0;
        public static int RenderedFrameIndex { get; set; } = 0;
        public static int TotalProcessedFrames
        {
            get
            {
                if(RenderedFrameIndex > _totalProcessedFrames)
                {
                    _totalProcessedFrames = RenderedFrameIndex;
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
