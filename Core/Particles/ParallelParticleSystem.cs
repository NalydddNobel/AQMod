using ReLogic.Threading;

namespace Aequus.Core.Particles;

public abstract class ParallelParticleSystem<T> : ParticleSystem<T> where T : IParticle, new() {
    public override void Update() {
        Active = false;
        FastParallel.For(0, Particles.Length, UpdateCallback, this);
    }

    internal void UpdateCallback(System.Int32 fromInclusive, System.Int32 toExclusive, System.Object context) {
        ((ParallelParticleSystem<T>)context).UpdateParallel(fromInclusive, toExclusive);
    }

    protected abstract void UpdateParallel(System.Int32 start, System.Int32 end);
}