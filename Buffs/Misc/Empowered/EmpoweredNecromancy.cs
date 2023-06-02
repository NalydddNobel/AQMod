using Aequus.Content.ItemPrefixes.Potions;
using Aequus.Unused.Items;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Misc.Empowered {
    public class EmpoweredNecromancy : EmpoweredBuffBase
    {
        public override int OriginalBuffType => ModContent.BuffType<NecromancyPotionBuff>();

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ModContent.ItemType<NecromancyPotion>(), Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.Aequus().ghostSlotsMax += 2;
        }
    }
}