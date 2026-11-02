using Cysharp.Threading.Tasks;
using System;

namespace ClickerTestTask.Infrastructure.Network
{
    public sealed class RequestHandle<T>
    {
        private readonly RequestQueue _queue;
        private readonly QueuedRequest<T> _request;

        public Guid Id => _request.Id;
        public UniTask<T> Result => _request.Result;

        internal RequestHandle(
            RequestQueue queue,
            QueuedRequest<T> request)
        {
            _queue = queue;
            _request = request;
        }

        public void Cancel() => _queue.Cancel(Id);
    }
}