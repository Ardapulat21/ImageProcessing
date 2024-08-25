using System.Threading;
using ImageProcessing.Services;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.IO;
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
        }

        public void DecoderStart(int index)
        {
            if(DecoderThread != null && DecoderThread.IsAlive)
            {
                ConsoleService.WriteLine("Decoder Thread Aborted",Color.Red);
                DecoderThread.Join();
            }
            DecoderThread = new Thread(_decoder.Decode);
            DecoderThread.Start(index);
        }
        public void ObserverStart()
        {
            if(ObserverThread != null && ObserverThread.IsAlive)
                ObserverThread.Join();
            ObserverThread = new Thread(_bufferDealer.Observer);
            ObserverThread.Start();
        }
        public void ProcessorStart()
        {
            if(ProcessorThread != null && ProcessorThread.IsAlive)
                ProcessorThread.Join();
            ProcessorThread = new Thread(_processor.Process);
            ProcessorThread.Start();
        }
        public void AbortAll()
        {
            DecoderThread.Abort();
            ObserverThread.Abort();
            ProcessorThread.Abort();
        }
        public static ThreadManager GetInstance()
        {
            if(_threadManager == null)
                _threadManager = new ThreadManager();
            return _threadManager;
        }

    }
}
