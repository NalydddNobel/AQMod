using System.Collections.Generic;

namespace AequusRemake.Core.Structures.Particles;

public abstract class ParticleList<T> : IParticleSystem, IParticleEmitter<T> where T : IParticle, new() {
    internal static ParticleList<T> Instance { get; private set; }

    public int ParticleCount => _list.Count;

    public bool Active => _list.Count > 0;

    protected List<T> _list = [];
    private List<T> _swap = [];

    public virtual void OnLoad() { }
    public virtual void OnUnload() { }
    public abstract void Activate();
    public abstract void Deactivate();
    public abstract void Update(T t);
    public virtual void Draw(SpriteBatch spriteBatch, T t) { }

    public virtual void Draw(SpriteBatch spriteBatch) {
        for (int i = 0; i < _list.Count; i++) {
            Draw(spriteBatch, _list[i]);
        }
    }

    public virtual void Update() {
        // Clear the swap bag.
        _swap.Clear();

        // Iterate over the current bag.
        for (int i = 0; i < _list.Count; i++) {
            T item = _list[i];
            Update(item);

            // Add active items to the swap bag.
            if (item.Active) {
                _swap.Add(item);
            }
        }

        // Swap the bag with the swap bag.
        (_list, _swap) = (_swap, _list);
    }

    public void Load(Mod mod) {
        Instance = this;
        OnLoad();
        Particle<T>._instance = this;
    }

    public void Unload() {
        Particle<T>._instance = null;
        OnUnload();
        Instance = null;
    }

    private T NewInner() {
        T instance = new T {
            Active = true
        };
        _list.Add(instance);
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
