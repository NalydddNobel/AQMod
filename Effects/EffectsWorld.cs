using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Effects
{
    internal sealed class EffectsWorld : ModWorld
    {
        public override void Initialize()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                FX.InternalInitalize();
                AQMod.Particles.PreDrawProjectiles.Initialize();
                AQMod.Particles.PostDrawPlayers.Initialize();

                AQMod.Trails.PreDrawProjectiles.Initialize();
            }
        }
    }
}