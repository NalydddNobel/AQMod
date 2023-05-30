using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs {
    public class NecromancyPotionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            PotionColorsDatabase.BuffToColor.Add(Type, new Color(75, 175, 29));
            AequusBuff.AddPotionConflict(Type, BuffID.Summoning);
            AequusBuff.AddPotionConflict(Type, ModContent.BuffType<SentryBuff>());
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Aequus().ghostSlotsMax++;
        }
    }
}