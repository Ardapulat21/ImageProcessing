using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Interfaces
{
    public interface IDecoder
    {
        void Decode(int fromIndex);
    }
}
