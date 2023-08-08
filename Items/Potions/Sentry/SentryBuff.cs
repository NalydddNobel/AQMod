using Aequus.Common.Buffs;
using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.Sentry {
    public class SentryBuff : ModBuff {
        public override void SetStaticDefaults() {
            PotionColorsDatabase.BuffToColor.Add(Type, Color.SandyBrown);
            AequusBuff.AddPotionConflict(Type, BuffID.Summoning);
        }

        public override void Update(Player player, ref int buffIndex) {
            player.maxTurrets++;
        }
    }
}