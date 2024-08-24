using System.Threading;
using ImageProcessing.Services;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.VideoProcessing;
namespace ImageProcessing.Interfaces
{
    public class ThreadManager
    {
        private static ThreadManager _threadManager;
        private Decoder _decoder;
        private Processor _processor;
        private BufferDealer _bufferDealer;

        private Thread DecoderThread;
        private Thread ProcessorThread;
        private Thread ObserverThread;
        private ThreadManager()
        {
            _decoder = Decoder.GetInstance();
            _processor = Processor.GetInstance();
            _bufferDealer = BufferDealer.GetInstance();
            DecoderThread = new Thread(_decoder.Decode);
            ProcessorThread = new Thread(_processor.Process);
            ObserverThread = new Thread(_bufferDealer.Observer);
        }

        public void DecoderStart(int index)
        {
            if(DecoderThread.IsAlive)
                DecoderThread.Join();
            DecoderThread.Start(index);
        }
        public void ObserverStart()
        {
            if(ObserverThread.IsAlive)
                ObserverThread.Join();
            ObserverThread.Start();
        }
        public void ProcessorStart()
        {
            if(ProcessorThread.IsAlive)
                ProcessorThread.Join();
            ProcessorThread.Start();
        }
        public static ThreadManager GetInstance()
        {
            if(_threadManager == null)
                _threadManager = new ThreadManager();
            return _threadManager;
        }

    }
}
