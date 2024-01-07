using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Terraria.Graphics.Renderers;

namespace Aequus.Common.Particles;
public class ParticleSystem : ModSystem {
    internal class ParticlePools<T> where T : BaseParticle<T>, new() {
        public static ParticlePool<T> Pool;
    }

    private static ParticleRenderer[] layers;
    internal static List<ParticleBatch> Batches { get; private set; } = new();

    public static T Fetch<T>() where T : BaseParticle<T>, new() {
        return ParticlePools<T>.Pool.RequestParticle();
    }

    public static ParticleRenderer GetLayer(ParticleLayer layer) {
        return layers[(int)layer];
    }

    /// <summary>
    /// Do not use on servers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static T New<T>(ParticleLayer layer) where T : BaseParticle<T>, new() {
        var value = Fetch<T>();
        layers[(int)layer].Add(value);
        return value;
    }

    public override void Load() {
        if (Main.dedServ) {
            return;
        }

        layers = new ParticleRenderer[(int)ParticleLayer.Count];
        for (byte i = 0; i < (int)ParticleLayer.Count; i++) {
            layers[i] = new ParticleRenderer();
        }
    }

    public override void Unload() {
        Batches.Clear();
        layers = null;
    }

    public override void ClearWorld() {
        if (Main.dedServ) {
            return;
        }

        foreach (var layer in layers) {
            layer.Particles.Clear();
        }
    }

    public override void PreUpdatePlayers() {
        if (Main.dedServ) {
            return;
        }

        foreach (var l in layers) {
            l.Particles = l.Particles.Distinct().ToList();
            l.Update();
        }
        foreach (var batch in Batches) {
            batch.Update();
        }
    }
}