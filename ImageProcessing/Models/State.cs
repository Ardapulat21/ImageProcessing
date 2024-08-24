using ImageProcessing.Enum;
namespace ImageProcessing.Models
{
    public class State
    {
        public static DecodingProcess DecodingProcess = DecodingProcess.None;
        public static RenderingProcess RenderingProcess = RenderingProcess.None;
        public static ProcessingProcess ProcessingProcess = ProcessingProcess.None;
        public static int DecodedFrameIndex { get; set; } = 0;
        public static int RenderedFrameIndex { get; set; } = 0;
        public static int TotalProcessedFrames { get; set; } = 0;
        public static int ProcessedFrameIndex { get; set; } = 0;
        public static int SliderValue { get; set; } = 0;
    }
}
