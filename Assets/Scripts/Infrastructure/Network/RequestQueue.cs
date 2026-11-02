using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;

namespace ClickerTestTask.Infrastructure.Network
{
    public sealed class RequestQueue : IDisposable
    {
        private readonly LinkedList<IQueuedRequest> _pending = new();

        private IQueuedRequest _currentRequest;
        private bool _isProcessing;
        private bool _isDisposed;

        public RequestHandle<T> Enqueue<T>(
            Func<System.Threading.CancellationToken, UniTask<T>> operation)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(RequestQueue));

            var request = new QueuedRequest<T>(operation);

            _pending.AddLast(request);

            if (!_isProcessing)
                ProcessQueueAsync().Forget();

            return new RequestHandle<T>(this, request);
        }

        public void Cancel(Guid id)
        {
            if (_currentRequest != null && _currentRequest.Id == id)
            {
                _currentRequest.Cancel();
                return;
            }

            var node = _pending.First;

            while (node != null)
            {
                if (node.Value.Id == id)
                {
                    IQueuedRequest request = node.Value;

                    _pending.Remove(node);

                    request.Cancel();
                    request.Dispose();

                    return;
                }

                node = node.Next;
            }
        }

        public void CancelAll()
        {
            _currentRequest?.Cancel();

            while (_pending.Count > 0)
            {
                IQueuedRequest request = _pending.First.Value;

                _pending.RemoveFirst();

                request.Cancel();
                request.Dispose();
            }
        }

        private async UniTask ProcessQueueAsync()
        {
            _isProcessing = true;

            try
            {
                while (_pending.Count > 0 && !_isDisposed)
                {
                    IQueuedRequest request = _pending.First.Value;
                    _pending.RemoveFirst();

                    if (request.IsCancellationRequested)
                    {
                        request.Dispose();
                        continue;
                    }

                    _currentRequest = request;

                    try
                    {
                        await request.ExecuteAsync(
                            System.Threading.CancellationToken.None);
                    }
                    finally
                    {
                        _currentRequest = null;
                    }
                }
            }
            finally
            {
                _isProcessing = false;

                if (_pending.Count > 0 && !_isDisposed)
                    ProcessQueueAsync().Forget();
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            CancelAll();
        }
    }
}