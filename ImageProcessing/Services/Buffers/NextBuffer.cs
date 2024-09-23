using ImageProcessing.Services.IO;
using System;
using System.Linq;
using System.Threading;
namespace ImageProcessing.Services.Buffers
{
    public class NextBuffer : Buffer
    {
        Buffer _prevBuffer;
        public override bool TryGetFrame(int key,out byte[] frame)
        {
            try
            {
                int minKey = Dictionary.Keys.Min();
                while (key > minKey && Dictionary.TryRemove(minKey, out var frameToBeTransferred))
                {
                    _prevBuffer.Insert(minKey, frameToBeTransferred);
                }
                if (Dictionary.TryRemove(key, out var stream))
                {
                    frame = stream;
                    _prevBuffer.Insert(key, frame);
                    return true;
                }
                LoggerService.Info($"{key} can not be found in dictionary");
                frame = null;
                return false;
            }
            catch (Exception ex)
            {
                LoggerService.Error(ex.Message);
                frame = null;
                return false;
            }
        }
        public override void Insert(int key, byte[] frame)
        {
            while (Size > BUFFER_SIZE)
            {
                Thread.Sleep(100);
            }
            Dictionary.TryAdd(key, frame);
        }
        
        #region Singleton
        static NextBuffer _buffer;
        public static NextBuffer GetInstance()
        {
            if (_buffer == null)
            {
                _buffer = new NextBuffer();
            }
            return _buffer;
        }
        private NextBuffer()
        {
            _prevBuffer = PrevBuffer.GetInstance();
        }
        #endregion
    }
}
