using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace ClickerTestTask.Infrastructure.Network
{
    public sealed class QueuedRequest<T> : IQueuedRequest
    {
        private readonly Func<CancellationToken, UniTask<T>> _operation;
        private readonly UniTaskCompletionSource<T> _completionSource = new();
        private readonly CancellationTokenSource _cancellationSource = new();

        private bool _isCancellationRequested;
        private bool _isCompleted;
        private bool _isDisposed;

        public Guid Id { get; } = Guid.NewGuid();

        public bool IsCancellationRequested => _isCancellationRequested;
        public UniTask<T> Result => _completionSource.Task;

        public QueuedRequest(Func<CancellationToken, UniTask<T>> operation) => _operation = operation;

        public void Cancel()
        {
            if (_isCompleted || _isCancellationRequested)
                return;

            _isCancellationRequested = true;
            _cancellationSource.Cancel();
            _completionSource.TrySetCanceled();
        }

        public async UniTask ExecuteAsync(CancellationToken queueToken)
        {
            if (_isCancellationRequested)
                return;

            using var linkedTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(
                    queueToken,
                    _cancellationSource.Token);

            try
            {
                T result = await _operation(linkedTokenSource.Token);

                if (!_isCancellationRequested)
                    _completionSource.TrySetResult(result);
            }
            catch (OperationCanceledException)
            {
                _isCancellationRequested = true;
                _completionSource.TrySetCanceled();
            }
            catch (Exception exception)
            {
                _completionSource.TrySetException(exception);
            }
            finally
            {
                _isCompleted = true;
                Dispose();
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _cancellationSource.Dispose();
        }
    }
}