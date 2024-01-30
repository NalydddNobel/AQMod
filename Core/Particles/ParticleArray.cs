using System;
using System.Collections.Generic;

namespace Aequus.Core.Particles;

[Autoload(Side = ModSide.Client)]
public abstract class ParticleArray<T> : IParticleSystem where T : IParticle, new() {
    protected Mod Mod { get; private set; }
    protected T[] Particles;

    public abstract int ParticleCount { get; }
    public bool Active { get; protected set; }

    /// <returns>A single particle instance.</returns>
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

    /// <summary>
    /// Gets multiple particle instances, used to reduce <see cref="CheckInit"/> calls. Unlike <see cref="NewMultipleReduced(int, int)"/>, 
    /// <paramref name="count"/> is NOT multiplied by <see cref="Main.gfxQuality"/>.
    /// </summary>
    /// <param name="count">The amount of particles wanted.</param>
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

    /// <summary>
    /// Gets multiple particle instances, used to reduce <see cref="CheckInit"/> calls. <paramref name="count"/> is multiplied by <see cref="Main.gfxQuality"/>, 
    /// but returns atleast <paramref name="minimum"/> amount of particles.
    /// </summary>
    /// <param name="count">The amount of particles wanted. This is multiplied <see cref="Main.gfxQuality"/>.</param>
    /// <param name="minimum">The minimum amount of particles to return. <paramref name="count"/> is multiplied by <see cref="Main.gfxQuality"/>, which reduces it depending on quality settings.</param>
    public IEnumerable<T> NewMultipleReduced(int count, int minimum = 1) {
        return NewMultiple(Math.Clamp((int)(count * Main.gfxQuality), minimum, count));
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
