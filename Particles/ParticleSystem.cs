using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Particles
{
    public class ParticleSystem : ModSystem
    {
        internal class ParticlePools<T> where T : BaseParticle<T>
        {
            public static ParticlePool<T> Pool;
        }

        private static ParticleRenderer[] layers;

        public static T Fetch<T>() where T : BaseParticle<T>
        {
            return ParticlePools<T>.Pool.RequestParticle();
        }

        public static ParticleRenderer GetLayer(int i)
        {
            return layers[i];
        }

        public static T New<T>(int layer) where T : BaseParticle<T>
        {
            var value = Fetch<T>();
            layers[layer].Add(value);
            return value;
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            layers = new ParticleRenderer[ParticleLayer.Count];
            for (byte i = 0; i < ParticleLayer.Count; i++)
            {
                layers[i] = new ParticleRenderer();
            }
        }
        public override void Unload()
        {
            layers = null;
        }

        public void InitWorldData()
        {
            if (Main.dedServ)
                return;
            foreach (var layer in layers)
                layer.Particles.Clear();
        }

        public override void OnWorldLoad()
        {
            InitWorldData();
        }

        public override void OnWorldUnload()
        {
            InitWorldData();
        }

        public override void PreUpdatePlayers()
        {
            if (Main.dedServ)
                return;

            foreach (var l in layers)
            {
                l.Update();
            }
        }
    }

    public class ParticleLayer
    {
        public const int BehindAllNPCs = 0;
        public const int BehindProjs = 1;
        public const int BehindPlayers = 2;
        public const int AbovePlayers = 3;
        public const int AboveDust = 4;
        public const int AboveLiquid = 5;
        public const int Count = 6;
    }
}