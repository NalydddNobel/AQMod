using System;
using System.Collections.Generic;

namespace Aequu2.Core.Particles;

internal interface IParticleEmitter<T> where T : IParticle, new() {
    /// <returns>A single particle instance.</returns>
    T New();

    /// <summary>
    /// Gets multiple particle instances.
    /// Use <see cref="NewMultipleReduced(int, int)"/>, if you want to spawn particles based on <see cref="Main.gfxQuality"/>.
    /// </summary>
    /// <param name="count">The amount of particles wanted.</param>
    IEnumerable<T> NewMultiple(int count);

    /// <summary>
    /// Gets multiple particle instances. <paramref name="count"/> is multiplied by <see cref="Main.gfxQuality"/>, 
    /// but returns atleast <paramref name="minimum"/> amount of particles.
    /// </summary>
    /// <param name="count">The amount of particles wanted. This is multiplied <see cref="Main.gfxQuality"/>.</param>
    /// <param name="minimum">The minimum amount of particles to return. <paramref name="count"/> is multiplied by <see cref="Main.gfxQuality"/>, which reduces it depending on quality settings.</param>
    IEnumerable<T> NewMultipleReduced(int count, int minimum = 1) {
        return NewMultiple(Math.Clamp((int)(count * Main.gfxQuality), minimum, count));
    }
}
