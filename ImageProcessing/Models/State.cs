using ImageProcessing.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Models
{
    public class State
    {
        public DecodingProcess DecodingProcess = DecodingProcess.None;
        public RenderingProcess RenderingProcess = RenderingProcess.None;
        public ProcessingProcess ProcessingProcess = ProcessingProcess.None;
    }
}
