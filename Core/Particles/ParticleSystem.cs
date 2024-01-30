using System.Collections.Generic;

namespace Aequus.Core.Particles;

[Autoload(Side = ModSide.Client)]
public abstract class ParticleSystem<T> : IParticleSystem where T : IParticle, new() {
    protected Mod Mod { get; private set; }
    protected T[] Particles;

    public abstract int ParticleCount { get; }
    public bool Active { get; protected set; }

    public T New() {
        CheckInit();

        for (int i = 0; i < Particles.Length; i++) {
            T p = Particles[i] ??= new();
            if (!p.Active) {
                p.Active = true;
                return p;
            }
        }

        return Particles[^1];
    }

    public IEnumerable<T> NewMultiple(int count) {
        if (Main.netMode == NetmodeID.Server) {
            yield break;
        }
        CheckInit();

        for (int i = 0; i < Particles.Length; i++) {
            T p = Particles[i] ??= new();
            if (!p.Active) {
                p.Active = true;
                yield return p;

                if(--count == 0) {
                    yield break;
                }
            }
        }
    }

    private void CheckInit() {
        if (!Active) {
            Activate();
            ModContent.GetInstance<ParticleManager>()._activeSystems.AddLast(this);
        }
    }

    public abstract void Update();

    public abstract void Draw(SpriteBatch spriteBatch);

    /// <summary>
    /// Allows the renderer to subscribe to various drawing actions.
    /// This is ran when the particle system is activated.
    /// </summary>
    public abstract void Activate();
    /// <summary>
    /// Allows the renderer to unsubscribe to various drawing actions.
    /// This is ran when all particles are cleared from the system.
    /// </summary>
    public abstract void Deactivate();

    public virtual void Load(Mod mod) {
        Mod = mod;
        Particles = new T[ParticleCount];
        ParticleManager.Register(this);
        OnLoad();
    }
    protected virtual void OnLoad() { }

    public virtual void Unload() {
        OnUnload();
        Mod = null;
    }
    protected virtual void OnUnload() { }
}
