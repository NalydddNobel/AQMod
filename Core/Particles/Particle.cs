using System.Collections.Generic;

namespace Aequu2.Core.Particles;

public static class Particle<T> where T : IParticle, new() {
    internal static IParticleEmitter<T> _instance;

    /// <returns><inheritdoc cref="IParticleEmitter{T}.New"/></returns>
    public static T New() {
        return _instance.New();
    }

    /// <summary><inheritdoc cref="IParticleEmitter{T}.NewMultiple(int)"/></summary>
    public static IEnumerable<T> NewMultiple(int count) {
        return _instance.NewMultiple(count);
    }

    /// <summary><inheritdoc cref="IParticleEmitter{T}.NewMultipleReduced(int, int)"/></summary>
    public static IEnumerable<T> NewMultipleReduced(int count, int minimum = 1) {
        return _instance.NewMultipleReduced(count, minimum);
    }
}
