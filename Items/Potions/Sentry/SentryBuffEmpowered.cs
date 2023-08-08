using Aequus.Buffs.Misc.Empowered;
using Aequus.Content.ItemPrefixes.Potions;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.Sentry {
    public class SentryBuffEmpowered : EmpoweredBuffBase {
        public override int OriginalBuffType => ModContent.BuffType<SentryBuff>();

        public override void SetStaticDefaults() {
            EmpoweredPrefix.ItemToEmpoweredBuff[ModContent.ItemType<SentryPotion>()] = Type;
        }

        public override void Update(Player player, ref int buffIndex) {
            base.Update(player, ref buffIndex);
            player.maxTurrets += 2;
        }
    }
}