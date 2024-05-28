using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aequus.Core.Particles;

public abstract class ConcurrentParticles<T> : IParticleSystem, IParticleEmitter<T> where T : IParticle, new() {
    internal static ConcurrentParticles<T> Instance { get; private set; }

    public int ParticleCount => _bag.Count;

    public bool Active => !_bag.IsEmpty;

    private ConcurrentBag<T> _bag = [];
    private ConcurrentBag<T> _bagSwap = [];

    public abstract void Activate();
    public abstract void Deactivate();
    public abstract void Update(T t);
    public abstract void Draw(SpriteBatch spriteBatch, T t);

    public void Draw(SpriteBatch spriteBatch) {
        foreach (T item in _bag) {
            Draw(spriteBatch, item);
        }
    }

    public void Update() {
        // Clear the swap bag.
        _bagSwap.Clear();

        // Iterate over the current bag.
        Parallel.ForEach(_bag, item => {
            Update(item);

            // Add active items to the swap bag.
            if (item.Active) {
                _bagSwap.Add(item);
            }
        });

        // Swap the bag with the swap bag.
        (_bag, _bagSwap) = (_bagSwap, _bag);
    }

    public void Load(Mod mod) {
        Instance = this;
    }

    public void Unload() {
        Instance = null;
    }

    private T NewInner() {
        T instance = new T();
        instance.Active = true;
        _bag.Add(instance);
        return instance;
    }

    T IParticleEmitter<T>.New() {
        if (!Active) {
            Activate();
            ModContent.GetInstance<ParticleManager>()._activeSystems.AddLast(this);
        }

        return NewInner();
    }

    IEnumerable<T> IParticleEmitter<T>.NewMultiple(int count) {
        if (!Active) {
            Activate();
            ModContent.GetInstance<ParticleManager>()._activeSystems.AddLast(this);
        }

        for (int i = 0; i < count; i++) {
            yield return NewInner();
        }
    }
}
