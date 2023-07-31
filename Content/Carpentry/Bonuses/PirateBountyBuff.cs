using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Carpentry.Bonuses {
    public class PirateBountyBuff : ModBuff {
        public override void SetStaticDefaults() {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) {
            player.fishingSkill += 15;
            player.Aequus().shopPriceMultiplier -= 0.1f;
        }

        public override bool RightClick(int buffIndex) {
            return false;
        }
    }
}