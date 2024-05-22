using System;
using System.Collections.Generic;

namespace Aequus.Core.Particles;

/// <summary>A basic particle array system, initializes an array of a specified size on-load.</summary>
[Autoload(Side = ModSide.Client)]
public abstract class ParticleArray<T> : IParticleSystem where T : IParticle, new() {
    protected Mod Mod { get; private set; }

    protected T[] Particles;

    public abstract int ParticleCount { get; }
    public bool Active { get; protected set; }

    internal static ParticleArray<T> Instance { get; private set; }

    /// <returns>A single particle instance.</returns>
    public static T New() {
        return Instance.NewInner();
    }

    /// <summary>
    /// Gets multiple particle instances, used to reduce <see cref="CheckInit"/> calls. 
    /// Use <see cref="NewMultipleReduced(int, int)"/>, if you want to spawn particles based on <see cref="Main.gfxQuality"/>.
    /// </summary>
    /// <param name="count">The amount of particles wanted.</param>
    public static IEnumerable<T> NewMultiple(int count) {
        return Instance.NewMultipleInner(count);
    }

    /// <summary>
    /// Gets multiple particle instances, used to reduce <see cref="CheckInit"/> calls. <paramref name="count"/> is multiplied by <see cref="Main.gfxQuality"/>, 
    /// but returns atleast <paramref name="minimum"/> amount of particles.
    /// </summary>
    /// <param name="count">The amount of particles wanted. This is multiplied <see cref="Main.gfxQuality"/>.</param>
    /// <param name="minimum">The minimum amount of particles to return. <paramref name="count"/> is multiplied by <see cref="Main.gfxQuality"/>, which reduces it depending on quality settings.</param>
    public static IEnumerable<T> NewMultipleReduced(int count, int minimum = 1) {
        return Instance.NewMultipleInner(Math.Clamp((int)(count * Main.gfxQuality), minimum, count));
    }

    /// <returns><inheritdoc cref="New"/></returns>
    internal T NewInner() {
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

    /// <summary><inheritdoc cref="NewMultiple(int)"/></summary>
    internal IEnumerable<T> NewMultipleInner(int count) {
        if (Main.netMode == NetmodeID.Server) {
            yield break;
        }
        CheckInit();

        for (int i = 0; i < Particles.Length; i++) {
            T p = Particles[i] ??= new();
            if (!p.Active) {
                p.Active = true;
                yield return p;

                if (--count == 0) {
                    yield break;
                }
            }
        }
    }

    private void CheckInit() {
        if (!Active) {
            Particles ??= new T[ParticleCount];

            Activate();
            ModContent.GetInstance<ParticleManager>()._activeSystems.AddLast(this);
        }
    }

    public abstract void Update();

    public abstract void Draw(SpriteBatch spriteBatch);

    public void Activate() {
        if (!Active) {
            Active = true;
            ModContent.GetInstance<ParticleManager>()._activeSystems.AddLast(this);
            OnActivate();
        }
    }
    /// <summary>
    /// Allows the renderer to subscribe to various drawing actions.
    /// This is ran when the particle system is activated.
    /// </summary>
    public virtual void OnActivate() { }

    /// <summary>
    /// Allows the renderer to unsubscribe to various drawing actions.
    /// This is ran when all particles are cleared from the system.
    /// </summary>
    public abstract void Deactivate();

    public void Load(Mod mod) {
        Mod = mod;
        Instance = this;
    }
    public void Unload() {
        OnUnload();
        Instance = null;
        Mod = null;
    }
    protected virtual void OnUnload() { }
}
