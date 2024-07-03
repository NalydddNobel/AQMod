using System.Collections.Generic;

namespace Aequu2.Core.Particles;

/// <summary>A basic particle array system, initializes an array of a specified size on-load.</summary>
[Autoload(Side = ModSide.Client)]
public abstract class ParticleArray<T> : IParticleSystem, IParticleEmitter<T> where T : IParticle, new() {
    protected Mod Mod { get; private set; }

    protected T[] Particles;

    public abstract int ParticleCount { get; }
    public bool Active { get; protected set; }

    /// <returns><inheritdoc cref="IParticleEmitter{T}.New"/></returns>
    public static T New() {
        return Particle<T>.New();
    }

    /// <summary><inheritdoc cref="IParticleEmitter{T}.NewMultiple(int)"/></summary>
    public static IEnumerable<T> NewMultiple(int count) {
        return Particle<T>.NewMultiple(count);
    }

    /// <summary><inheritdoc cref="IParticleEmitter{T}.NewMultipleReduced(int, int)"/></summary>
    public static IEnumerable<T> NewMultipleReduced(int count, int minimum = 1) {
        return Particle<T>.NewMultipleReduced(count, minimum);
    }

    T IParticleEmitter<T>.New() {
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

    IEnumerable<T> IParticleEmitter<T>.NewMultiple(int count) {
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
    public abstract void OnActivate();

    /// <summary>
    /// Allows the renderer to unsubscribe to various drawing actions.
    /// This is ran when all particles are cleared from the system.
    /// </summary>
    public abstract void Deactivate();

    public void Load(Mod mod) {
        Mod = mod;
        Particle<T>._instance = this;
    }
    public void Unload() {
        OnUnload();
        Particle<T>._instance = null;
        Mod = null;
    }
    protected virtual void OnUnload() { }
}
