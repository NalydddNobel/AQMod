using Aequus.Content.ItemPrefixes.Potions;
using Aequus.Unused.Items;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Misc.Empowered {
    public class EmpoweredSentry : EmpoweredBuffBase
    {
        public override int OriginalBuffType => ModContent.BuffType<SentryBuff>();

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ModContent.ItemType<SentryPotion>(), Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.maxTurrets += 2;
        }
    }
}