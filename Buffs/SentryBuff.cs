using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs {
    public class SentryBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            PotionColorsDatabase.BuffToColor.Add(Type, new Color(175, 75, 29));
            AequusBuff.AddPotionConflict(Type, BuffID.Summoning);
            AequusBuff.AddPotionConflict(Type, ModContent.BuffType<NecromancyPotionBuff>());
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.maxTurrets++;
        }
    }
}