using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.Graphics.Particles
{
    internal sealed class ParticleWorld : ModWorld
    {
        public override void Initialize()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Particle.PreDrawProjectiles.Initialize();
                Particle.PostDrawPlayers.Initialize();
            }
        }

        public override void PostUpdate()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Particle.PreDrawProjectiles.UpdateParticles();
                Particle.PostDrawPlayers.UpdateParticles();
            }
        }
    }
}