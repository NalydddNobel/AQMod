using ReLogic.Threading;

namespace Aequus.Core.Particles;

public abstract class ParallelParticleSystem<T> : ParticleSystem<T> where T : IParticle, new() {
    public override void Update() {
        Active = false;
        FastParallel.For(0, Particles.Length, UpdateCallback, this);
    }

    internal void UpdateCallback(int fromInclusive, int toExclusive, object context) {
        ((ParallelParticleSystem<T>)context).UpdateParallel(fromInclusive, toExclusive);
    }

    protected abstract void UpdateParallel(int start, int end);
}