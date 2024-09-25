using FFMediaToolkit.Decoding;
using ImageProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Interfaces
{
    public interface IVideoProcess
    {
        bool IsInitialized { get; set; }
        MainViewModel MainViewModel { get; set; }
        MediaFile MediaFile { get; set; }
        VideoStreamInfo VideoStreamInfo { get; set; }
        void Initialize(MainViewModel _mainViewModel);
        void OpenVideo();
        void Dispose();
    }
}
