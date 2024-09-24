using ImageProcessing.Services.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageProcessing.ParallelProcessing
{
    public abstract class TaskService
    {
        public Task Task;
        public CancellationTokenSource CancellationTokenSource;
        public CancellationToken CancellationToken;
        public async Task CreateTask(Action action)
        {
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
            try
            {
                Task = Task.Run(action, CancellationToken);
                await Task;
            }
            catch (OperationCanceledException){ }
            catch { }
        }
        public async Task Cancel()
        {
            if (Task.Status == TaskStatus.WaitingForActivation)
            {
                CancellationTokenSource.Cancel();
                try
                {
                    await Task;
                }
                catch (OperationCanceledException) { }
                catch { }
            }
        }
        public abstract void Reset();
    }
}
