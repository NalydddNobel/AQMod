using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.Pyramid {
    public class PyramidStatueBuff : ModBuff {
        public override void SetStaticDefaults() {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) {
            player.buffTime[buffIndex] = Math.Max(player.buffTime[buffIndex], 2);

            ApplyEffects(player, ref buffIndex);
        }

        protected virtual void ApplyEffects(Player player, ref int buffIndex) {

        }
    }
}