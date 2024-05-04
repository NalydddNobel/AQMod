using System.Collections.Generic;

namespace Aequus.Core;

public abstract class RequestHandler<T> : ModType {
    public bool Prepared { get; private set; }
    public bool Active => _active;

    private bool _active;

    protected readonly Queue<T> _queue = new();

    protected abstract bool HandleRequests(IEnumerable<T> todo);
    protected virtual void OnActivate() { }
    protected virtual void OnDeactivate() { }

    protected sealed override void Register() {
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }

    public void HandleRequests() {
        if (!_active) {
            return;
        }

        if (_queue.Count == 0) {
            OnDeactivate();
            _active = false;
            return;
        }

        Prepared = false;

        if (HandleRequests(_queue)) {
            Prepared = true;
        }
    }

    protected void ClearQueue() {
        _queue.Clear();
    }

    public void Enqueue(T item) {
        if (!_active) {
            OnActivate();
            _active = true;
        }

        _queue.Enqueue(item);
    }
}
