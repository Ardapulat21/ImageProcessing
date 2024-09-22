using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Interfaces
{
    public interface IVideoProcess
    {
        void Initialize(MainViewModel _mainViewModel);
        void OpenVideo();
        void ResetVideo();
        void Dispose();
    }
}
