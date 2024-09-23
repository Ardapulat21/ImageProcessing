using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Interfaces
{
    interface ICanceller
    {
        Task Cancel();
    }
}
