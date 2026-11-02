using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace ClickerTestTask.Infrastructure.Network
{
    public interface IQueuedRequest
    {
        Guid Id { get; }
        bool IsCancellationRequested { get; }

        void Cancel();
        void Dispose();

        UniTask ExecuteAsync(CancellationToken queueToken);
    }
}