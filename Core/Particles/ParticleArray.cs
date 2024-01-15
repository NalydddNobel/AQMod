using System.Collections.Generic;

namespace Aequus.Core.Particles;

/// <summary>
/// A basic particle array system, initializes an array of a specified size on-load.
/// </summary>
/// <typeparam name="T"></typeparam>
[Autoload(Side = ModSide.Client)]
public abstract class ParticleArray<T> : ParticleSystem where T : IParticle, new() {
    protected T[] Particles;

    public T New() {
        Activate();

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
        Activate();

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

    public sealed override void Load(Mod mod) {
        base.Load(mod);
        Particles = new T[ParticleCount];
        ParticleManager.Register(this);
        OnLoad();
    }
    protected virtual void OnLoad() { }

    public sealed override void Unload() {
        OnUnload();
        base.Unload();
    }
    protected virtual void OnUnload() { }
}
