using ImageProcessing.Enum;
namespace ImageProcessing.Models
{
    public class State
    {
        public DecodingProcess DecodingProcess = DecodingProcess.None;
        public RenderingProcess RenderingProcess = RenderingProcess.None;
        public ProcessingProcess ProcessingProcess = ProcessingProcess.None;
    }
}
