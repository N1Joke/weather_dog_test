using Core;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Assets._Project.Scripts.Network
{
    public class QueryManager : BaseDisposable // to do interface ServiseProvider
    {
        private readonly Queue<Request> _queue = new();
        private bool _isRunning;
        private CancellationTokenSource _currentCts;

        private class Request
        {
            public Func<CancellationToken, UniTask> TaskFunc;
            public string Tag;
            public Action OnCanceled;

            public Request(string tag, Func<CancellationToken, UniTask> func, Action onCancel)
            {
                Tag = tag;
                TaskFunc = func;
                OnCanceled = onCancel;
            }
        }

        public void Enqueue(string tag, Func<CancellationToken, UniTask> func, Action onCancel = null)
        {
            _queue.Enqueue(new Request(tag, func, onCancel));
            if (!_isRunning)
                RunQueue().Forget();
        }

        public void RemoveByTag(string tag)
        {
            var filtered = new Queue<Request>();
            while (_queue.Count > 0)
            {
                var req = _queue.Dequeue();
                if (req.Tag != tag)
                    filtered.Enqueue(req);
                else
                    req.OnCanceled?.Invoke();
            }
            foreach (var r in filtered) _queue.Enqueue(r);
        }

        public void CancelCurrent()
        {
            _currentCts?.Cancel();
        }

        private async UniTaskVoid RunQueue()
        {
            _isRunning = true;
            while (_queue.Count > 0)
            {
                var req = _queue.Dequeue();
                _currentCts = new CancellationTokenSource();
                try
                {
                    await req.TaskFunc(_currentCts.Token);
                }
                catch (OperationCanceledException) { req.OnCanceled?.Invoke(); }
            }
            _isRunning = false;
        }
    }
}
