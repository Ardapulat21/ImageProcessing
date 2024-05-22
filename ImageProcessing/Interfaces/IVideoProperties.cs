using FFMediaToolkit.Decoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Interfaces
{
    public interface IVideoProperties
    {
        MediaFile mediaFile { get; set; }
        VideoStreamInfo videoStreamInfo { get; set; }
    }
}
