using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class StariteBottleBuff : ModBuff
    {
        public override bool RightClick(int buffIndex)
        {
            return false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }

        public static void UpdateEffects(Player player, int buffIndex)
        {
            if (player.buffTime[buffIndex] == 10)
            {
                if (Main.netMode != NetmodeID.Server && player.statMana < player.statManaMax2)
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.HealMana, 20, dot: true);
                    foreach (var v in AequusHelpers.CircularVector(8))
                    {
                        var d = Dust.NewDustPerfect(player.Center + v.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * player.Size * Main.rand.NextFloat(1f, 2.5f),
                            ModContent.DustType<GhostDrainDust>(), -v * player.Size / 16f, Alpha: 255, newColor: new Color(60, 150, 255, 100));
                        d.customData = player;
                    }
                    foreach (var v in AequusHelpers.CircularVector(6))
                    {
                        var d = Dust.NewDustPerfect(player.Center + v.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * player.Size * 2f * Main.rand.NextFloat(0.5f, 2f),
                            ModContent.DustType<GhostDrainDust>(), -v * player.Size / 10f, Alpha: 255, newColor: new Color(100, 200, 255, 200), Scale: Main.rand.NextFloat(1f, 1.2f));
                        d.customData = player;
                    }
                }
                if (player.statMana < player.statManaMax2)
                {
                    player.statMana = Math.Min(player.statMana + 20, player.statManaMax2);
                }
            }
            else if (Main.netMode != NetmodeID.Server && player.statMana < player.statManaMax2 && player.buffTime[buffIndex] > 70 && Main.GameUpdateCount % 8 == 0)
            {
                var v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                var d = Dust.NewDustPerfect(player.Center + v * player.Size * Main.rand.NextFloat(1f, 2.5f),
                    ModContent.DustType<GhostDrainDust>(), -v * player.Size / 16f, Alpha: 255, newColor: new Color(100, 200, 255, 200));
                d.customData = player;
            }
        }
    }
}