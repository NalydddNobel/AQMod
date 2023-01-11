using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs.Wabbajack
{
    public class WabbajackEffectParticles : ModBuff
    {
        public override string Texture => Aequus.Debuff;

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            int amt = (int)Math.Max(npc.Size.Length() / 20f, 1f);
            for (int i = 0; i < amt; i++)
            {
                if (Main.rand.NextBool(Math.Max(20 - npc.buffTime[buffIndex] / 12, 2)))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>());
                    Main.dust[d].velocity *= 0.1f;
                    Main.dust[d].scale = Main.rand.NextFloat(0.66f, 2f);
                    Main.dust[d].color = Color.Red.UseA(100) * Main.rand.NextFloat(0.4f, 1f);
                }
            }
            if (Main.rand.NextBool(20))
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Teleporter);
                Main.dust[d].scale = Main.rand.NextFloat(0.66f, 1f);
            }
        }
    }
}