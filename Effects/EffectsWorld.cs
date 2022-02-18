using AQMod.Common.Graphics.DrawTypes;
using AQMod.Effects.Wind;
using System.Collections.Generic;
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
                GeneralEffectsManager.InternalInitalize();
                WindLayer.windDraws = new List<IDrawType>();
                Particle.PreDrawProjectiles.Initialize();
                Particle.PostDrawPlayers.Initialize();

                Trail.PreDrawProjectiles.Initialize();
            }
        }
    }
}